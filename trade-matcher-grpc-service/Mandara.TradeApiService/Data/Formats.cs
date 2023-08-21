namespace Mandara.TradeApiService.Data
{
    public static class Formats
    {
        public const string SortableShortDate = "yyyyMMdd";

        public const string DashSeparatedShortDate = "yyyy-MM-dd";

        public const string FullYear = "yyyy";

        public const string YearWithoutCentury = "yy";

        [Obsolete("Use YearWithoutCentury")]
        public const string YearWithoutCentuary = "yy";

        public const string SortableMonth = "MM";

        public const string MonthShortName = "MMM";

        public const string Month = "M";

        public const string SortableDay = "dd";

        public const string Day = "d";

        public const string TwentyFourHourTime = "HHmmss";

        public const string TwelveHourTime = "hhmmss";

        public const string TwentyFourHour = "HH";

        public const string TwelveHour = "hh";

        public const string Minutes = "mm";

        public const string Seconds = "ss";

        public const string Milliseconds = "fff";

        public static string DayFirstDateFormat()
        {
            return "ddMMyyyy";
        }

        public static string DayFirstDateFormat(char separator)
        {
            return string.Format("{0}{1}{2}{3}{4}", "dd", separator, "MM", separator, "yyyy");
        }

        public static string SortableShortDateFormat(char separator)
        {
            return string.Format("{0}{1}{2}{3}{4}", "yyyy", separator, "MM", separator, "dd");
        }

        public static string TwentyFourHourFormat(char separator)
        {
            return string.Format("{0}{1}{2}{3}{4}", "HH", separator, "mm", separator, "ss");
        }

        public static string TwelveHourFormat(char separator)
        {
            return string.Format("{0}{1}{2}{3}{4}", "hh", separator, "mm", separator, "ss");
        }

        public static string DateTimeFormat(char dateSeparator, char dateTimeSeparator, char timeSeparator)
        {
            return $"{SortableShortDateFormat(dateSeparator)}{dateTimeSeparator}{TwentyFourHourFormat(timeSeparator)}";
        }
    }

}
