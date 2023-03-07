using Mandara.Business.Extensions;
using Mandara.Business.Model;
using Mandara.Entities;
using Mandara.Extensions.Option;
using Mandara.IRM.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Services.Prices
{
    public class DailyPricesCalculationService : IPricesCalculationService
    {
        private readonly ISettlementPricesStorage _settlementPricesStorage;
        private readonly ILivePricesProvider _originalLivePricesProvider;

        public DailyPricesCalculationService(
            ISettlementPricesStorage settlementPricesStorage,
            ILivePricesProvider livePricesProvider)
        {
            _settlementPricesStorage = settlementPricesStorage;
            _originalLivePricesProvider = livePricesProvider;
        }

        public void Initialise()
        {
        }

        public bool TryCalculateLivePrice(
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

            tradePrice = new TradePrice();
            livePrice = new LivePrice();

            Product product = trade.SecurityDefinition.Product;
            tradePrice.TradedPrice = new Money(trade.Price.Value, product.OfficialProduct.Currency.IsoName);

            Legs legs = GetLegs(trade, riskDate);
            TryGetResult<Money> livePriceValue = TryGetLivePriceForLegs(livePricesProvider, legs, product);

            if (!livePriceValue.HasValue)
            {
                return false;
            }

            livePrice.TradeLivePrice = livePriceValue.Value;
            return true;
        }

        public PriceCalcResult TryCalculateLivePrice(TradeCapture trade, DateTime riskDate, IPricesProvider livePrices)
        {
            LivePrice livePrice;
            TradePrice tradePrice;

            bool calcCompleted = TryCalculateLivePrice(trade, riskDate, livePrices, out livePrice, out tradePrice);

            return calcCompleted ? new PriceCalcResult(livePrice, tradePrice) : PriceCalcResult.Default;
        }

        private TryGetResult<Money> TryGetLivePriceForLegs(
            IPricesProvider livePricesProvider,
            Legs legs,
            Product product)
        {
            TryGetResult<Money> livePriceValue;
            if (!legs.Leg2Data.IsDefault())
            {
                livePriceValue = GetDiffLivePrice(livePricesProvider, legs, product);
            }
            else
            {
                livePriceValue = GetLegPrice(legs.Leg1Data, product, livePricesProvider);
            }

            return livePriceValue;
        }

        private TryGetResult<Money> GetDiffLivePrice(IPricesProvider livePricesProvider, Legs legs, Product product)
        {
            TryGetResult<Money> leg1LivePrice = GetLegPrice(
                legs.Leg1Data,
                product.ComplexProduct.ChildProduct1,
                livePricesProvider);

            TryGetResult<Money> leg2LivePrice = GetLegPrice(
                legs.Leg2Data,
                product.ComplexProduct.ChildProduct2,
                livePricesProvider);

            if (!leg1LivePrice.HasValue || !leg2LivePrice.HasValue)
            {
                return new TryGetVal<Money>(m => true)
                {
                    Value = Money.CurrencyDefault(product.OfficialProduct.Currency.IsoName)
                };
            }

            Money livePrice = GetLivePriceFromLegPrices(legs, product, leg1LivePrice, leg2LivePrice);

            return new TryGetVal<Money>(m => false) { Value = livePrice };
        }

        private static Money GetLivePriceFromLegPrices(
            Legs legs,
            Product product,
            TryGetResult<Money> leg1LivePrice,
            TryGetResult<Money> leg2LivePrice)
        {
            Money livePrice = GetLegAdjustedPrice(legs.Leg1Data, leg1LivePrice)
                              - GetLegAdjustedPrice(legs.Leg2Data, leg2LivePrice);
            decimal pnlConversionFactor = product.PnlFactor ?? 1M;

            if (pnlConversionFactor > 0M)
            {
                livePrice = livePrice / pnlConversionFactor;
            }

            return livePrice;
        }

        private static Money GetLegAdjustedPrice(LegData leg, TryGetResult<Money> legLivePrice)
        {
            return legLivePrice.Value * leg.LegPnlFactor.Value / leg.LegPositionFactor;
        }

        private TryGetResult<Money> GetLegPrice(LegData legData, Product product, IPricesProvider livePricesProvider)
        {
            if (legData.IsDefault())
            {
                return new TryGetVal<Money>(m => true)
                {
                    Value = Money.CurrencyDefault(product.OfficialProduct.Currency.IsoName)
                };
            }

            List<DateTime> businessDays = legData.BusinessDaysInMonth.Select(x => x.Day).ToList();

            if (product.IsBfoe())
            {
                return GetBfoePriceValue(
                    legData.TradeDaysElapsedInMonth,
                    businessDays,
                    legData.ProductId,
                    legData.MappingColumn,
                    legData.OfficialProductId,
                    livePricesProvider,
                    legData.BusinessDaysInMonth.First().PositionMonth,
                    legData.Currency);
            }

            return GetLiveDailyPriceValue(
                legData.TradeDaysElapsedInMonth,
                businessDays,
                legData.ProductId,
                legData.MappingColumn,
                legData.OfficialProductId,
                livePricesProvider,
                legData.Currency);
        }

        private Legs GetLegs(TradeCapture trade, DateTime riskDate)
        {
            DateTime currentMonthStart = trade.TradeStartDate.Value;
            DateTime currentMonthEnd = trade.TradeEndDate.GetValueOrDefault();
            if (currentMonthEnd == default(DateTime))
            {
                return Legs.Default;
            }

            LegData leg1Data = LegData.GetDefault();
            LegData leg2Data = LegData.GetDefault();

            List<PrecalcDetail> precalcDetails = GetPrecalcDetails(trade);
            Product product = trade.SecurityDefinition.Product;

            if (product.IsDailyDiff)
            {
                Product leg1Product = product.ComplexProduct.ChildProduct1;
                leg1Data = SetLegProperties(
                    precalcDetails,
                    currentMonthStart,
                    currentMonthEnd,
                    riskDate,
                    leg1Product,
                    (decimal)(product.ComplexProduct.PnlFactor1 ?? 1.0));

                Product leg2Product = product.ComplexProduct.ChildProduct2;
                leg2Data = SetLegProperties(
                    precalcDetails,
                    currentMonthStart,
                    currentMonthEnd,
                    riskDate,
                    leg2Product,
                    (decimal)(product.ComplexProduct.PnlFactor2 ?? 1.0));
            }
            else
            {
                leg1Data = SetLegProperties(
                    precalcDetails,
                    currentMonthStart,
                    currentMonthEnd,
                    riskDate,
                    product,
                    product.PnlFactor ?? 1M);
            }

            return new Legs(leg1Data, leg2Data);
        }

        private LegData SetLegProperties(
            List<PrecalcDetail> precalcDetails,
            DateTime currentMonthStart,
            DateTime currentMonthEnd,
            DateTime riskDate,
            Product product,
            decimal legPnlFactor)
        {
            List<PrecalcDetail> legDetails = precalcDetails.Where(x => x.ProductId == product.ProductId).ToList();
            List<DayPosition> daysPositions = GetDaysPositions(currentMonthStart, currentMonthEnd, legDetails);
            DateTime productRiskDate = product.GetRiskDate(riskDate);
            int tradeDaysElapsed = daysPositions.Count(p => p.Day <= productRiskDate);

            string mappingColumn = product.MonthlyOfficialProduct != null
                ? product.MonthlyOfficialProduct.MappingColumn
                : product.OfficialProduct.MappingColumn;

            int officialProductId = product.MonthlyOfficialProductId
                                    ?? product.OfficialProduct.SettlementProductId
                                    ?? product.OfficialProduct.OfficialProductId;

            if (officialProductId == product.MonthlyOfficialProductId
                && product.MonthlyOfficialProduct.SettlementProductId != null)
            {
                officialProductId = product.MonthlyOfficialProduct.SettlementProductId.Value;
            }

            return new LegData
            {
                Currency = product.OfficialProduct.Currency.IsoName,
                BusinessDaysInMonth = daysPositions,
                LegPnlFactor = legPnlFactor,
                LegPositionFactor = product.PositionFactor ?? 1M,
                MappingColumn = mappingColumn,
                OfficialProductId = officialProductId,
                PositionFactor = product.PositionFactor ?? 1M,
                ProductId = product.ProductId,
                TradeDaysElapsedInMonth = tradeDaysElapsed
            };
        }

        private List<DayPosition> GetDaysPositions(
            DateTime currentMonthStart,
            DateTime currentMonthEnd,
            List<PrecalcDetail> details)
        {
            return details.Select(x => x.DaysPositions.Select(y => new DayPosition(y.Key, y.Value, x.Month)))
                          .SelectMany(x => x).Where(
                              x => (x.Position != 0M) && (currentMonthStart <= x.Day) && (x.Day <= currentMonthEnd))
                          .ToList();
        }

        private List<PrecalcDetail> GetPrecalcDetails(TradeCapture trade)
        {
            return trade.PrecalcDetails == null
                ? new List<PrecalcDetail>()
                : trade.PrecalcDetails.Select(
                    x => new PrecalcDetail
                    {
                        Month = x.Month, ProductId = x.ProductId, DaysPositions = x.DaysPositions
                    }).ToList();
        }

        private TryGetResult<Money> GetBfoePriceValue(
            int businessDaysElapsed,
            List<DateTime> businessDays,
            int productId,
            string mappingColumn,
            int officialProductId,
            IPricesProvider livePricesProvider,
            DateTime bfoeMonth,
            string currency)
        {
            Money livePriceValue = GetPriceValue(
                businessDaysElapsed,
                businessDays,
                productId,
                mappingColumn,
                officialProductId,
                date => new Tuple<DateTime?, DateTime?>(null, null),
                ProductDateType.MonthYear,
                livePricesProvider,
                date => bfoeMonth,
                currency);

            return new TryGetVal<Money>(m => false) { Value = livePriceValue };
        }

        private Money GetPriceValue(
            int businessDaysElapsed,
            List<DateTime> businessDays,
            int productId,
            string mappingColumn,
            int officialProductId,
            Func<DateTime, Tuple<DateTime?, DateTime?>> getFuturePriceTradeStartAndEndDate,
            ProductDateType noDaysElapsedProductDateType,
            IPricesProvider livePricesProvider,
            Func<DateTime, DateTime> getTradeMonth,
            string currency)
        {
            int daysElapsed = businessDaysElapsed;
            int numberOfBusinessDays = businessDays.Count;

            Money livePriceValue = Money.CurrencyDefault(currency);

            foreach (DateTime day in businessDays)
            {
                DateTime tradeMonthDate = getTradeMonth(day);
                Tuple<DateTime?, DateTime?> tradeStartAndEndDate = getFuturePriceTradeStartAndEndDate(day);

                if (daysElapsed <= 0)
                {
                    TryGetResult<Money> productPrice = livePricesProvider.TryGetProductPrice(
                        productId,
                        tradeMonthDate,
                        noDaysElapsedProductDateType,
                        mappingColumn,
                        officialProductId,
                        tradeStartAndEndDate.Item1,
                        tradeStartAndEndDate.Item2);

                    if (productPrice.HasValue)
                    {
                        livePriceValue += productPrice.Value;
                    }
                    else
                    {
                        numberOfBusinessDays--;
                    }
                }
                else
                {
                    Money settlementPrice = Money.CurrencyDefault(currency);

                    if (_settlementPricesStorage.TryGetPrice(
                        officialProductId,
                        day,
                        tradeMonthDate,
                        out settlementPrice))
                    {
                        livePriceValue += settlementPrice;
                    }
                    else
                    {
                        TryGetResult<Money> productPrice = livePricesProvider.TryGetProductPrice(
                            productId,
                            tradeMonthDate,
                            noDaysElapsedProductDateType,
                            mappingColumn,
                            officialProductId,
                            tradeStartAndEndDate.Item1,
                            tradeStartAndEndDate.Item2);

                        if (productPrice.HasValue)
                        {
                            livePriceValue += productPrice.Value;
                        }
                        else
                        {
                            numberOfBusinessDays--;
                        }
                    }
                }

                daysElapsed--;
            }

            if (numberOfBusinessDays > 0)
            {
                livePriceValue = livePriceValue / numberOfBusinessDays;
            }

            return livePriceValue;
        }

        private TryGetResult<Money> GetLiveDailyPriceValue(
            int businessDaysElapsed,
            List<DateTime> businessDays,
            int productId,
            string mappingColumn,
            int officialProductId,
            IPricesProvider livePricesProvider,
            string currency)
        {
            Money livePriceValue = GetPriceValue(
                businessDaysElapsed,
                businessDays,
                productId,
                mappingColumn,
                officialProductId,
                date => new Tuple<DateTime?, DateTime?>(date, date),
                ProductDateType.Daily,
                livePricesProvider,
                date => date,
                currency);

            return new TryGetVal<Money>(m => false) { Value = livePriceValue };
        }
    }
}