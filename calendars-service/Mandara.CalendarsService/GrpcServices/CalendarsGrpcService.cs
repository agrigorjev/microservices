using Grpc.Core;
using Mandara.CalendarsService.GrpcDefinitions;
using Mandara.CalendarsService.Services;
using Mandara.CalendarsService.Data;
using static Mandara.CalendarsService.GrpcDefinitions.PortfolioService;
using Mandara.CalendarsService.DataConverters;

namespace Mandara.CalendarsService.GrpcServices;

public class CalendarsGrpcService : PortfolioServiceBase
{
    private readonly ICalendarsStorage _portfolioCache;
    private readonly DataConverters.PortfolioDataConverter portfolioConverter = new DataConverters.PortfolioDataConverter();

    public CalendarsGrpcService(ICalendarsStorage portfolioCache)
    {
        _portfolioCache = portfolioCache;
    }

    public override Task<AllPortfoliosResponseGrpcMessage> GetAllPortfolios(AllPortfoliosRequestGrpcMessage request, ServerCallContext context)
    {
        AllPortfoliosResponseGrpcMessage responseMessage = new();
        List<Portfolio> portfolios = _portfolioCache.GetPortfolios().ToList();


        portfolios.ForEach(portfolio => responseMessage.Portfolios.Add(portfolioConverter.Convert(portfolio)));

        return Task.FromResult(responseMessage);
    }

    public override Task<PortfolioResponseGrpcMessage> GetPortfolio(PortfolioRequestGrpcMessage request, ServerCallContext context)
    {
        PortfolioResponseGrpcMessage responseMessage = new();

        var portfolio = _portfolioCache.GetPortfolio(request.PortfolioId);

        responseMessage.PortfolioData = portfolioConverter.Convert(portfolio.ValueOr(Portfolio.Default));

        return Task.FromResult(responseMessage);
    }
}