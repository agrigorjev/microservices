using System.Globalization;

namespace Mandara.TradeApiService.Data
{
    public static class DateTimeExtensions
    {
        private const string CentralStandardTime = "Central Standard Time";

        public static string ToShortDateString(this DateTime? optionalDate)
        {
            if (!optionalDate.HasValue)
            {
                return string.Empty;
            }

            return optionalDate.Value.ToShortDateString();
        }

        public static bool IsWeekendDay(this DateTime date)
        {
            if (date.DayOfWeek != DayOfWeek.Saturday)
            {
                return date.DayOfWeek == DayOfWeek.Sunday;
            }

            return true;
        }

        [Obsolete("Use PreviousBusinessDay")]
        public static DateTime GetPreviousBusinessDay(this DateTime date)
        {
            return date.PreviousBusinessDay();
        }

        public static DateTime PreviousBusinessDay(this DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday => date.AddDays(-3.0),
                DayOfWeek.Sunday => date.AddDays(-2.0),
                _ => date.AddDays(-1.0),
            };
        }

        [Obsolete("Use NextBusinessDay")]
        public static DateTime GetNextBusinessDay(this DateTime date)
        {
            return date.NextBusinessDay();
        }

        public static DateTime NextBusinessDay(this DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Friday => date.AddDays(3.0),
                DayOfWeek.Saturday => date.AddDays(2.0),
                _ => date.AddDays(1.0),
            };
        }

        public static DateTime FromLocalTimeToCst(this DateTime date)
        {
            return date.FromLocalTime("Central Standard Time");
        }

        public static DateTime FromCstToLocalTime(this DateTime date)
        {
            return date.ToLocalTime("Central Standard Time");
        }

        public static DateTime FromLocalTime(this DateTime date, string targetTimeZone)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, TimeZoneInfo.Local.Id, targetTimeZone);
        }

        public static DateTime ToLocalTime(this DateTime date, string sourceTimeZone)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, sourceTimeZone, TimeZoneInfo.Local.Id);
        }

        [Obsolete("Use BeginningOfNextDay")]
        public static DateTime GetBeginningOfNextDay(this DateTime date)
        {
            return date.BeginningOfNextDay();
        }

        public static DateTime BeginningOfNextDay(this DateTime date)
        {
            if (!date.Equals(DateTime.MaxValue))
            {
                return date.Date.AddDays(1.0);
            }

            return date;
        }

        [Obsolete("Use EndOfDay")]
        public static DateTime GetEndOfDay(this DateTime date)
        {
            return date.EndOfDay();
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return date.BeginningOfNextDay().AddMilliseconds(-1.0);
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int num = dt.DayOfWeek - startOfWeek;
            if (num < 0)
            {
                num += 7;
            }

            return dt.AddDays(-1 * num).Date;
        }

        public static DateTime LastWeekDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).PreviousBusinessDay();
        }

        [Obsolete("Use LastWeekDayOfMonth")]
        public static DateTime LastDayOfMonth(this DateTime date)
        {
            return date.LastWeekDayOfMonth();
        }

        public static DateTime FinalDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1.0);
        }

        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime FirstDayOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        public static DateTime AsUnspecified(this DateTime toCopy)
        {
            if (DateTimeKind.Local != toCopy.Kind)
            {
                return DateTime.SpecifyKind(toCopy, DateTimeKind.Unspecified);
            }

            return DateTime.SpecifyKind(toCopy.ToUtc(), DateTimeKind.Unspecified);
        }

        private static DateTime ToUtc(this DateTime local)
        {
            return local.ToUniversalTime();
        }

        public static DateTime AsUtc(this DateTime toCopy)
        {
            if (DateTimeKind.Local != toCopy.Kind)
            {
                return DateTime.SpecifyKind(toCopy, DateTimeKind.Utc);
            }

            return DateTime.SpecifyKind(toCopy.ToUtc(), DateTimeKind.Utc);
        }

        public static DateTime WithDay(this DateTime toModify, int dayOfMonth)
        {
            return new DateTime(toModify.Year, toModify.Month, dayOfMonth);
        }

        public static string ToSortableShortDate(this DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }

        public static string ToSortableShortDate(this DateTime date, char separator)
        {
            return date.ToString(string.Format("{0}{1}{2}{3}{4}", "yyyy", separator, "MM", separator, "dd"));
        }

        public static string ToSortableShortDateTime(this DateTime date)
        {
            return date.ToString("yyyyMMddHHmmss");
        }

        public static string ToSortableShortDateTime(this DateTime date, char separator)
        {
            return date.ToString(string.Format("{0}{1}{2}", "yyyyMMdd", separator, "HHmmss"));
        }

        public static string ToDayFirstString(this DateTime date)
        {
            return date.ToString("dMyyyy");
        }

        public static string ToDayFirstString(this DateTime date, char separator)
        {
            return date.ToString(string.Format("{0}{1}{2}{3}{4}", "d", separator, "M", separator, "yyyy"));
        }

        public static string To24HoursMinutesSeconds(this DateTime date)
        {
            return date.ToString("HHmmss");
        }

        public static string To24HourTime(this DateTime date)
        {
            return date.To24HourTime(':');
        }

        public static string To24HourTime(this DateTime date, char separator)
        {
            return date.ToString(string.Format("{0}{1}{2}{3}{4}", "HH", separator, "mm", separator, "ss"));
        }

        public static string ToShortDateAndTime(this DateTime date)
        {
            return date.ToShortDateAndTime('-', ' ', ':');
        }

        public static string ToShortDateAndTime(this DateTime date, char dateSeparator, char dateTimeSeparator, char timeSeparator)
        {
            return $"{date.ToSortableShortDate(dateSeparator)}{dateTimeSeparator}{date.To24HourTime(timeSeparator)}";
        }

        public static string ToDayFirstDateAndTime(this DateTime date)
        {
            return date.ToDayFirstDateAndTime('-', ' ', ':');
        }

        public static string ToDayFirstDateAndTime(this DateTime date, char dateSeparator, char dateTimeSeparator, char timeSeparator)
        {
            return $"{date.ToSortableShortDate(dateSeparator)}{dateTimeSeparator}{date.To24HourTime(timeSeparator)}";
        }

        public static int MonthsSince(this DateTime to, DateTime from)
        {
            return (to.Year - from.Year) * 12 + (to.Month - from.Month);
        }
    }
}
