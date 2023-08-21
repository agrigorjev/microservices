using Mandara.Business.Calculators;
using Mandara.Business.Contracts;
using Mandara.Business.Model;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Extensions.Option;
using Mandara.IRM.Server.Services;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Services.Prices
{
    public class FuturesPricesCalculationService : PricesCalculationService
    {
        private readonly IProductsStorage _productsStorage;
        private readonly ILivePricesProvider _originalLivePricesProvider;
        private readonly ISettlementPricesStorage _settlementPricesStorage;
        private readonly ISnapshotPricesProvider _snapshotPricesProvider;

        public FuturesPricesCalculationService(
            IProductsStorage productsStorage,
            ILivePricesProvider livePricesProvider,
            ISettlementPricesStorage settlementPricesStorage,
            ISnapshotPricesProvider snapshotPricesProvider,
            ITradesBusinessService tradesBusinessService,
            ILogger log)
            : base(livePricesProvider, settlementPricesStorage, log, productsStorage, tradesBusinessService)
        {
            _productsStorage = productsStorage;
            _originalLivePricesProvider = livePricesProvider;
            _settlementPricesStorage = settlementPricesStorage;
            _snapshotPricesProvider = snapshotPricesProvider;
        }

        public override bool TryCalculateLivePrice(
            TradeCapture trade,
            DateTime riskDate,
            IPricesProvider livePricesProvider,
            out LivePrice livePrice,
            out TradePrice tradePrice)
        {
            if (livePricesProvider == null)
            {
                livePricesProvider = _originalLivePricesProvider;
            }

            // manual trade impact
            if (trade.TradeId == 0)
            {
                return base.TryCalculateLivePrice(trade, riskDate, livePricesProvider, out livePrice, out tradePrice);
            }

            tradePrice = GetTradeInnerPrice(trade, riskDate);

            if (tradePrice?.TradedPrice == null)
            {
                livePrice = new LivePrice();
                return true;
            }

            Product product = trade.SecurityDefinition.Product;
            livePrice = null;
            bool isLivePriceCalculated = false;
            DateTime? expiryDate = GetExpiryDate(trade);
            bool isFuturesExpiryDay = IsFuturesExpiryDay(trade, expiryDate, riskDate);
            bool isFuturesExpired = false;

            if (isFuturesExpiryDay)
            {
                isLivePriceCalculated = GetExpiringFuturesLivePrice(
                    trade,
                    product,
                    riskDate,
                    livePricesProvider,
                    out livePrice,
                    out isFuturesExpired);
            }

            // if expire date has passed then futures expired
            if (expiryDate != null && riskDate.Date > expiryDate.Value.Date)
            {
                livePrice = new LivePrice();
                isLivePriceCalculated = true;
            }

            // For minute marker and MOPs trades where the underlying futures contract has not expired the live price  
            // must be calculated.
            if ((product.TasType == TasType.Mm || product.TasType == TasType.Mops) && isFuturesExpired)
            {
                isLivePriceCalculated = true;
            }

            if (isLivePriceCalculated)
            {
                return true;
            }

            return base.TryCalculateLivePrice(trade, riskDate, livePricesProvider, out livePrice, out tradePrice);
        }

        private bool GetExpiringFuturesLivePrice(
            TradeCapture trade,
            Product product,
            DateTime riskDate,
            IPricesProvider livePricesProvider,
            out LivePrice livePrice,
            out bool isFuturesExpired)
        {
            livePrice = new LivePrice();

            bool isLivePriceCalculated = false;
            DateTime now = riskDate;

            FutureExpireData expireData = new FutureExpireData(product, now);

            // if pricing end time has passed but it's not expire time yet
            if (expireData.HasPricingEndTimePassedButProductNotExpired())
            {
                isLivePriceCalculated = true;
                livePrice = PricingEndTimeFutureLivePriceCalculator.GetFuturesLivePriceForPricingEndTime(
                    trade,
                    riskDate,
                    livePricesProvider,
                    _snapshotPricesProvider);
            }

            isFuturesExpired = expireData.HasFutureExpired();

            // if expire time on expiry date has passed
            if (isFuturesExpired)
            {
                isLivePriceCalculated = GetExpiredFuturesLivePrice(
                    trade,
                    product,
                    riskDate,
                    livePricesProvider,
                    out livePrice);
            }

            return isLivePriceCalculated;
        }

        private bool GetExpiredFuturesLivePrice(
            TradeCapture trade,
            Product product,
            DateTime riskDate,
            IPricesProvider livePricesProvider,
            out LivePrice livePrice)
        {
            livePrice = new LivePrice();

            OfficialProduct officialProduct = trade.SecurityDefinition.Product.OfficialProduct;
            int officialProductId = officialProduct.SettlementProductId ?? officialProduct.OfficialProductId;

            Money settlementPrice;
            if (_settlementPricesStorage.TryGetPrice(
                officialProductId,
                riskDate,
                trade.Strip.Part1.StartDate,
                out settlementPrice))
            {
                livePrice.TradeLivePrice = settlementPrice;

                if (trade.Strip.IsTimeSpread)
                {
                    Money leg2Price;
                    TryGetResult<Money> leg2PriceResult = livePricesProvider.TryGetProductPrice(
                        product.ProductId,
                        trade.Strip.Part2.StartDate,
                        trade.Strip.Part2.DateType,
                        product.OfficialProduct.MappingColumn,
                        product.OfficialProductId,
                        trade.TradeStartDate,
                        trade.TradeEndDate);

                    if (!leg2PriceResult.HasValue)
                    {
                        leg2Price = new Money(0M, product.OfficialProduct.Currency.IsoName);
                    }
                    else
                    {
                        leg2Price = leg2PriceResult.Value;
                    }

                    livePrice.Leg1LivePrice = livePrice.TradeLivePrice.Value;
                    livePrice.Leg2LivePrice = leg2Price;
                    livePrice.TradeLivePrice -= leg2Price;
                }
            }
            else
            {
                // otherwise, fallback to pricing end time live price from a snapshot
                livePrice = PricingEndTimeFutureLivePriceCalculator.GetFuturesLivePriceForPricingEndTime(
                trade,
                riskDate,
                livePricesProvider,
                _snapshotPricesProvider);
            }

            return true;
        }

        private bool IsFuturesExpiryDay(TradeCapture trade, DateTime? expiryDate, DateTime riskDate)
        {
            return expiryDate.HasValue && trade.SecurityDefinition.Product.PricingEndTime.HasValue
                   && riskDate.Date == expiryDate.Value.Date;
        }

        private DateTime? GetExpiryDate(TradeCapture trade)
        {
            Product product = trade.SecurityDefinition.Product;
            DateTime startDate;

            if (trade.Strip.IsTimeSpread)
            {
                startDate = trade.Strip.Part1.StartDate < trade.Strip.Part2.StartDate
                    ? trade.Strip.Part1.StartDate
                    : trade.Strip.Part2.StartDate;
            }
            else
            {
                startDate = trade.Strip.Part1.StartDate;
            }

            DateTime expirationMonth = new DateTime(startDate.Year, startDate.Month, 1);

            Dictionary<DateTime, CalendarExpiryDate> calendarExpiryDates;
            if (_productsStorage.ExpiryDates.TryGetValue(product.ExpiryCalendar.CalendarId, out calendarExpiryDates))
            {
                CalendarExpiryDate calendarExpiryDate;
                if (calendarExpiryDates.TryGetValue(expirationMonth, out calendarExpiryDate))
                {
                    return calendarExpiryDate.ExpiryDate;
                }
            }

            return trade.ExpiryDate;
        }

    }
}