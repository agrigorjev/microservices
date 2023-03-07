using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Entities;

namespace Mandara.Business.Managers.Calendars
{
    public struct UpdatedCalendars
    {
        public List<StockCalendar> Added { get; }
        public List<int> Removed { get; }
        public List<StockCalendar> Modified { get; }

        public UpdatedCalendars(List<StockCalendar> added, List<int> removed, List<StockCalendar> modified)
        {
            Added = added;
            Removed = removed;
            Modified = modified;
        }
    }
}
