using System;
using System.Collections.Generic;

namespace Mandara.Business.Calculators
{
    public class HoursInMonthCalculator
    {
        private static Dictionary<string, TimeZoneInfo> timeZoneInfo = new Dictionary<string, TimeZoneInfo>();
        private const string TimeZoneId = "W. Europe Standard Time";
        private static readonly TimeZoneInfo _timeZone;

        static HoursInMonthCalculator()
        {
            _timeZone = GetTimeZoneInfo(TimeZoneId);
        }

        private static TimeZoneInfo GetTimeZoneInfo(string timeZoneId)
        {
            TimeZoneInfo timeZone;

            if (timeZoneInfo.TryGetValue(timeZoneId, out timeZone))
            {
                return timeZone;
            }

            timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            if (timeZone == null)
            {
                string message = string.Format("Cannot find time zone with id '{0}'", timeZoneId);

                throw new ApplicationException(message);
            }

            timeZoneInfo.Add(timeZoneId, timeZone);
            return timeZone;
        }

        public static decimal HoursInMonth(int year, int month)
        {
            return HoursInMonth(year, month, _timeZone);
        }

        public static decimal HoursInMonth(int year, int month, string timeZoneId)
        {
            TimeZoneInfo timeZone = GetTimeZoneInfo(timeZoneId);

            return HoursInMonth(year, month, timeZone);
        }

        public static decimal HoursInMonth(int year, int month, TimeZoneInfo timeZone)
        {
            DateTime periodStart = new DateTime(year, month, 1);
            DateTime periodEnd = periodStart.AddDays(DateTime.DaysInMonth(year, month));
            TimeSpan period = periodEnd - periodStart;

            bool isDaylightSavingTimePeriodStart = timeZone.IsDaylightSavingTime(periodStart);
            bool isDaylightSavingTimePeriodEnd = timeZone.IsDaylightSavingTime(periodEnd);

            if (isDaylightSavingTimePeriodStart != isDaylightSavingTimePeriodEnd)
            {
                // define daylight saving time delta
                TimeSpan delta = TimeSpan.FromHours(1);

                foreach (TimeZoneInfo.AdjustmentRule rule in timeZone.GetAdjustmentRules())
                {
                    if (rule.DateStart <= periodStart && periodEnd <= rule.DateEnd)
                    {
                        delta = rule.DaylightDelta;
                        break;
                    }
                }

                if (isDaylightSavingTimePeriodStart)
                {
                    period = period + delta;
                }
                else
                {
                    period = period - delta;
                }
            }

            return Convert.ToDecimal(period.TotalHours);
        }
    }
}