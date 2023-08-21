using System.Collections.Generic;

namespace Mandara.Entities
{
    public enum ProductType
    {
        Futures = 0,
        Swap = 1,
        FuturesBasedSwap = 2,
        Diff = 6,
        Balmo = 7,
        DailySwap = 8,
        DayVsMonthFullWeek = 9,
        Spot = 10,
        TradeMonthSwap = 11,
        DayVsMonthCustom = 12,
        DailyVsDaily = 13
    }

    public static class ProductTypeExtensions
    {
        public static bool IsDailyOrWeeklyDiff(this ProductType prodType)
        {
            return ProductType.DayVsMonthCustom == prodType
                   || ProductType.DayVsMonthFullWeek == prodType
                   || ProductType.DailyVsDaily == prodType;
        }

        public static bool IsDaily(this ProductType prodType)
        {
            return IsDailyOrWeeklyDiff(prodType) || ProductType.DailySwap == prodType;
        }

        public static bool IsDiff(this ProductType prodType)
        {
            return ProductType.Diff == prodType || IsDailyOrWeeklyDiff(prodType);
        }

        public static Dictionary<ProductType, string> GetNames()
        {
            return new Dictionary<ProductType, string>(Names);
        }

        private static readonly Dictionary<ProductType, string> Names = new Dictionary<ProductType, string>()
        {
            { ProductType.Futures, "Futures" },
            { ProductType.Swap, "Swap" },
            { ProductType.Spot, "Spot" },
            { ProductType.FuturesBasedSwap, "Futures based swap" },
            { ProductType.Diff, "Diff" },
            { ProductType.Balmo, "Balmo" },
            { ProductType.DailySwap, "Daily Swap" },
            { ProductType.DayVsMonthFullWeek, "Daily Diff (full week)" },
            { ProductType.DayVsMonthCustom, "Daily Diff (custom)" },
            { ProductType.DailyVsDaily, "Daily Diff (cross month)" },
            { ProductType.TradeMonthSwap, "Trade Month Swap" }
        };

        public static string Name(this ProductType prodType)
        {
            return Names[prodType];
        }
    }
}
