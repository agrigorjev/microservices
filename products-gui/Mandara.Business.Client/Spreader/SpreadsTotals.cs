using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mandara.Business.Bus.Messages.Spreader;

namespace Mandara.Business.Spreader
{
    public class SpreadsTotals
    {
        public const int NoExitMonth = -1;

        private struct NextSpread
        {
            public SpreaderOutput Next { get; }
            public decimal NextJalSpread { get; }
            public decimal NextTotalJalSpread { get; }

            public NextSpread(SpreaderOutput next, decimal nextJal, decimal nextTotalJal)
            {
                Next = next;
                NextJalSpread = nextJal;
                NextTotalJalSpread = nextTotalJal;
            }
        }

        private struct JALSpreadValues
        {
            public decimal JALSpread { get; }
            public decimal TotalJALSpread { get; }

            public JALSpreadValues(decimal jalSpread, decimal totalJalSpread)
            {
                JALSpread = jalSpread;
                TotalJALSpread = totalJalSpread;
            }
        }

        private static readonly Func<SpreaderOutput, decimal, SpreaderOutput> IncludeSimulatedPositions =
            (spreaderRow, futuresEqVal) =>
            {
                SpreaderOutput spread = spreaderRow.Clone();

                spread.TotalJALSpreads += futuresEqVal;
                return spread;
            };

        private static readonly Func<SpreaderOutput, decimal, SpreaderOutput> NoSimulatedPositions =
            (spreaderRow, futuresEqVal) => spreaderRow.Clone();

        public static Func<SpreaderOutput, decimal, SpreaderOutput> GetSimulatePositionsAction(
            bool includeSimulatedPositions)
        {
            return includeSimulatedPositions ? IncludeSimulatedPositions : NoSimulatedPositions;
        }

        // It's not clear that the H and AA columns were actually for these values.
        public static List<SpreaderOutput> CalculateSpreadAndTotalSpread(
            List<SpreaderOutput> spreaderResults,
            Func<SpreaderOutput, decimal, SpreaderOutput> includeSimulatedPositions,
            int exitMonthIndex)
        {
            SpreaderDiagnostics("input", spreaderResults);

            List<SpreaderOutput> spreads =  IsExitMonthValid()
                ? CalculateSpreadsAroundExit()
                : CalculateSpreadsBeforeExit(spreaderResults.Count);

            SpreaderDiagnostics("output", spreads);
            return spreads;

            bool IsExitMonthValid()
            {
                return 0 < exitMonthIndex && exitMonthIndex < spreaderResults.Count;
            }

            List<SpreaderOutput> CalculateSpreadsAroundExit()
            {
                return CalculateSpreadsBeforeExit(exitMonthIndex)
                       .Concat(Reverse(CalculateSpreadsFromExit())).ToList();
            }

            List<SpreaderOutput> CalculateSpreadsBeforeExit(int exitMonth)
            {
                return CalculateSpreadAndTotalSpread(
                    spreaderResults.Take(exitMonth).ToList(),
                    GetNextSpreadValuesForward(includeSimulatedPositions));
            }

            List<SpreaderOutput> CalculateSpreadsFromExit()
            {
                List<SpreaderOutput> reverseSpreads = Reverse(spreaderResults.Skip(exitMonthIndex).ToList());

                return CalculateSpreadAndTotalSpread(
                    reverseSpreads.ToList(),
                    GetNextSpreadReverse(includeSimulatedPositions));
            }

            void SpreaderDiagnostics(string spreadDataState, List<SpreaderOutput> spreadResults)
            {
                Debug.WriteLine($"SpreadsTotal.CalculateSpreadAndTotalSpread, {spreadDataState}:");
                spreadResults.ForEach(spread =>
                Debug.WriteLine($"{spread.Month:dd-MM-yy} "
                                + $"book eq: {spread.BookFuturesEquivalent,12:F3} "
                                + $"fut eq: {spread.FuturesEquivalent,12:F3} "
                                + $"jal:{spread.JALSpreads,12:F3} "
                                + $"total jal:{spread.TotalJALSpreads,12:F3}"));
            }
        }

        private static Func<SpreaderOutput, JALSpreadValues, NextSpread>
            GetNextSpreadValuesForward(Func<SpreaderOutput, decimal, SpreaderOutput> includeSimulatedPositions)
        {
            return (spreaderConfig, prevFuturesEqValues) => NextSpreadForward(
                spreaderConfig,
                prevFuturesEqValues,
                includeSimulatedPositions);
        }

