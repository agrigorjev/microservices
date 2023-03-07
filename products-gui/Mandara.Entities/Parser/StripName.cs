using System;
using Mandara.Date;

namespace Mandara.Entities.Parser
{
    public static class StripName
    {
        private const string MonthlyStripNameFormat = "MMMyy";

        public static string MonthlyStripFromDate(DateTime date)
        {
            return date.ToString(MonthlyStripNameFormat);
        }

        public static DateTime DateFromMonthlyStripName(string stripName)
        {
            return DateTime.ParseExact(stripName, MonthlyStripNameFormat, null);
        }

        public static string Get(ProductType product, ProductDateType dateType, DateTime productDate)
        {
            if (product == ProductType.Balmo)
            {
                return "Bal Month";
            }

            switch (dateType)
            {
                case ProductDateType.Custom:
                {
                    return "Custom Monthly";
                }

                case ProductDateType.Quarter:
                {
                    int quarter = Quarter.GetQuarterNumber(productDate);

                    return $"Q{quarter} {productDate:yy}";
                }

                case ProductDateType.Year:
                {
                    return $"Cal {productDate:yy}";
                }

                default:
                {
                    return string.Format("{0:MMM}{0:yy}", productDate);
                }
            }
        }
    }
}
