using System.Collections.Generic;
using Mandara.Entities;

namespace Mandara.Business.Managers.Calendars
{
    public class HolidaysSavedEventArgs
    {
        public UpdatedCalendars Changes { get; }

        public HolidaysSavedEventArgs(List<StockCalendar> added, List<int> removed, List<StockCalendar> modified)
        {
            Changes = new UpdatedCalendars(added, removed, modified);
        }
    }
}
