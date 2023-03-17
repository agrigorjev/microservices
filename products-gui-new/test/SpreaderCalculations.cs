using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mandara.Business.Bus.Messages.Spreader;
using Mandara.Business.Spreader;
using Mandara.Extensions.Collections;

namespace Mandara.RiskMgmtTool.Spreader.MonthlySpreader
{
    public static class SpreaderCalculations
    {
        public static List<SpreaderOutput> GetSpreadsWithSimulatedPositions(
            List<SpreaderInput> positions,
            List<MonthlySpread> spreadsConfiguration,
            int exitMonth)
        {
            // Check if there are ever missing months...  and then prohibit that.  Either positions or
            // spreadsConfiguration must be required to hold the complete collection of months
            List<SpreaderOutput> spreads = GetBaseSpreadsFromPositions(positions, spreadsConfiguration);

            return SpreadsTotals.CalculateSpreadAndTotalSpread(
                spreads,
                SpreadsTotals.GetSimulatePositionsAction(true),
                exitMonth);
        }

        private static List<SpreaderOutput> GetBaseSpreadsFromPositions(
            List<SpreaderInput> positions,
            List<MonthlySpread> spreadsConfiguration)
        {
            // Check if there are ever missing months...  and then prohibit that.  Either positions or
            // spreadsConfiguration must be required to hold the complete collection of months
            return positions.Select(pos => GetSpreaderCalculationConfiguration(pos, GetSpreadConfig(pos.Month)))
                            .ToList();

            MonthlySpread GetSpreadConfig(DateTime positionDate)
            {
                return spreadsConfiguration.FirstOrDefault(spread => spread.Month.DateValue == positionDate);
            }
        }

        public static List<SpreaderOutput> GetSpreads(
            List<SpreaderInput> positions,
            List<MonthlySpread> spreadsConfiguration,
            int exitMonthIndex)
        {
            List<SpreaderOutput> spreads = GetBaseSpreadsFromPositions(positions, spreadsConfiguration);

            return SpreadsTotals.CalculateSpreadAndTotalSpread(
                spreads,
                SpreadsTotals.GetSimulatePositionsAction(false),
                exitMonthIndex
            );
        }

        private static SpreaderOutput GetSpreaderCalculationConfiguration(SpreaderInput position, MonthlySpread config)
        {
            return new SpreaderOutput()
            {
                Month = position.Month,
                BookFuturesEquivalent = position.FuturesAmount ?? 0M,
                FuturesEquivalent = config?.FuturesEquivalent ?? 0M
            };
        }

        /// <summary>
        /// Moved from Spreader form (RecalculateCells)
        /// </summary>
        public static void UpdateSpreads(List<SpreaderOutput> calculatedSpreads, ICollection<MonthlySpread> spreadTargets)
        {
            Dictionary<DateTime, SpreaderOutput>
                spreadsByMonth = calculatedSpreads.ToDictionary(spread => spread.Month);

            spreadTargets.ForEach(
                (spreadTarget) =>
                {
                    if (!spreadsByMonth.TryGetValue(spreadTarget.Month.DateValue, out SpreaderOutput spread))
                    {
                        return;
                    }

                    UpdateSpreads(spread, spreadTarget);
                });

            void UpdateSpreads(SpreaderOutput newSpread, MonthlySpread targetSpread)
            {
                targetSpread.JALSpreads = newSpread.JALSpreads;
                targetSpread.TotalJALSpreads = newSpread.TotalJALSpreads;
                targetSpread.BookFuturesEquivalent = newSpread.BookFuturesEquivalent;
            }
        }

        public static void UpdateFuturesEquivalent(List<SpreaderOutput> calculatedSpreads, BindingList<MonthlySpread> spreadTargets)
        {
            Dictionary<DateTime, SpreaderOutput>
                spreadsByMonth = calculatedSpreads.ToDictionary(spread => spread.Month);

            spreadTargets.ForEach(
                (spreadTarget) =>
                {
                    if (!spreadsByMonth.TryGetValue(spreadTarget.Month.DateValue, out SpreaderOutput spread))
                    {
                        return;
                    }

                    UpdateFuturesEquivalent(spread, spreadTarget);
                });

            void UpdateFuturesEquivalent(SpreaderOutput newSpread, MonthlySpread targetSpread)
            {
                targetSpread.FuturesEquivalent = newSpread.FuturesEquivalent;
            }
        }



    }
}
