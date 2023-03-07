using System;
using Mandara.Date;

namespace Mandara.Business.Client.Extensions
{
    // TODO: Move to Mandara.Date
    public static class TimeSpanZoneExtensions
    {
        public static TimeSpan ToUtc(this TimeSpan localTime, DateTime localDay)
        {
            return DateTime.SpecifyKind(localDay, DateTimeKind.Local).Add(localTime).AsUtc().TimeOfDay;
        }

        public static TimeSpan ToLocal(this TimeSpan utcTime, DateTime utcDay)
        {
            return DateTime.SpecifyKind(utcDay, DateTimeKind.Utc).Add(utcTime).ToLocalTime().TimeOfDay;
        }
    }
}