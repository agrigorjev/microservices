using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Mandara.Entities;

namespace Mandara.Import
{
    public class InstrumentParser
    {
        //Expecting names in the form
        // "04 NOV 10 Product alias"
        // "NOV 10 Product alias"
        // "Cal 10 Product alias"
        // "Q1 10 Product alias"

        // Need to implement 
        // 4 NOV 10 Product alias
        // NOV 10 Product alias D5
        private static readonly Regex InstrumentSplitter = new Regex(@"(?<maturityTag1>(\d)?\d\s)?(?<maturityTag2>(?<dType>\w\w(\w)?)\s(?<year>\d\d))\s(?<productAlias>.*)(?<maturityTag3>\b\w+$)?");
        private static readonly Regex DaySplitter = new Regex(@"(?<dayTag3>\s?D\d(\d)?)$");
        private static readonly string[] MONTHES = new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

        public static Tuple<string, DateTime, ProductDateType> SplitDescription(string instrumentDescription)
        {
            string dType;
            string productName;
            DateTime date;
            int day;
            int year;

            var match = MakeMatch(instrumentDescription, out dType, out productName, out date, out day, out year);

            if (match.Success)
            {
                var type = ParseDate(instrumentDescription, match, day, dType, ref productName, ref date);

                return new Tuple<string, DateTime, ProductDateType>(productName, date, type);
            }

            throw new FormatException("Could not parse " + instrumentDescription + " to instrument.");
        }

        private static Match MakeMatch(string instrumentDescription, out string dType, out string productName, out DateTime date,
                                       out int day, out int year)
        {
            dType = string.Empty;
            productName = string.Empty;
            date = new DateTime(2000, 1, 1);
            day = 1;
            year = 2000;

            var match = InstrumentSplitter.Match(instrumentDescription);

            if (match.Success)
            {
                productName = match.Groups["productAlias"].Value;
                dType = match.Groups["dType"].Value.Trim().ToUpper();
                year = Convert.ToInt32(match.Groups["year"].Value);
                date = date.AddYears(year);
            }

            return match;
        }

        private static ProductDateType ParseDate(string instrumentDescription, Match match, int day, string dType,
                                                 ref string productName, ref DateTime date)
        {
            ProductDateType type = ProductDateType.MonthYear;
            if (MONTHES.Contains(dType))
            {
                int m = MONTHES.ToList().IndexOf(dType);
                date = date.AddMonths(m);
            }
            else
            {
                switch (dType)
                {
                    case "CAL":
                        type = ProductDateType.Year;
                        break;
                    case "Q1":
                        type = ProductDateType.Quarter;
                        break;
                    case "Q2":
                        date = date.AddMonths(3);
                        type = ProductDateType.Quarter;
                        break;
                    case "Q3":
                        date = date.AddMonths(6);
                        type = ProductDateType.Quarter;
                        break;
                    case "Q4":
                        date = date.AddMonths(9);
                        type = ProductDateType.Quarter;
                        break;
                    default:
                        throw new FormatException("Could not parse " + instrumentDescription + " to instrument.");
                }
            }

            if (match.Groups["maturityTag1"].Success)
            {
                day = Convert.ToInt32(match.Groups["maturityTag1"].Value.Trim());
                type = ProductDateType.Day;
            }
            else
            {
                var match2 = DaySplitter.Match(instrumentDescription);
                if (match2.Success)
                {
                    day = Convert.ToInt32(match2.Value.Trim(new char[] {' ', 'D'}));
                    type = ProductDateType.Day;
                    productName = productName.Replace(match2.Value, String.Empty);
                }
            }
            date = date.AddDays(day - 1);
            return type;
        }

        public static Tuple<string, string, DateTime, ProductDateType> SplitDescriptionWithStripName(string instrumentDescription)
        {
            string dType;
            string productName;
            DateTime date;
            int day;
            int year;

            var match = MakeMatch(instrumentDescription, out dType, out productName, out date, out day, out year);

            if (match.Success)
            {
                var type = ParseDate(instrumentDescription, match, day, dType, ref productName, ref date);

                return new Tuple<string, string, DateTime, ProductDateType>(productName, dType + year, date, type);
            }

            throw new FormatException("Could not parse " + instrumentDescription + " to instrument.");
        }
    }
}
