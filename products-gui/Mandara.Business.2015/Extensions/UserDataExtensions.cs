using Mandara.Business.Authorization;
using Mandara.Business.Bus.Messages;
using Mandara.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Extensions
{
    public static class UserDataExtensions
    {
        public static UserData AssignFrom(this UserData userData, User user)
        {
            userData.FirstName = user.FirstName;
            userData.LastName = user.LastName;
            userData.Locked = user.LockedValue;
            userData.UserId = user.UserId;
            userData.UserName = user.UserName;
            userData.ForcePasswordChange = user.ForcePasswordChange;

            if (user.Groups != null)
                userData.Groups =
                    user.Groups.Select(
                        am => new GroupData { GroupId = am.GroupId, GroupName = am.GroupName }).ToList
                        ();

            if (user.Portfolio != null)
                userData.Portfolio = new PortfolioData
                {
                    Name = user.Portfolio.Name,
                    PortfolioId = user.Portfolio.PortfolioId
                };

            if (user.PortfolioPermissions != null)
                userData.PortfolioPermissions =
                    user.PortfolioPermissions.Select(
                        am =>
                            new PortfolioPermissionsData
                            {
                                PortfolioId = am.PortfolioId,
                                CanAddEditTrades = am.CheckWithParents(user, p => p.CanAddEditTradesValue),
                                CanUseMasterTool = am.CheckWithParents(user, p => p.CanUseMasterToolValue),
                                CanViewPnl = am.CheckWithParents(user, p => p.CanViewPnlValue),
                                CanViewRisk = am.CheckWithParents(user, p => p.CanViewRiskValue),
                                CanAddEditBooks = am.CheckWithParents(user, p => p.CanAddEditBooksValue),
                                CanAmendBrokerage = am.CheckWithParents(user, p => p.CanAmendBrokerageValue),
                            }).ToList();

            IEnumerable<Permission> userPriv =
                AuthorizationService.AuthorizationManager.GetUserPermissions(userData.UserName);
            userData.Permissions.AddRange(userPriv.Select(am => am.PermissionId).ToList());

            return userData;
        }
    }
}
