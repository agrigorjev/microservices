using Mandara.Business.Audit;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Mandara.Business.Services.Users;
using Optional;

namespace Mandara.Business.Managers
{
    public class AuthorizationManager : IAuthorisationManager
    {
        private const int MaxFailedAuthAttempts = 5;

        private readonly IUsersRepository _users = new UserRepository(
            MandaraEntities.DefaultConnStrName,
            nameof(Client.Managers.AuthorizationManager));

        [Obsolete("Pass in an IUsersRepository")]
        public AuthorizationManager()
        {
            _users = new UserRepository(
                ConfigurationManager.ConnectionStrings[MandaraEntities.DefaultConnStrName].ConnectionString,
                nameof(Client.Managers.AuthorizationManager));
        }

        public AuthorizationManager(IUsersRepository users)
        {
            _users = users
                     ?? throw new ArgumentNullException(
                         $"{nameof(AuthorizationManager)}: Cannot construct with a null users repository");
        }

        public IEnumerable<Permission> GetUserPermissions(string userName)
        {
            return _users.TryGetUser(userName).Match(GetUserPermissions, () => new List<Permission>());
        }

        public IEnumerable<Permission> GetUserPermissions(User user)
        {
            List<Permission> permissions = new List<Permission>();
            IEnumerable<Group> allUserGroups = GetGroups();
            HashSet<int> userGroupMembership = user.Groups.Select(group => group.GroupId).ToHashSet();

            foreach (var group in allUserGroups.Where(grp => userGroupMembership.Contains(grp.GroupId)))
            {
                permissions.AddRange(group.Permissions);
            }

            return permissions.Distinct();
        }

        public IEnumerable<User> GetUsers()
        {
            return _users.GetUsers();
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(AuthorizationManager));
        }

        public TryGetResult<User> TryGetUserForAlias(string userAlias)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                User user = cxt
                    .Users
                    .Include(u => u.Portfolio)
                    .Include(u => u.UserAliases)
                    .Include(u => u.PortfolioPermissions)
                    .FirstOrDefault(usr => usr.UserAliases.Any(alias => alias.Alias == userAlias));

