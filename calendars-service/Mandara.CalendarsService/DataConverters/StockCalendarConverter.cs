using Mandara.CalendarsService.Data;
using Mandara.CalendarsService.GrpcDefinitions;


namespace Mandara.CalendarsService.DataConverters
{
    public class StockCalendarConverter : IDataConverter<StockCalendar, StockCalendarGrpc>
    {
        public StockCalendarGrpc Convert(StockCalendar data)
        {
            
            CalendarHolidayConverter  calendarHolidayConverter= new CalendarHolidayConverter();
            CalendarExpiryDateConverter calendarExpiryDateConverter= new CalendarExpiryDateConverter();
            StockCalendarGrpc convertedCalendar = new();
            convertedCalendar.CalendarId= data.CalendarId;
            convertedCalendar.CalendarType = (int)data.CalendarType;
            convertedCalendar.CalendarTypeDb=data.CalendarTypeDb;
           convertedCalendar.Name= data.Name;
            convertedCalendar.Correction= data.Correction;
            convertedCalendar.Timezone=data.Timezone;
            data.Holidays.ToList().ForEach(d =>
            {
                convertedCalendar.Holidays.Add(calendarHolidayConverter.Convert(d));
            });
            data.FuturesExpiries.ToList().ForEach(d =>
            {
                convertedCalendar.FutureExpiries.Add(calendarExpiryDateConverter.Convert(d));
            });

            return  convertedCalendar;
          }
    }
}
