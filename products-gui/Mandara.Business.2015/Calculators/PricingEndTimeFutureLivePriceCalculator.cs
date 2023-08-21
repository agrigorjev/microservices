using Mandara.Business.Extensions;
using Mandara.Business.Model;
using Mandara.Business.Services.Prices;
using Mandara.Entities;
using Mandara.Extensions.Option;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Calculators
{
    public class PricingEndTimeFutureLivePriceCalculator
    {
        private static readonly ConcurrentDictionary<Tuple<int, string>, LivePrice> PricingEndTradePriceStorage =
            new ConcurrentDictionary<Tuple<int, string>, LivePrice>();

        private static readonly ConcurrentDictionary<Tuple<int, DateTime>, TryGetResult<Money>>
            PricingEndPositionPriceStorage =
                new ConcurrentDictionary<Tuple<int, DateTime>, TryGetResult<Money>>();

        public static LivePrice GetFuturesLivePriceForPricingEndTime(
            TradeCapture trade,
            DateTime riskDate,
            IPricesProvider livePricesProvider,
            ISnapshotPricesProvider snapshotPricesProvider)
        {
            LivePrice livePrice = new LivePrice();
            Product product = trade.SecurityDefinition.Product;

            if (!product.PricingEndTime.HasValue)
            {
                return livePrice;
            }

            Tuple<int, string> key = new Tuple<int, string>(
                product.ProductId,
                trade.SecurityDefinition.StripName);

            if (!PricingEndTradePriceStorage.TryGetValue(key, out livePrice))
            {
                IEnumerable<TimeSpan> timestamps = livePricesProvider.TodayTimestamps;
                TryGetResult<TimeSpan> nearestTimestamp = TryGetNearestTimestamp(timestamps, product);

                if (nearestTimestamp.HasValue)
                {
                    UpdateSnapshotPricesIfNewSnapshotTime(riskDate, snapshotPricesProvider, nearestTimestamp);
                    livePrice = snapshotPricesProvider.GetSnapshotTradePrice(trade, livePricesProvider);
                    PricingEndTradePriceStorage.TryAdd(key, livePrice);
                }
            }
            return livePrice;
        }

        public static TryGetResult<Money> GetFuturesLivePriceForPricingEndTime(
            CalculationDetailModel detail,
            Product product,
            DateTime riskDate,
            IPricesProvider livePricesProvider,
            ISnapshotPricesProvider snapshotPricesProvider)
        {
            TryGetResult<Money> productPrice = new TryGetVal<Money>();

            if (!product.PricingEndTime.HasValue)
            {
                return productPrice;
            }

            Tuple<int, DateTime> key = new Tuple<int, DateTime>(product.ProductId, detail.CalculationDate);

            if (!PricingEndPositionPriceStorage.TryGetValue(key, out productPrice))
            {
                TryGetResult<TimeSpan> nearestTimestamp = TryGetNearestTimestamp(
                    livePricesProvider.TodayTimestamps,
                    product);

                if (nearestTimestamp.HasValue)
                {
                    UpdateSnapshotPricesIfNewSnapshotTime(riskDate, snapshotPricesProvider, nearestTimestamp);

                    productPrice = snapshotPricesProvider.TryGetProductPrice(
                        detail.ProductId,
                        detail.CalculationDate,
                        ProductDateType.MonthYear,
                        detail.MappingColumn,
                        detail.OfficialProductId);

                    PricingEndPositionPriceStorage.TryAdd(key, productPrice);
                }
            }
            return productPrice;
        }

        private static void UpdateSnapshotPricesIfNewSnapshotTime(
            DateTime riskDate,
            ISnapshotPricesProvider snapshotPricesProvider,
            TryGetResult<TimeSpan> nearestTimestamp)
        {
            DateTime nearestSnapshot = riskDate.Date.Add(nearestTimestamp.Value);
            if (snapshotPricesProvider.PriceTimestamp != nearestSnapshot)
            {
                snapshotPricesProvider.UpdatePrices(nearestSnapshot);
            }
        }

        private static TryGetResult<TimeSpan> TryGetNearestTimestamp(
            IEnumerable<TimeSpan> timestamps,
            Product product)
        {
            if (timestamps != null)
            {
                TimeSpan nearestTimestamp =
                    timestamps.Where(t => t <= product.PricingEndTime.Value.TimeOfDay)
                        .OrderByDescending(t => t)
                        .ToList()
                        .FirstOrDefault();

                if (nearestTimestamp != TimeSpan.Zero)
                {
                    return new TryGetVal<TimeSpan>() { Value = nearestTimestamp };
                }
            }
            return new TryGetVal<TimeSpan>();
        }

    }
}
