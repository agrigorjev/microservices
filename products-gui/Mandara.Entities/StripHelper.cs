using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Mandara.Date;
using Mandara.Entities.EntitiesCustomization;

namespace Mandara.Entities
{
    public class ParsedStrip : IEquatable<ParsedStrip>
    {
        public ProductDateType Type { get; }
        public DateTime Date { get; }
        public string CustomStripName { get; }

        public static readonly ProductDateType DefaultType = ProductDateType.MonthYear;
        public static readonly DateTime DefaultDate = DateTime.MinValue;

        public static ParsedStrip Default => new ParsedStrip(DefaultType, DefaultDate);

        public ParsedStrip(ProductDateType type, DateTime stripDate, string customName)
        {
            Type = type;
            Date = stripDate;
            CustomStripName = customName;
        }

        public ParsedStrip(ProductDateType type, DateTime stripDate) : this(type, stripDate, string.Empty)
        {
        }

        public bool IsDefault()
        {
            return (Type == DefaultType) && Date == DefaultDate;
        }

        public bool HasCustomStripName => !String.IsNullOrWhiteSpace(CustomStripName);

        public Tuple<int?, DateTime, string> ToTuple()
        {
            return ((int?)Type, Date, CustomStripName).ToTuple();
        }

        public Tuple<DateTime, ProductDateType> ToTypeAndDateTuple()
        {
            return (Date, Type).ToTuple();
        }

        public DateTime? GetDate()
        {
            if (IsDefault()) return null;
            return Date;
        }

        public override bool Equals(object obj) => Equals(obj as Strip);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Date.GetHashCode();
                hashCode = (hashCode * 397) ^ Type.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(ParsedStrip other)
        {
            return other != null && Date == other.Date && Type == other.Type;
        }

    }

    public static class StripHelper
    {
        private static readonly Dictionary<string, int> MonthAbbreviations = new Dictionary<string, int>()
        {  
            {"JAN", 0},
            {"FEB", 1},
            {"MAR", 2},
            {"APR", 3},
            {"MAY", 4},
            {"JUN", 5},
            {"JUL", 6},
            {"AUG", 7},
            {"SEP", 8},
            {"OCT", 9},
            {"NOV", 10},
            {"DEC", 11},
        };

        public const int MonthsInAYear = 12;
        private static readonly DateTime baseDate = new DateTime(2000, 1, 1);
        private const string stripPeriodName = "period";
        private const string stripYearName = "year";

        private static readonly Regex monthYearFormat =
            new Regex($@"(?<{stripPeriodName}>\w\w(\w)?)\s?(?<{stripYearName}>\d\d)$");

        private const string stripDayName = "day";

        private static readonly Regex iceBalmoMonthYearFormat = new Regex(
            $@"^(?<{stripDayName}>[0-9]{{1,2}})\s?(?<{stripPeriodName}>\w\w(\w)?)\s?(?<{stripYearName}>\d\d)$");

        public const string StandardBalmoName = "Bal Month";
        public const string DailyBalmoName = "Bal Month-ND";
        public const string CustomPeriodBalmoName = "Custom BalMonth";

        public static readonly string StripNameBalmoVariants =
            ConfigurationManager.AppSettings["StripName_AtParseDate"]
            ?? $"{StandardBalmoName}|{DailyBalmoName}|{CustomPeriodBalmoName}";

        public static readonly string StripNameCustomVariants =
            ConfigurationManager.AppSettings["StripName_CustomDateType"] ?? "Custom Monthly";

        public static readonly string StripNameDailyVariants =
            ConfigurationManager.AppSettings["StripName_DailyDateType"] ?? "Custom CFD|Custom CFD (Month)";

        public static CultureInfo UkCulture = CultureInfo.GetCultureInfo("en-GB");
        public const string BalmoNewIceDateFormat = "dd MMM yy";

        public static string GetBalmoStripNameWithDate(string stripName, DateTime? tradeStartDate)
        {
            if (tradeStartDate == null)
            {
                return stripName;
            }

            return StripNameBalmoVariants.Contains(stripName)
                ? $"{stripName}_{tradeStartDate.Value.ToDayFirstString('/')}"
                : stripName;
        }

        public static Tuple<DateTime, ProductDateType> ParseStripDate(
            string dateString,
            DateTime startDate,
            DateTime? transactTime = null)
        {
            Match match = monthYearFormat.Match(dateString);

            return match.Success
                ? GetFullMonthStripDate(dateString, match).ToTypeAndDateTuple()
                : TryGetCustomPeriodStripDate(dateString, startDate, transactTime).ToTypeAndDateTuple();
        }

