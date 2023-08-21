using Google.Protobuf.WellKnownTypes;
using Mandara.CalendarsService.Data;
using Mandara.CalendarsService.GrpcDefinitions;

namespace Mandara.CalendarsService.DataConverters
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
