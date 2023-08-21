using System;
using System.Collections.Generic;
using Mandara.Entities;

namespace Mandara.Business.Managers.Calendars
{
    public struct NewExpiryCalendar
    {
        public string Name { get; }
        public int? DateCorrection { get; }
        public TimeZoneInfo CalendarTimeZone { get; }
        public List<CalendarExpiryDate> ExpiryDates { get; }

        public NewExpiryCalendar(string name, int? dateCorr, TimeZoneInfo timeZone, List<CalendarExpiryDate> dates)
        {
            Name = name;
            DateCorrection = dateCorr;
            CalendarTimeZone = timeZone;
            ExpiryDates = dates;
        }

        public Tuple<string, int?, TimeZoneInfo, List<CalendarExpiryDate>> ToTuple()
        {
            return new Tuple<string, int?, TimeZoneInfo, List<CalendarExpiryDate>>(
                Name,
                DateCorrection,
                CalendarTimeZone,
                ExpiryDates);
        }
    }
}
