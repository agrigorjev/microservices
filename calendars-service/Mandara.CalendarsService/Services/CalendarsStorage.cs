using System.Collections.Concurrent;
using Optional;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Mandara.CalendarsService.Data;
using Mandara.CalendarsService.Configuration;

namespace Mandara.CalendarsService.Services;

public class CalendarsStorage : ICalendarsStorage
{
    private ConcurrentDictionary<int, Portfolio> Portfolios { get; set; } = new();

    private List<Portfolio> _portfolios = new();

    private readonly DataStoragesSettings _serviceSettings;
    private readonly IDbContextFactory<MandaraEntities> _contextFactory;

    public CalendarsStorage(IOptions<DataStoragesSettings> serviceSettings,
        IDbContextFactory<MandaraEntities> contextFactory)
    {
        _serviceSettings = serviceSettings.Value;
        _contextFactory = contextFactory;
    }

    public void Update()
    {
        using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
        {
            _portfolios = GetPortfolioQuery(productsDb).ToList();
        }

        Portfolio? errorBook = _portfolios.FirstOrDefault(portfolio => portfolio.PortfolioId == _serviceSettings.ErrorBookId);

        if (errorBook != null)
        {
            errorBook.IsErrorBook = true;
        }

        Portfolios =
            new ConcurrentDictionary<int, Portfolio>(_portfolios.ToDictionary(portfolio => portfolio.PortfolioId));
    }

    private static IQueryable<Portfolio> GetPortfolioQuery(MandaraEntities cxt)
    {
        return cxt.Portfolios.Include(x => x.ParentPortfolio).Include(x => x.Portfolios);
    }

    public Option<Portfolio> GetPortfolio(int portfolioId)
    {
        if (Portfolio.NoPortfolio == portfolioId)
        {
            return Option.None<Portfolio>();
        }

        if (Portfolios.TryGetValue(portfolioId, out Portfolio? portfolio))
        {
            if (portfolio == null)
                return Option.None<Portfolio>();

            return Option.Some(portfolio);
        }

        portfolio = LoadPortfolio(portfolioId);

        return portfolio.IsDefault() ? Option.None<Portfolio>() : Option.Some(portfolio);
    }

    public IList<Portfolio> GetPortfolios()
    {
        return _portfolios;
    }

    private Portfolio LoadPortfolio(int portfolioId)
    {
        using var productsDb = _contextFactory.CreateDbContext();

        Portfolio portfolio =
            GetPortfolioQuery(productsDb).FirstOrDefault(pfolio => pfolio.PortfolioId == portfolioId)
            ?? Portfolio.Default;

        if (!portfolio.IsDefault())
        {
            if (portfolio.PortfolioId == _serviceSettings.ErrorBookId)
                portfolio.IsErrorBook = true;

            Portfolios.TryAdd(portfolio.PortfolioId, portfolio);
        }

        return portfolio;
    }
}