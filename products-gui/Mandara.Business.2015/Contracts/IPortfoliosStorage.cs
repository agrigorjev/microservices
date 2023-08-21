using Mandara.Entities;
using Mandara.Extensions.Option;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Optional;

namespace Mandara.Business.Contracts
{
    public interface IPortfoliosStorage
    {
        ConcurrentDictionary<int, Portfolio> Portfolios { get; }
        void Update();
        IList<Portfolio> GetPortfolios();
        TryGetResult<Portfolio> GetFirstRootPortfolio();
        TryGetResult<Portfolio> GetPortfolioById(int portfolioId);
        Option<Portfolio> GetPortfolio(int portfolioId);
        TryGetResult<Portfolio> GetPortfolioByName(string portfolioName);
        TryGetResult<Portfolio> GetPortfolioByAccount(string exchangeAccount);
    }
}