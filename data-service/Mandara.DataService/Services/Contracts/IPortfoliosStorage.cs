using Mandara.DataService.Data;
using Optional;

namespace Mandara.DataService.Services;

public interface IPortfoliosStorage
{
    void Update();
    Option<Portfolio> GetPortfolio(int portfolioId);
    IList<Portfolio> GetPortfolios();
}