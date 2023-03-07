using System;
using System.Collections.Generic;
using Mandara.Entities;

namespace Mandara.Business.Managers.Calendars
{
    public struct NewHolidayCalendar
    {
        public string Name { get; }
        public List<CalendarHoliday> Holidays { get; }

        public NewHolidayCalendar(string name, List<CalendarHoliday> dates)
        {
            Name = name;
            Holidays = dates;
        }

        public Tuple<string, List<CalendarHoliday>> ToTuple()
        {
            return new Tuple<string, List<CalendarHoliday>>(Name, Holidays);
        }
    }
}
