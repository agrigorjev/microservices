using System;
using Mandara.Date;
using Mandara.Date.Time;

namespace Mandara.Entities
{
    public struct RelativeStrip
    {
        public ProductDateType DateType { get; set; }
        public int DateIndex { get; set; }

        public DateTime AbsoluteDate
        {
            get
            {
                DateTime absoluteDate;

                DateTime now = SystemTime.Now();
                DateTime curMonth = new DateTime(now.Year, now.Month, 1);

                switch (DateType)
                {
                    case ProductDateType.Quarter:
                    {
                        DateTime advanceToQuarter = curMonth.AddMonths(DateIndex * 3);

                        absoluteDate = advanceToQuarter.FirstDayOfQuarter();
                    }
                    break;

                    case ProductDateType.Year:
                    {
                        DateTime advanceToYear = curMonth.AddMonths(DateIndex * 12);

                        absoluteDate = advanceToYear.FirstDayOfYear();
                    }
                    break;

                    default:
                    {
                        absoluteDate = curMonth.AddMonths(DateIndex);
                    }
                    break;
                }

                return absoluteDate;
            }
        }

        public string ToAbsoluteStrip()
        {
            string strip;

            DateTime absoluteDate = AbsoluteDate;

            switch (DateType)
            {
                case ProductDateType.Quarter:
                {
                    int quarter = Quarter.GetQuarterNumber(absoluteDate);

                    strip = $"q{quarter} {absoluteDate.ToString(Formats.YearWithoutCentuary)}";
                }
                break;

                case ProductDateType.Year:
                {
                    strip = $"cal{absoluteDate.ToString(Formats.YearWithoutCentuary)}";
                }
                break;

                default:
                {
                    strip = absoluteDate.ToString("MMMyy");
                }
                break;
            }

            return Char.ToUpper(strip[0]) + strip.Substring(1);
        }

        public override string ToString()
        {
            string dateTypeString = "m";

            switch (DateType)
            {
                case ProductDateType.Year:
                dateTypeString = "cal";
                break;

                case ProductDateType.Quarter:
                dateTypeString = "q";
                break;
            }

            return string.Format("{0}{1}", dateTypeString, DateIndex);
        }
    }
}