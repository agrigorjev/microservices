using Grpc.Core;
using Mandara.DataService.GrpcDefinitions;
using Mandara.DataService.Services;
using Mandara.DataService.Data;
using static Mandara.DataService.GrpcDefinitions.PortfolioService;
using Mandara.DataService.DataConverters;

namespace Mandara.DataService.GrpcServices;

public class PortfoliosGrpcService : PortfolioServiceBase
{
    private readonly IPortfoliosStorage _portfolioCache;
    private readonly DataConverters.PortfolioDataConverter portfolioConverter = new DataConverters.PortfolioDataConverter();

    public PortfoliosGrpcService(IPortfoliosStorage portfolioCache)
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