using System.Collections.Generic;
using Mandara.Entities;

namespace Mandara.Business.Managers.Calendars
{
    public class CalendarChanges<T>
    {
        public List<StockCalendar> ModifiedCalendars { get; }
        public List<T> AddedCalendars { get; }
        public List<int> DeletedCalendars { get; }

        public CalendarChanges(List<StockCalendar> modified, List<T> added, List<int> deleted)
        {
            ModifiedCalendars = modified;
            AddedCalendars = added;
            DeletedCalendars = deleted;
        }
    }
}
