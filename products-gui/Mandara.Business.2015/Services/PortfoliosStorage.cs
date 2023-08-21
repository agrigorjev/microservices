using System;
using Mandara.Business.Contracts;
using Mandara.Entities;
using Mandara.Extensions.Option;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using Mandara.Entities.Entities;
using Optional;

namespace Mandara.Business.Services
{
    public class PortfoliosStorage : IPortfoliosStorage
    {
        [Obsolete("This will be made private.  Access portfolios using accessor functions.")]
        public ConcurrentDictionary<int, Portfolio> Portfolios { get; set; } =
            new ConcurrentDictionary<int, Portfolio>();

        private List<Portfolio> _portfolios = new List<Portfolio>();

        [Obsolete("This will be made private.  Access exchange accounts using accessor functions.")]
        public ConcurrentDictionary<string, Portfolio> PortfolioExchangeAccounts { get; private set; } =
            new ConcurrentDictionary<string, Portfolio>();

        private readonly int _errorBookId;

        public PortfoliosStorage()
        {
            int.TryParse(ConfigurationManager.AppSettings["ErrorBookId"], out _errorBookId);
        }

        public void Update()
        {
            List<ExchangeAccount> exchangeAccounts;

            using (MandaraEntities productsDb = CreateMandaraProductsDbContext())
            {
                _portfolios = GetPortfolioQuery(productsDb).ToList();
                exchangeAccounts = GetExchangeAccountQuery(productsDb).ToList();
            }

            Portfolio errorBook = _portfolios.FirstOrDefault(portfolio => portfolio.PortfolioId == _errorBookId);

            if (errorBook != null)
            {
                errorBook.IsErrorBook = true;
            }

            Portfolios =
                new ConcurrentDictionary<int, Portfolio>(_portfolios.ToDictionary(portfolio => portfolio.PortfolioId));
            PortfolioExchangeAccounts = new ConcurrentDictionary<string, Portfolio>(
                exchangeAccounts.ToDictionary(acc => acc.AccountName, acc => acc.Portfolio));
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(PortfoliosStorage));
        }

        private IQueryable<Portfolio> GetPortfolioQuery(MandaraEntities cxt)
        {
            return cxt.Portfolios.Include(x => x.ParentPortfolio).Include(x => x.Portfolios);
        }

        public IList<Portfolio> GetPortfolios()
        {
            return _portfolios;
        }

        private IQueryable<ExchangeAccount> GetExchangeAccountQuery(MandaraEntities cxt)
        {
            return cxt.ExchangeAccounts.Include(x => x.Portfolio);
        }

        public TryGetResult<Portfolio> GetFirstRootPortfolio()
        {
            return new TryGetRef<Portfolio>()
            {
                Value = Portfolios.Values.FirstOrDefault(portfolio => PortfolioManager.IsRootPortfolio(portfolio))
            };
        }

        public TryGetResult<Portfolio> GetPortfolioById(int portfolioId)
        {
            return new TryGetRef<Portfolio>((portfolio) => portfolio.IsDefault())
            {
                Value = GetPortfolio(portfolioId).ValueOr(Portfolio.Default)
            };
        }

        public Option<Portfolio> GetPortfolio(int portfolioId)
        {
            if (Portfolio.NoPortfolio == portfolioId)
            {
                return Option.None<Portfolio>();
            }

            if (Portfolios.TryGetValue(portfolioId, out Portfolio portfolio))
            {
                return Option.Some(portfolio);
            }

            portfolio = LoadPortfolio(portfolioId);

            return portfolio.IsDefault() ? Option.None<Portfolio>() : Option.Some(portfolio);
        }

        private Portfolio LoadPortfolio(int portfolioId)
        {
            using (var productsDb = CreateMandaraProductsDbContext())
            {
                Portfolio portfolio =
                    GetPortfolioQuery(productsDb).FirstOrDefault(pfolio => pfolio.PortfolioId == portfolioId)
                    ?? Portfolio.Default;

                if (!portfolio.IsDefault())
                {
                    Portfolios.TryAdd(portfolio.PortfolioId, portfolio);
                }

                return portfolio;
            }
        }

        public TryGetResult<Portfolio> GetPortfolioByName(string portfolioName)
        {
            Portfolio portfolio = Portfolios.Values.FirstOrDefault(ptfolio => portfolioName == ptfolio.Name);

            if (null == portfolio)
            {
                using (var cxt = CreateMandaraProductsDbContext())
                {
                    portfolio = GetPortfolioQuery(cxt)
                        .FirstOrDefault(pfolio => pfolio.Name == portfolioName);

                    if (null != portfolio)
                    {
                        Portfolios.TryAdd(portfolio.PortfolioId, portfolio);
                    }
                }
            }

            return new TryGetRef<Portfolio>() { Value = portfolio };
        }

        public TryGetResult<Portfolio> GetPortfolioByAccount(string exchangeAccount)
        {
            Portfolio portfolio=null;
            if (!PortfolioExchangeAccounts.TryGetValue(exchangeAccount,out portfolio))
            {
                using (var cxt = CreateMandaraProductsDbContext())
                {
                    var mapping = GetExchangeAccountQuery(cxt)
                        .FirstOrDefault(x => x.AccountName == exchangeAccount);
                    if (mapping != null)
                    {
                        PortfolioExchangeAccounts.TryAdd(mapping.AccountName, mapping.Portfolio);
                        portfolio = mapping.Portfolio;
                    }
                }
            }

            return new TryGetRef<Portfolio>() { Value = portfolio };

        }
    }
}