using Google.Protobuf.WellKnownTypes;
using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService.Data;
using Mandara.TradeApiService.GrpcDefinitions;

namespace Mandara.TradeApiService.DataConverters;

public class TradeTemplateDataConverter : IDataConverter<TradeTemplate, TradeTemplateGrpc>
{
    public TradeTemplateGrpc Convert(TradeTemplate data)
    {
        StockCalendarConverter scConverter = new StockCalendarConverter();
        UnitDataConverter unitConverter = new UnitDataConverter();
        ExchangeDataConverter exchangeDataConverter = new ExchangeDataConverter();
        OfficialProductDataConverter offproductDataConverter = new OfficialProductDataConverter();
        PortfolioDataConverter portfolioDataConverter = new PortfolioDataConverter();


        TradeTemplateGrpc converted = new();
        converted.ExchangeId = data.ExchangeId;
        converted.Exchange = exchangeDataConverter.Convert(data.Exchange);
        converted.OfficialProduct = offproductDataConverter.Convert(data.OfficialProduct);
        converted.OfficialProductId = data.OfficialProductId;
        converted.Portfolio = portfolioDataConverter.Convert(data.Portfolio);
        converted.PortfolioId = data.PortfolioId;
        converted.TemplateName = data.TemplateName;
        converted.TradeTemplateId = data.TradeTemplateId;
        converted.Unit = unitConverter.Convert(data.Unit);
        converted.UnitId = data.UnitId; 
        converted.Volume = data.Volume;

        return converted;
    }
}
