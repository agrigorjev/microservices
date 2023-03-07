using System.Collections.Generic;
using System.Linq;
using Mandara.Entities;
using Mandara.Entities.DatabaseContext;
using Optional;

namespace Mandara.Business.Services.Users
{
    public class UserRepository: IUsersRepository
    {
        private readonly ProductsContextCreator _productsDb;
        private readonly string _dbCallerId;

        public UserRepository(string connectionString, string callerId = nameof(UserRepository))
        {
            _productsDb = new ProductsContextCreator(connectionString);
            _dbCallerId = callerId;
        }

        public Option<User> TryGetUser(string name)
        {
            if (name == null)
            {
                return Option.None<User>();
            }

            using (MandaraEntities productsDb = _productsDb.ProductsDb(_dbCallerId))
            {
                User withName = productsDb.Users.Include("UserAliases")
                                 .Include("Portfolio")
                                 .Include("PortfolioPermissions")
                                 .Include("PortfolioPermissions.Portfolio")
                                 .Include("Groups")
                                 .Include("ProductGroupPortfolios")
                                 .Include("ProductGroupPortfolios.ProductCategory")
                                 .SingleOrDefault(user => user.UserName == name);

                return null != withName ? Option.Some(withName) : Option.None<User>();
            }
        }

        public Option<User> TryGetUser(int id)
        {
            if (id <= 0)
            {
                return Option.None<User>();
            }

            using (MandaraEntities productsDb = _productsDb.ProductsDb(_dbCallerId))
            {
                User withId = productsDb.Users.Include("UserAliases")
                                        .Include("Portfolio")
                                        .Include("PortfolioPermissions")
                                        .Include("PortfolioPermissions.Portfolio")
                                        .Include("Groups")
                                        .Include("ProductGroupPortfolios")
                                        .Include("ProductGroupPortfolios.ProductCategory")
                                        .SingleOrDefault(user => user.UserId == id);

                return null != withId ? Option.Some(withId) : Option.None<User>();
            }
        }

        public ICollection<User> GetUsers()
        {
            using (MandaraEntities productsDb = _productsDb.ProductsDb(_dbCallerId))
            {
                return productsDb.Users.Include("Groups")
                                 .Include("UserAliases")
                                 .Include("Portfolio")
                                 .Include("PortfolioPermissions")
                                 .ToList();
            }
        }

        public void SaveUser(User toSave)
        {
            throw new System.NotImplementedException();
        }
    }
}