using System.Collections.Generic;
using Mandara.Entities;

namespace Mandara.Business.Managers.Calendars
{
    public class ExpiriesSavedEventArgs
    {
        public UpdatedCalendars Changes { get; }

        public ExpiriesSavedEventArgs(List<StockCalendar> added, List<int> removed, List<StockCalendar> modified)
        {
            Changes = new UpdatedCalendars(added, removed, modified);
        }
    }
}
