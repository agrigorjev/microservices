using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService.Data;
using Mandara.TradeApiService.GrpcDefinitions;


namespace Mandara.TradeApiService.DataConverters;

public class PortfolioDataConverter : IDataConverter<Portfolio, PortfolioGrpc>
{
    public PortfolioGrpc Convert(Portfolio data)
    {
        //if (data == null)
        //    return null;

        PortfolioGrpc convertedPortfolio = new();

        convertedPortfolio.PortfolioId = data.PortfolioId;
        convertedPortfolio.PortfolioId = data.PortfolioId;
        convertedPortfolio.IsErrorBook = data.IsErrorBook;
        convertedPortfolio.IsArchived = data.IsArchived;

        convertedPortfolio.Name = data.Name ?? string.Empty;

        return convertedPortfolio;
    }
}

