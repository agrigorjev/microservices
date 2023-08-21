using Google.Protobuf.WellKnownTypes;
using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService.Data;
using Mandara.TradeApiService.GrpcDefinitions;


namespace Mandara.TradeApiService.DataConverters
{
    public class CalendarExpiryDateConverter : IDataConverter<CalendarExpiryDate, ExpiryDatesGrpc>
    {
        public ExpiryDatesGrpc Convert(CalendarExpiryDate data)
        {
  
            ExpiryDatesGrpc convertedDate = new();
            convertedDate.ExpiryDate = Timestamp.FromDateTime(data.ExpiryDate.ensureUtc());
            convertedDate.FutureDate = Timestamp.FromDateTime(data.FuturesDate.ensureUtc());
            convertedDate.CalendarId = data.CalendarId;
            return convertedDate;
          }
    }
}
