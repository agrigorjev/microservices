using Mandara.Entities;
using Mandara.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Date;

namespace Mandara.Business.Calculators
{
    public class TradeCalculator
    {
        public static List<int> GetTradePeriodsRemaining(TradeCapture trade, DateTime riskDate)
        {
            List<int> tradePeriodsRemaining = new List<int>();
            List<DateTime> monthsRemaining = GetTradeMonthsRemaining(trade, riskDate);

            switch (trade.SecurityDefinition.Product.ContractSizeMultiplier)
            {
                case ContractSizeMultiplier.Daily:
                    {
                        tradePeriodsRemaining = GetTradeDaysRemaining(monthsRemaining);
                    }
                    break;

                case ContractSizeMultiplier.Hourly:
                    {
                        tradePeriodsRemaining = GetTradeHoursRemaining(monthsRemaining);
                    }
                    break;

                default:
                    {
                        tradePeriodsRemaining.Add(monthsRemaining.Count);
                    }
                    break;
            }

            return tradePeriodsRemaining;
        }

        private static List<int> GetTradeHoursRemaining(List<DateTime> remainingMonths)
        {
            return remainingMonths
                .Select(x => (int)HoursInMonthCalculator.HoursInMonth(x.Year, x.Month))
                .ToList();
        }

        public static List<int> GetTradePeriodsRemaining(TradeCapture trade)
        {
            return GetTradePeriodsRemaining(trade, trade.Strip.Part1.StartDate);
        }

        public static List<DateTime> GetTradeMonthsRemaining(TradeCapture trade, DateTime riskDate)
        {
            List<DateTime> remainingMonths = new List<DateTime>();
            int numberOfMonthsRemaining = 1;
            DateTime startDate = trade.Strip.Part1.StartDate;
            DateTime lastMonth;

            switch (trade.Strip.Part1.DateType)
            {
                case ProductDateType.Quarter:
                    {
                        lastMonth = startDate.AddMonths(2);
                        numberOfMonthsRemaining = GetMonthsRemainingOnQuarter(startDate, riskDate);
                    }
                    break;

                case ProductDateType.Year:
                    {
                        lastMonth = startDate.AddMonths(11);
                        numberOfMonthsRemaining = GetMonthsRemainingOnCalendar(startDate, riskDate);
                    }
                    break;

                case ProductDateType.Custom:
                    {
                        DateTime? tradeStartDate = trade.TradeStartDate;
                        DateTime? tradeEndDate = trade.TradeEndDate;

                        if (tradeStartDate.HasValue && tradeEndDate.HasValue)
                        {
                            tradeStartDate = new DateTime(tradeStartDate.Value.Year, tradeStartDate.Value.Month, 1);
                            tradeEndDate = new DateTime(tradeEndDate.Value.Year, tradeEndDate.Value.Month, 1);
                            lastMonth = tradeEndDate.Value;

                            numberOfMonthsRemaining =
                                GetMonthsRemainingOnCustom(tradeStartDate.Value, tradeEndDate.Value, riskDate);
                        }
                        else
                        {
                            throw new ArgumentNullException(
                                String.Format(
                                    "Trade {0} on product with custom date type requires start and end dates",
                                    trade.TradeId
                                    ));
                        }
                    }
                    break;

                default:
                    {
                        lastMonth = startDate;
                    }
                    break;
            }

            for (int month = -1 * numberOfMonthsRemaining + 1; month <= 0; ++month)
            {
                DateTime currentMonth = lastMonth.AddMonths(month);

                remainingMonths.Add(currentMonth);
            }

            return remainingMonths;
        }

        public static List<DateTime> GetTradeMonthsRemaining(TradeCapture trade)
        {
            return GetTradeMonthsRemaining(trade, trade.Strip.Part1.StartDate);
        }

