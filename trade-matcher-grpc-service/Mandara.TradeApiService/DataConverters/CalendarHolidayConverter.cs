using Google.Protobuf.WellKnownTypes;
using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService.Data;
using Mandara.TradeApiService.GrpcDefinitions;

namespace Mandara.TradeApiService.DataConverters
{
    public class CalendarHolidayConverter : IDataConverter<CalendarHoliday, HolidayGrpc>
    {
        public HolidayGrpc Convert(CalendarHoliday data)
        {
            HolidayGrpc converted = new();
            converted.CalendarId= data.CalendarId;
            converted.HolidayDate = Timestamp.FromDateTime(data.HolidayDate.ensureUtc());
            return converted;
        }
    }
}
