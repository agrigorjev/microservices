using System;

namespace Mandara.Entities.Exceptions
{
    public class CalendarNotFoundException : ApplicationException
    {
        private readonly StockCalendar _calendar;

        public CalendarNotFoundException(SourceDetail sourceDetail, StockCalendar calendar) : base(string.Format("Calendar not found, name [{0}], id [{1}]", calendar.Name, calendar.CalendarId.ToString()))
        {
            _calendar = calendar;
        }
    }
}