        private static NextSpread NextSpreadForward(
            SpreaderOutput manualPositions,
            JALSpreadValues prevFuturesEqValues,
            Func<SpreaderOutput, decimal, SpreaderOutput> includeSimulatedPositions)
        {
            decimal futuresEquivVal = GetFuturesEqValue(manualPositions.FuturesEquivalent);
            SpreaderOutput nextSpread = includeSimulatedPositions(
                CreateSpreaderResult(
                    manualPositions,
                    prevFuturesEqValues.JALSpread + futuresEquivVal,
                    prevFuturesEqValues.TotalJALSpread + manualPositions.BookFuturesEquivalent),
                futuresEquivVal);

            return new NextSpread(
                nextSpread,
                nextSpread.JALSpreads,
                nextSpread.TotalJALSpreads);
        }

        private static decimal GetFuturesEqValue(decimal? futuresEqVal)
        {
            return futuresEqVal ?? 0M;
        }

        private static SpreaderOutput CreateSpreaderResult(
            SpreaderOutput spread,
            decimal jalSpread,
            decimal totalJalSpread)
        {
            return new SpreaderOutput()
            {
                Month = spread.Month,
                FuturesEquivalent = spread.FuturesEquivalent,
                BookFuturesEquivalent = spread.BookFuturesEquivalent,
                JALSpreads = jalSpread,
                TotalJALSpreads = totalJalSpread
            };
        }

        private static JALSpreadValues DefaultInitialJalSpread => new JALSpreadValues(0M, 0M);

        private static List<SpreaderOutput> CalculateSpreadAndTotalSpread(
            List<SpreaderOutput> spreaderResults,
            Func<SpreaderOutput, JALSpreadValues, NextSpread> calculateNextSpreadPositions)
        {
            JALSpreadValues futuresEqValues = DefaultInitialJalSpread;

            List<SpreaderOutput> spreads = spreaderResults.Select(
                spreaderRow =>
                {
                    NextSpread spread = calculateNextSpreadPositions(
                        spreaderRow,
                        futuresEqValues);

                    futuresEqValues = new JALSpreadValues(spread.NextJalSpread, spread.NextTotalJalSpread);
                    return spread.Next;
                }).ToList();
            return spreads;
        }

        private static List<SpreaderOutput> Reverse(List<SpreaderOutput> spreads)
        {
            spreads.Reverse();
            return spreads;
        }


        private static Func<SpreaderOutput, JALSpreadValues, NextSpread>
            GetNextSpreadReverse(Func<SpreaderOutput, decimal, SpreaderOutput> includeSimulatedPositions)
        {
            return (spreaderConfig, prevFuturesEqValues) => NextSpreadReverse(
                spreaderConfig,
                prevFuturesEqValues,
                includeSimulatedPositions);
        }

        /// <summary>
        /// Remember that this is used in a calculation stepping from the end of a list toward the beginning.
        /// 
        /// When calculating in reverse, the new spread's JAL is the negative of the sum of all JALs up to this point,
        /// i.e. -(;&ltSum all positions from this month forward;&gt).  This is the same as saying
        /// for month N the JAL is
        /// prevFuturesEqVal = -((pos N+1) + (pos N+2) + ... + (pos M)) where M is the last month
        /// 
        /// This is the prevFuturesEqVal passed in.
        /// The futures equivalent value used for the next step is then the previous futures equivalent value minus the
        /// futures equivalent value of the current entry:
        /// -((pos N+1) + (pos N+2) + (pos N+3) + ... + (pos M)) - (pos N)
        ///
        /// The next total JAL spread is the next total including simulated positions (if set) less the next book
        /// futures equivalent.  This follows the same pattern as above.  This looks particularly different to the
        /// forward case because in the forward case the next spread's total has already been calculated as previous
        /// total + book value + simulated positions (if set).  Because of the fact that the flat price position in the
        /// current calculation row only affects the next calculation row, includeSimulatedPositions is not called to
        /// modify the current row, but only to calculate the total spread for the next row.
        /// </summary>
        /// <param name="prevFuturesEqVal"></param>
        /// <param name="prevBookFuturesEqVal"></param>
        /// <returns></returns>
        private static NextSpread NextSpreadReverse(
            SpreaderOutput manualPositions,
            JALSpreadValues prevFuturesEqValues,
            Func<SpreaderOutput, decimal, SpreaderOutput> includeSimulatedPositions)
        {
            decimal futuresEquivVal = GetFuturesEqValue(manualPositions.FuturesEquivalent);
            SpreaderOutput nextSpread = CreateSpreaderResult(
                manualPositions,
                prevFuturesEqValues.JALSpread,
                prevFuturesEqValues.TotalJALSpread);

            return new NextSpread(
                nextSpread,
                prevFuturesEqValues.JALSpread - futuresEquivVal,
                includeSimulatedPositions(nextSpread, -futuresEquivVal).TotalJALSpreads
                - nextSpread.BookFuturesEquivalent);
        }
    }
}