                return new TryGetRef<User>() { Value = user };
            }
        }

        public IEnumerable<Group> GetGroups()
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                return cxt.Groups.Include("Permissions").ToList();
            }
        }

        public List<AuditMessage> GetAuditMessages(DateTime? filterDate = null)
        {
            using (var context = CreateMandaraProductsDbContext())
            {
                context.Database.CommandTimeout = 0;
                DbQuery<AuditMessage> query = context.AuditMessages.Include("User").AsNoTracking();

                if (filterDate.HasValue)
                {
                    DateTime startDate = filterDate.Value.Date;
                    DateTime endDate = startDate.AddDays(1);

                    query = (DbQuery<AuditMessage>)
                        query.Where(am => startDate <= am.MessageTime && am.MessageTime < endDate);
                }

                return query.ToList();
            }
        }

        public List<Portfolio> GetPortfolios()
        {
            using (var context = CreateMandaraProductsDbContext())
            {
                return context.Portfolios.ToList();
            }
        }

        public void SaveGroups(List<Group> groupsToSave, AuditContext auditContext)
        {
            using (var cxt = CreateMandaraProductsDbContext())
            using (var transaction = cxt.Database.BeginTransaction())
            {
                List<Group> groups = cxt.Groups.Include("Permissions").ToList();
                List<Permission> permissions = cxt.Permissions.ToList();

                foreach (var updatedGroup in groupsToSave)
                {
                    Group existingGroup = groups.SingleOrDefault(x => x.GroupId == updatedGroup.GroupId);

                    if (existingGroup != null)
                    {
                        cxt.Entry(existingGroup).CurrentValues.SetValues(updatedGroup);

                        existingGroup.Permissions.Clear();

                        foreach (var p in updatedGroup.Permissions)
                        {
                            var permission = permissions.SingleOrDefault(x => x.PermissionId == p.PermissionId);
                            if (permission != null)
                                existingGroup.Permissions.Add(permission);
                        }
                    }
                    else
                    {
                        var newGroup = new Group
                        {
                            GroupName = updatedGroup.GroupName,
                            Permissions = new List<Permission>()
                        };

                        foreach (var p in updatedGroup.Permissions)
                        {
                            var permission = permissions.SingleOrDefault(x => x.PermissionId == p.PermissionId);
                            if (permission != null)
                                newGroup.Permissions.Add(permission);
                        }

                        cxt.Groups.Add(newGroup);
                    }
                }

                List<Group> updatedEntities =
                    groupsToSave.Where(g => groups.Any(gr => gr.GroupId == g.GroupId)).ToList();
                List<Group> deletedEntities =
                    groups.Where(g => !updatedEntities.Any(gr => gr.GroupId == g.GroupId)).ToList();

                foreach (Group deletedGroup in deletedEntities)
                    cxt.Groups.Remove(deletedGroup);

                cxt.SaveChanges();
                transaction.Commit();
            }
        }

        public void DeleteUser(User user, AuditContext auditContext)
        {
            if (user == null || user.UserId == 0)
                return;

            using (var ctx = CreateMandaraProductsDbContext())
            {
                var dbUser = ctx.Users.Include("Groups")
                    .Include("PortfolioPermissions")
                    .Include("UserAliases")
                    .SingleOrDefault(u => u.UserId == user.UserId);

                if (dbUser != null)
                {
                    dbUser.UserAliases.Clear();
                    dbUser.Groups.Clear();
                    dbUser.PortfolioPermissions.Clear();
                    ctx.Users.Remove(dbUser);

                    ctx.SaveChanges();
                }
            }
        }

        public void SaveUser(User user, AuditContext auditContext)
        {
            using (var ctx = CreateMandaraProductsDbContext())
            {
                List<Group> groups = ctx.Groups.ToList();

                User dbUser = ctx.Users.Include("Groups").
                    Include("PortfolioPermissions").
                    Include("UserAliases").
                    Include("ProductGroupPortfolios").
                    Include("ProductGroupPortfolios.Portfolio").
                    Include("Portfolio").SingleOrDefault(u => u.UserId == user.UserId);

                if (dbUser == null)
                {
                    dbUser = new User();
                    ctx.Users.Add(dbUser);
                }

                dbUser.UserName = user.UserName;
                dbUser.LastName = user.LastName;
                dbUser.FirstName = user.FirstName;
                dbUser.MasterPasswordHash = user.MasterPasswordHash;
                dbUser.Locked = user.Locked;

                dbUser.DaysBetweenPasswordChange = user.DaysBetweenPasswordChange;
                dbUser.FailedAuthAttempts = user.FailedAuthAttempts;
                dbUser.ForcePasswordChange = user.ForcePasswordChange;
                dbUser.LastPasswordChange = user.LastPasswordChange;

                dbUser.Groups.Clear();
                foreach (var g in user.Groups)
                {
                    dbUser.Groups.Add(groups.Where(x => x.GroupId == g.GroupId).Single());
                }

                var newAliases =
                    user.UserAliases.Where(ua => !dbUser.UserAliases.Any(a => a.Alias == ua.Alias))
                        .Where(a => !string.IsNullOrEmpty(a.Alias))
                        .ToList();
                var deletedAleases =
                    dbUser.UserAliases.Where(ua => !user.UserAliases.Any(a => a.Alias == ua.Alias)).ToList();
                foreach (var newAlias in newAliases)
                {
                    var alias = new UserAlias { UserId = dbUser.UserId, Alias = newAlias.Alias };
                    ctx.UserAliases.Add(alias);
                    dbUser.UserAliases.Add(alias);
                }
                foreach (var deletedAlias in deletedAleases)
                {
                    dbUser.UserAliases.Remove(deletedAlias);
                    ctx.UserAliases.Remove(deletedAlias);
                }

                if (user.Portfolio == null)
                {
                    dbUser.Portfolio = null;
                }
                else if (dbUser.Portfolio == null || dbUser.Portfolio.PortfolioId != user.Portfolio.PortfolioId)
                {
                    dbUser.Portfolio = ctx.Portfolios.SingleOrDefault(p => p.PortfolioId == user.Portfolio.PortfolioId);
                }

                List<UserPortfolioPermission> newPermissions =
                    user.PortfolioPermissions.Where(
                        pp => !dbUser.PortfolioPermissions.Any(p => p.PortfolioId == pp.PortfolioId)).ToList();
                List<UserPortfolioPermission> editedPermissions =
                    user.PortfolioPermissions.Where(
                        pp => dbUser.PortfolioPermissions.Any(p => p.PortfolioId == pp.PortfolioId)).ToList();
                List<UserPortfolioPermission> deletedPermissions =
                    dbUser.PortfolioPermissions.Where(
                        pp => !user.PortfolioPermissions.Any(p => p.PortfolioId == pp.PortfolioId)).ToList();

                foreach (var newPermission in newPermissions)
                {
                    var pp = new UserPortfolioPermission
                    {
                        PortfolioId = newPermission.PortfolioId,
                        CanViewRisk = newPermission.CanViewRisk,
                        CanViewPnl = newPermission.CanViewPnl,
                        CanAddEditTrades = newPermission.CanAddEditTrades,
                        CanUseMasterTool = newPermission.CanUseMasterTool,
                        CanAddEditBooks = newPermission.CanAddEditBooks,
                        CanEditBrokerage = newPermission.CanEditBrokerage,
                    };

                    ctx.UserPortfolioPermissions.Add(pp);
                    dbUser.PortfolioPermissions.Add(pp);
                }

                if (newPermissions.Count > 0)
                    TrackOnlyAsDetail(dbUser.PortfolioPermissions.ToList());

                foreach (var deletedPermission in deletedPermissions)
                {
                    dbUser.PortfolioPermissions.Remove(deletedPermission);
                    ctx.UserPortfolioPermissions.Remove(deletedPermission);
                }

                foreach (var editedPermission in editedPermissions)
                {
                    var dbPermission =
                        dbUser.PortfolioPermissions.SingleOrDefault(p => p.PortfolioId == editedPermission.PortfolioId);
                    if (dbPermission == null)
                        continue;

                    bool changed = dbPermission.CanViewRisk != editedPermission.CanViewRisk ||
                                   dbPermission.CanViewPnl != editedPermission.CanViewPnl ||
                                   dbPermission.CanAddEditTrades != editedPermission.CanAddEditTrades ||
                                   dbPermission.CanUseMasterTool != editedPermission.CanUseMasterTool ||
                                   dbPermission.CanEditBrokerage != editedPermission.CanEditBrokerage ||
                                   dbPermission.CanAddEditBooks != editedPermission.CanAddEditBooks;

                    dbPermission.CanViewRisk = editedPermission.CanViewRisk;
                    dbPermission.CanViewPnl = editedPermission.CanViewPnl;
                    dbPermission.CanAddEditTrades = editedPermission.CanAddEditTrades;
                    dbPermission.CanUseMasterTool = editedPermission.CanUseMasterTool;
                    dbPermission.CanAddEditBooks = editedPermission.CanAddEditBooks;
                    dbPermission.CanEditBrokerage = editedPermission.CanEditBrokerage;
                }

                var newPortfolioGroups =
                    user.ProductGroupPortfolios.Where(
                        pg =>
                            !dbUser.ProductGroupPortfolios.Any(
                                db =>
                                    db.Portfolio.PortfolioId == pg.Portfolio.PortfolioId &&
                                    db.category_id == pg.category_id)).ToList();
                var changedPortfolioGroups =
                    dbUser.ProductGroupPortfolios.Where(
                        db =>
                            !user.ProductGroupPortfolios.Any(
                                pg =>
                                    db.Portfolio.PortfolioId == pg.Portfolio.PortfolioId &&
                                    db.category_id == pg.category_id)).ToList();
                var portfolios = ctx.Portfolios.ToList();
                foreach (var portfolioGroup in newPortfolioGroups)
                {
                    var portfolio =
                        portfolios.SingleOrDefault(p => p.PortfolioId == portfolioGroup.Portfolio.PortfolioId);
                    if (portfolio != null)
                    {
                        UserProductCategoryPortfolio group = new UserProductCategoryPortfolio
                        {
                            category_id = portfolioGroup.category_id
                        };
                        group.Portfolio = portfolio;
                        dbUser.ProductGroupPortfolios.Add(group);
                        ctx.UserProductCategoryPortfolios.Add(group);
                    }
                }
                foreach (var portfolioGroup in changedPortfolioGroups)
                {
                    dbUser.ProductGroupPortfolios.Remove(portfolioGroup);
                    ctx.UserProductCategoryPortfolios.Remove(portfolioGroup);
                }

                ctx.SaveChanges();
            }
        }

        private static void TrackOnlyAsDetail(List<UserPortfolioPermission> newPermissions)
        {
            FieldInfo trackOnlyAsDetailField = typeof(UserPortfolioPermission).GetField("_trackOnlyAsDetail",
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            FieldInfo isChangeTrackedField = typeof(UserPortfolioPermission).GetField("_isChangeTracked",
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            foreach (UserPortfolioPermission permission in newPermissions)
            {
                if (trackOnlyAsDetailField != null)
                    trackOnlyAsDetailField.SetValue(permission, true);

                if (isChangeTrackedField != null)
                    isChangeTrackedField.SetValue(permission, false);
            }
        }

        public void SaveUsers(List<User> usersToSave)
        {
            using (var cxt = CreateMandaraProductsDbContext())
            {
                var users = cxt.Users.Include("Groups").ToList();
                var groups = cxt.Groups.ToList();

                foreach (var updatedUser in usersToSave)
                {
                    var existingUser = users.Where(x => x.UserId == updatedUser.UserId).SingleOrDefault();

                    if (existingUser != null)
                    {
                        cxt.Entry(existingUser).CurrentValues.SetValues(updatedUser);

                        existingUser.Groups.Clear();

                        foreach (var g in updatedUser.Groups)
                        {
                            existingUser.Groups.Add(groups.Where(x => x.GroupId == g.GroupId).Single());
                        }
                    }
                    else
                    {
                        var newUser = new User
                        {
                            UserName = updatedUser.UserName,
                            FirstName = updatedUser.FirstName,
                            LastName = updatedUser.LastName,
                            Groups = new List<Group>()
                        };

                        foreach (var g in updatedUser.Groups)
                        {
                            newUser.Groups.Add(groups.Where(x => x.GroupId == g.GroupId).Single());
                        }

                        cxt.Users.Add(newUser);
                    }
                }

                var updatedEntities = usersToSave.Intersect(users).ToList();
                var deletedEntities = users.Except(updatedEntities).ToList();

                foreach (var deletedUser in deletedEntities)
                    cxt.Users.Remove(deletedUser);

                cxt.SaveChanges();
            }
        }

        public User GetUserByName(string userName)
        {
            return UserByName(userName).Match(user => user, () => null);
        }

        private Option<User> UserByName(string name)
        {
            return _users.TryGetUser(name);
        }

        public User GetUserById(int userId)
        {
            return _users.TryGetUser(userId).Match(user => user, () => null);
        }

        public User AuthorizeUser(string userName, string masterPasswordHash)
        {
            return UserByName(userName).Match(AuthorisedUser, () => null);

            User AuthorisedUser(User user)
            {
                return IsAuthorised(masterPasswordHash, user) ? user : null;
            }
        }

        private bool IsAuthorised(string masterPasswordHash, User user)
        {
            if (masterPasswordHash == null)
            {
                return true;
            }

            bool authorized = CheckPassword(user, masterPasswordHash);

            ApplySecurityPolicy(user, authorized);
            return authorized;
        }

        private bool CheckPassword(User user, string masterPasswordHash)
        {
            return masterPasswordHash.Equals(user.MasterPasswordHash);
        }

        private void ApplySecurityPolicy(User user, bool authorized)
        {
            bool userChanged = user.FailedAuthAttempts > 0;

            if (authorized)
            {
                user.FailedAuthAttempts = 0;

                if (user.DaysBetweenPasswordChange > 0
                    &&
                    (InternalTime.UtcNow() - (user.LastPasswordChange ?? DateTime.MinValue)).Days >=
                    user.DaysBetweenPasswordChange)
                {
                    userChanged = true;
                    user.ForcePasswordChange = PasswordChangeReason.PasswordExpired;
                }
            }
            else
            {
                user.FailedAuthAttempts = (user.FailedAuthAttempts ?? 0) + 1;

                if (user.FailedAuthAttempts >= MaxFailedAuthAttempts)
                {
                    userChanged |= !user.LockedValue;
                    user.Locked = true;
                }
            }

            if (userChanged)
            {
                SaveUser(user, null);
            }
        }

        public bool ChangePassword(User user, string newPasswordHash)
        {
            if (user.MasterPasswordHash == newPasswordHash)
                return false;

            user.MasterPasswordHash = newPasswordHash;
            user.LastPasswordChange = SystemTime.Now().ToUniversalTime();
            user.ForcePasswordChange = PasswordChangeReason.NoForce;
            SaveUser(user, null);
            return true;
        }

        public void AddAllPermissionsForPortfolio(string userName, Portfolio portfolio)
        {
            UserByName(userName).MatchSome(AddPortfolioPermissions);

            void AddPortfolioPermissions(User user)
            {
                user.PortfolioPermissions.Add(new UserPortfolioPermission
                {
                    PortfolioId = portfolio.PortfolioId,
                    CanViewRisk = true,
                    CanViewPnl = true,
                    CanAddEditTrades = true,
                    CanUseMasterTool = true,
                    CanAddEditBooks = true,
                    CanEditBrokerage = true,
                });

                SaveUser(user, null);
            }
        }

        public bool IsUserAuthorizedToPortfolio(User user, int portfolioId, PortfolioPermission permissionType)
        {
            return user.PortfolioPermissions.Any(it =>
                it.PortfolioId == portfolioId &&
                (permissionType == PortfolioPermission.CanViewPnl && it.CheckWithParents(user, p => p.CanViewPnlValue)
                || permissionType == PortfolioPermission.CanViewRisk && it.CheckWithParents(user, p => p.CanViewRiskValue)
                || permissionType == PortfolioPermission.CanAddEditBooks && it.CheckWithParents(user, p => p.CanAddEditBooksValue)
                || permissionType == PortfolioPermission.CanAddEditTrades && it.CheckWithParents(user, p => p.CanAddEditTradesValue)
                || permissionType == PortfolioPermission.CanAmendBrokerage && it.CheckWithParents(user, p => p.CanAmendBrokerageValue)
                || permissionType == PortfolioPermission.CanUseMasterTool && it.CheckWithParents(user, p => p.CanUseMasterToolValue))
                );
        }
    }
}