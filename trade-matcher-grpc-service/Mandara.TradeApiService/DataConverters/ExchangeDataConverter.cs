using Google.Protobuf.WellKnownTypes;
using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService.Data;

namespace Mandara.TradeApiService.DataConverters;

public class ExchangeDataConverter : IDataConverter<Exchange, ExchangeGrpc>
{
    public ExchangeGrpc Convert(Exchange data)
    {
        StockCalendarConverter scConverter = new StockCalendarConverter();

        ExchangeGrpc converted = new();
        converted.CalendarId = data.CalendarId;
        converted.ExchangeId = data.ExchangeId;
        converted.MappingValue = data.MappingValue;
        converted.Name = data.Name;
        converted.TimezoneId = data.TimeZoneId;
        
        if (data.CalendarId.HasValue)
            converted.Calendar = scConverter.Convert(data.Calendar);

        return converted;
    }
}
