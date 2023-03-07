using Mandara.Entities;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mandara.Business.Calculators
{
    public class StartDateFromStripNameCalculator
    {
        private static int Century = 2000;

        private static readonly Dictionary<string, int> MonthSymbols = new Dictionary<string, int>()
        {
            {"F", 1 },
            {"G", 2 },
            {"H", 3 },
            {"J", 4 },
            {"K", 5 },
            {"M", 6 },
            {"N", 7 },
            {"Q", 8 },
            {"U", 9 },
            {"V", 10 },
            {"X", 11 },
            {"Z", 12 }
        };

        private const string Month = "month";
        private const string QuarterNumberName = "quarter";
        private const string YearNumberName = "year";
        private const string CalStripDuration = "Cal";
        private const string QuarterStripDuration = "Q";

        private static readonly string YearNumberPattern = $"(?<{YearNumberName}>[0-9]{{2}})";

        private static readonly string MonthDurationPattern =
            $"(?<{Month}>{String.Join("|", MonthSymbols.Keys)}){YearNumberPattern}";

        private static readonly string CalDurationPattern =
            $"(?<{CalStripDuration}>{CalStripDuration}{YearNumberPattern})";

        private static readonly string QuarterDurationPattern =
            $"(?<{QuarterStripDuration}>{QuarterStripDuration})(?<{QuarterNumberName}>[1-4]){YearNumberPattern}";

        private static readonly Regex FullStripPattern =
            new Regex($"^({CalDurationPattern}|{MonthDurationPattern}|{QuarterDurationPattern})$");

        private static readonly Regex CalPattern = new Regex($"^{CalDurationPattern}$");

        private static readonly Regex QuarterPattern = new Regex($"^{QuarterDurationPattern}$");

        public static TryGetResult<DateTime> StripToStartDate(string strip)
        {
            if (strip == null)
            {
                return new TryGetVal<DateTime>((val) => true);
            }

            DateTime startDate = DateTime.MinValue;
            Match stripMatch = FullStripPattern.Match(strip);

            if (stripMatch.Success)
            {
                int year = Century + Int32.Parse(stripMatch.Groups[YearNumberName].Value);

                if (stripMatch.Groups[Month].Success)
                {
                    string monthName = stripMatch.Groups[Month].Value;
                    int month = MonthSymbols[monthName];

                    startDate = ProductDateHelper.DateFromMonth(year, month);
                }
                else if (stripMatch.Groups[QuarterStripDuration].Success)
                {
                    int quarter = Int32.Parse(stripMatch.Groups[QuarterNumberName].Value);

                    startDate = ProductDateHelper.DateFromQuarter(year, quarter);

                }
                else if (stripMatch.Groups[CalStripDuration].Success)
                {
                    startDate = ProductDateHelper.DateFromYear(year);
                }
                else
                {
                    throw new ArgumentException(
                        $"{strip} is not a valid strip name.  Expected <month symbol><year>, Q<quarter number><year>, "
                            + "or Cal<year>.");
                }
            }

            return new TryGetVal<DateTime>(startDate, (date) => DateTime.MinValue.Equals(date));
        }

        public static bool IsQOrCal(string strip)
        {
            return IsQuarterStrip(strip) || IsCalStrip(strip);
        }

        public static bool IsQuarterStrip(string strip)
        {
            if (string.IsNullOrEmpty(strip))
            {
                return false;
            }

            return QuarterPattern.Match(strip).Success;
        }

        public static bool IsCalStrip(string strip)
        {
            if (string.IsNullOrEmpty(strip))
            {
                return false;
            }

            return CalPattern.Match(strip).Success;
        }
    }
}