        public static ParsedStrip ParseStrip(string stripDate, DateTime startDate, DateTime transactTime)
        {
            Match match = monthYearFormat.Match(stripDate);

            if (match.Success)
            {
                return GetFullMonthStripDate(stripDate, match);
            }

            return TryGetCustomPeriodStripDate(stripDate, startDate, transactTime);
        }

        private static ParsedStrip GetFullMonthStripDate(string strip, Match match)
        {
            string period = match.Groups[stripPeriodName].Value.Trim().ToUpper();
            int year = Convert.ToInt32(match.Groups[stripYearName].Value);
            DateTime stripDate = baseDate.AddYears(year);
            ProductDateType dateType = ProductDateType.MonthYear;

            if (MonthAbbreviations.TryGetValue(period, out int month))
            {
                return new ParsedStrip(dateType, stripDate.AddMonths(month));
            }

            return GetMultiMonthStripDate(strip, period, stripDate);
        }

        private static ParsedStrip GetMultiMonthStripDate(
            string strip,
            string period,
            DateTime stripDate)
        {
            if (period.StartsWith("CAL"))
            {
                return new ParsedStrip(ProductDateType.Year, stripDate);
            }

            if (period.StartsWith("Q"))
            {
                return new ParsedStrip(
                    ProductDateType.Quarter,
                    stripDate.AddMonths((int.Parse(period[1].ToString()) - 1) * 3));
            }

            throw new FormatException("Cannot recognize '" + strip + "' as a valid for conversion to a product date.");
        }

        private static ParsedStrip TryGetCustomPeriodStripDate(
            string dateString,
            DateTime startDate,
            DateTime? transactTime)
        {
            Match customBalmo = iceBalmoMonthYearFormat.Match(dateString);

            if (customBalmo.Success)
            {
                DateTime stripDate = baseDate.AddYears(Int32.Parse(customBalmo.Groups[stripYearName].Value))
                                             .AddMonths(GetMonthNumberFromAbbr(customBalmo))
                                             .AddDays(Int32.Parse(customBalmo.Groups[stripDayName].Value));

                return new ParsedStrip(ProductDateType.MonthYear, stripDate, CustomPeriodBalmoName);
            }

            return TryGetStripDateFromCustomNameVariants(dateString, startDate, transactTime);
        }

        private static int GetMonthNumberFromAbbr(Match customBalmo)
        {
            return MonthAbbreviations[customBalmo.Groups[stripPeriodName].Value.ToUpper(CultureInfo.InvariantCulture)];
        }

        private static ParsedStrip TryGetStripDateFromCustomNameVariants(
            string dateString,
            DateTime startDate,
            DateTime? transactTime)
        {
            List<string> balmoVariants = StripNameBalmoVariants.Split('|').ToList();

            if (balmoVariants.Contains(dateString))
            {
                DateTime stripDate = DateTime.MinValue == startDate ? transactTime.Value.Date : startDate;

                return new ParsedStrip(ProductDateType.Day, stripDate);
            }

            List<string> customVariants = StripNameCustomVariants.Split('|').ToList();

            if (customVariants.Contains(dateString))
            {
                return new ParsedStrip(ProductDateType.Custom, startDate);
            }

            List<string> dailyVariants = StripNameDailyVariants.Split('|').ToList();

            if (dailyVariants.Contains(dateString))
            {
                return new ParsedStrip(ProductDateType.Daily, startDate);
            }

            string errorMsg = string.Format("Strip is not recognized: {0}", dateString);

            throw new FormatException(errorMsg);
        }

        public static DateTime? ParseDate(object objDateString, string format)
        {
            if (objDateString == null || objDateString is DBNull)
            {
                return null;
            }

            if (objDateString is DateTime)
            {
                return (DateTime)objDateString;
            }

            string dateString = (string)objDateString;

            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }

            var culture = CultureInfo.InvariantCulture;

            if (DateTime.TryParseExact(dateString, format, culture.DateTimeFormat, DateTimeStyles.None, out DateTime datetime))
            {
                return datetime;
            }

            return null;
        }

        public static bool IsBalmo(string stripName)
        {
            return StripNameBalmoVariants.Contains(stripName) || IsIceNewBalmoFormat(stripName);
        }

        public static bool IsIceNewBalmoFormat(string stripName)
        {
            return DateTime.TryParseExact(stripName, BalmoNewIceDateFormat, UkCulture, DateTimeStyles.None, out _);
        }

    }
}