        private static int GetMonthsRemainingOnQuarter(DateTime startDate, DateTime riskDate)
        {
            int numberOfMonthsRemaining;

            if (startDate.Year == riskDate.Year)
            {
                numberOfMonthsRemaining = Math.Min(3, startDate.Month + 3 - riskDate.Month);

                if (numberOfMonthsRemaining < 0)
                {
                    numberOfMonthsRemaining = 0;
                }
            }
            else if (startDate.Year > riskDate.Year)
            {
                numberOfMonthsRemaining = 3;
            }
            else
            {
                numberOfMonthsRemaining = 0;
            }

            return numberOfMonthsRemaining;
        }

        private static int GetMonthsRemainingOnCalendar(DateTime startDate, DateTime riskDate)
        {
            int numberOfMonthsRemaining;

            if (startDate.Year == riskDate.Year)
            {
                numberOfMonthsRemaining = startDate.Month + 12 - riskDate.Month;
            }
            else if (startDate.Year > riskDate.Year)
            {
                numberOfMonthsRemaining = 12;
            }
            else
            {
                numberOfMonthsRemaining = 0;
            }

            return numberOfMonthsRemaining;
        }

        private static int GetMonthsRemainingOnCustom(DateTime tradeStartDate, DateTime tradeEndDate, DateTime riskDate)
        {
            int numberOfMonthsRemaining;

            if (riskDate <= tradeStartDate)
            {
                numberOfMonthsRemaining = tradeEndDate.MonthsSince(tradeStartDate) + 1;
            }
            else if ((tradeStartDate < riskDate) && (riskDate <= tradeEndDate))
            {
                numberOfMonthsRemaining = tradeEndDate.MonthsSince(riskDate) + 1;
            }
            else
            {
                numberOfMonthsRemaining = 0;
            }

            return numberOfMonthsRemaining;
        }

        private static List<int> GetTradeDaysRemaining(List<DateTime> remainingMonths)
        {
            return
                remainingMonths
                    .Select(currentMonth => DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month))
                    .ToList();
        }

        public static List<List<int>> GetTradeStripPeriods(TradeCapture trade, DateTime riskDate)
        {
            if (!trade.Strip.IsTimeSpread)
            {
                return new List<List<int>>() { GetTradePeriodsRemaining(trade, riskDate) };
            }

            List<List<int>> tradePeriodsRemaining = new List<List<int>>();
            int strip2Offset = GetTradeStripsMonthOffset(trade);
            List<DateTime> strip1MonthsRemaining = GetTradeMonthsRemaining(trade, riskDate);
            List<DateTime> strip2MonthsRemaining =
                strip1MonthsRemaining.Select(month => month.AddMonths(strip2Offset)).ToList();

            switch (trade.SecurityDefinition.Product.ContractSizeMultiplier)
            {
                case ContractSizeMultiplier.Daily:
                    {
                        tradePeriodsRemaining.Add(GetTradeDaysRemaining(strip1MonthsRemaining));
                        tradePeriodsRemaining.Add(GetTradeDaysRemaining(strip2MonthsRemaining));
                    }
                    break;

                case ContractSizeMultiplier.Hourly:
                    {
                        tradePeriodsRemaining.Add(GetTradeHoursRemaining(strip1MonthsRemaining));
                        tradePeriodsRemaining.Add(GetTradeHoursRemaining(strip2MonthsRemaining));
                    }
                    break;

                default:
                    {
                        List<int> strip1SingleMonthsRemaining = strip1MonthsRemaining.Select(month => 1).ToList();
                        List<int> strip2SingleMonthsRemaining = strip2MonthsRemaining.Select(month => 1).ToList();

                        tradePeriodsRemaining.Add(strip1SingleMonthsRemaining);
                        tradePeriodsRemaining.Add(strip2SingleMonthsRemaining);
                    }
                    break;
            }

            return tradePeriodsRemaining;
        }

        private static int GetTradeStripsMonthOffset(TradeCapture trade)
        {
            if (trade.Strip.Part1.StartDate.Year == trade.Strip.Part2.StartDate.Year)
            {
                return trade.Strip.Part2.StartDate.Month - trade.Strip.Part1.StartDate.Month;
            }
            else
            {
                return trade.Strip.Part2.StartDate.Month + (12 - trade.Strip.Part1.StartDate.Month);
            }
        }
    }
}
