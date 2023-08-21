using Mandara.Business.OldCode;
using Mandara.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mandara.Business.2015Tests")]
namespace Mandara.Business.Calculators.Daily
{
    internal class SplitTradeAtExpiry
    {
        public decimal FullTradeQuantity { get; }
        public decimal FullCalculationQuantity { get; }
        public List<DateTime> OriginalBusinessDays { get; }
        public IEnumerable<DateTime> DaysBeforeExpiry { get; }
        public IEnumerable<DateTime> DaysFromExpiry { get; }
        private readonly decimal _percentageBeforeExpiry;

        public SplitTradeAtExpiry(SourceDetail trade, CalculationContext context, DateTime inRangeExpiry)
        {
            (DaysBeforeExpiry, DaysFromExpiry) = SplitDaysAt(inRangeExpiry, trade.BusinessDays1);
            FullTradeQuantity = trade.TradeCapture.Quantity.Value;
            FullCalculationQuantity = context.Quantity;
            OriginalBusinessDays = trade.BusinessDays1;
            _percentageBeforeExpiry = DaysBeforeExpiry.Count() / (decimal)trade.BusinessDays1.Count;
        }

        private static Tuple<List<DateTime>, List<DateTime>> SplitDaysAt(
            DateTime splitAt,
            List<DateTime> daysToSplit)
        {
            if (splitAt > daysToSplit.Last())
            {
                return new Tuple<List<DateTime>, List<DateTime>>(daysToSplit, new List<DateTime>());
            }

            if (splitAt < daysToSplit.First())
            {
                return new Tuple<List<DateTime>, List<DateTime>>(new List<DateTime>(), daysToSplit);
            }

            int splitIndex = daysToSplit.IndexOf(splitAt);

            return new Tuple<List<DateTime>, List<DateTime>>(
                daysToSplit.Take(splitIndex).ToList(),
                daysToSplit.Skip(splitIndex).ToList());
        }

        public decimal TradeQuantityBeforeExpiry => FullTradeQuantity * _percentageBeforeExpiry;
        public decimal TradeQuantityFromExpiry => FullTradeQuantity * (1 - _percentageBeforeExpiry);
        public decimal CalculationQuantityBeforeExpiry => FullCalculationQuantity * _percentageBeforeExpiry;
        public decimal CalculationQuantityFromExpiry => FullCalculationQuantity * (1 - _percentageBeforeExpiry);

        public DateTime EndDateTradeBeforeExpiry => DaysBeforeExpiry.Any()
            ? DaysBeforeExpiry.Last()
            : OriginalBusinessDays.First().AddDays(-1);
        public bool HasDatesAfterExpiry => DaysFromExpiry.Any();
        public DateTime StartDateTradeFromExpiry => DaysFromExpiry.First();
        public DateTime EndDateTradeAfterExpiry => DaysFromExpiry.Last();

    }
}
