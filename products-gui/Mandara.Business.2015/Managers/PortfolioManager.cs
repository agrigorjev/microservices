using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.ErrorReporting;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Mandara.Business
{
    public class PortfolioManager : Client.Managers.PortfolioManager
    {
        public static List<Portfolio> GetOnlyPortfolios()
        {
            using (var cxt = CreateMandaraProductsDbContext())
            {
                DbQuery<Portfolio> portfolioQuery = cxt.Portfolios.Include("ParentPortfolio").Include("Portfolios");
                List<Portfolio> portfolios = portfolioQuery.ToList();

                return portfolios;
            }
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(PortfolioManager));
        }

        public void AddPortfolio(Portfolio portfolio, string userName)
        {
            using (var cxt = CreateMandaraProductsDbContext())
            {
                if (portfolio.ParentPortfolio != null)
                {
                    portfolio.ParentPortfolio =
                        cxt.Portfolios.FirstOrDefault(x => x.PortfolioId == portfolio.ParentPortfolio.PortfolioId);

                    List<UserPortfolioPermission> permissions = cxt.UserPortfolioPermissions.Where(p => p.PortfolioId == portfolio.ParentPortfolioId).ToList();
                    permissions.ForEach(p =>
                    {
                        UserPortfolioPermission pp = new UserPortfolioPermission()
                        {
                            CanAddEditBooks = p.CanAddEditBooks,
                            CanAddEditTrades = p.CanAddEditTrades,
                            CanUseMasterTool = p.CanUseMasterTool,
                            CanViewPnl = p.CanViewPnl,
                            CanViewRisk = p.CanViewRisk,
                            UserId = p.UserId,
                        };
                        portfolio.UserPortfolioPermissions.Add(pp);
                        cxt.UserPortfolioPermissions.Add(pp);
                    });
                }

                var timestamp = SystemTime.Now();

                portfolio.CreatedAt = timestamp;
                portfolio.CreatedBy = userName;
                portfolio.UpdatedAt = timestamp;
                portfolio.UpdatedBy = userName;

                cxt.Portfolios.Add(portfolio);

                cxt.SaveChanges();
            }
        }

        public void UpdatePortfolio(Portfolio portfolio, string userName)
        {
            using (var cxt = CreateMandaraProductsDbContext())
            {
                Portfolio updatedPortfolio =
                    cxt.Portfolios.Include(x => x.ParentPortfolio).FirstOrDefault(x => x.PortfolioId == portfolio.PortfolioId);

                if (updatedPortfolio == null)
                {
                    ErrorReportingHelper.GlobalQueue.Enqueue(
                        new Error("Portfolio", ErrorType.DataError,
                                  string.Format("Updating portfolio not found in database, portfolio id [{0}]",
                                                portfolio.PortfolioId),
                                  portfolio.PortfolioId.ToString(), null, ErrorLevel.Critical));
                    return;
                }

                updatedPortfolio.Name = portfolio.Name;
                updatedPortfolio.IsArchived = portfolio.IsArchived;

                updatedPortfolio.UpdatedAt = SystemTime.Now();
                updatedPortfolio.UpdatedBy = userName;

                if (portfolio.ParentPortfolio != null)
                {
                    updatedPortfolio.ParentPortfolio =
                        cxt.Portfolios.FirstOrDefault(x => x.PortfolioId == portfolio.ParentPortfolio.PortfolioId);
                }

                cxt.SaveChanges();
            }
        }

        public bool DeletePortfolio(Portfolio portfolio)
        {
            using (var cxt = CreateMandaraProductsDbContext())
            {
                int numTradesInBook1 = cxt.PortfolioTrades.Count(pt => pt.PortfolioId == portfolio.PortfolioId);
                int numTradesInBook2 = cxt.TradeCaptures.Count(tc => tc.Portfolio != null && tc.Portfolio.PortfolioId == portfolio.PortfolioId);
                int numBooksInBook = cxt.Portfolios.Count(pt => pt.ParentPortfolio != null && pt.ParentPortfolio.PortfolioId == portfolio.PortfolioId);

                Portfolio portfolioToDelete =
                    cxt.Portfolios
                        .FirstOrDefault(x => x.PortfolioId == portfolio.PortfolioId);

                if (portfolioToDelete != null)
                {
                    if (numTradesInBook1 > 0 || numTradesInBook2 > 0 || numBooksInBook > 0)
                        return false;

                    List<UserPortfolioPermission> userPortfolioPermissions =
                        cxt.UserPortfolioPermissions.Where(x => x.PortfolioId == portfolioToDelete.PortfolioId).ToList();
                    userPortfolioPermissions.ForEach(upp => cxt.UserPortfolioPermissions.Remove(upp));

                    cxt.Portfolios.Remove(portfolioToDelete);

                    cxt.SaveChanges();
                }
            }

            return true;
        }

    }
}
