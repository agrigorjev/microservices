using Mandara.CalendarsService.Data;
using Optional;

namespace Mandara.CalendarsService.Services;

public interface ICalendarsStorage
{
    void Update();
    Option<Portfolio> GetPortfolio(int portfolioId);
    IList<Portfolio> GetPortfolios();
}