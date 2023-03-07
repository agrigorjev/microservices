using Mandara.Business.Contracts;
using Mandara.Business.Extensions;
using Mandara.Business.Model;
using Mandara.Date;
using Mandara.Entities;
using Mandara.Entities.EntitiesCustomization;
using Mandara.Extensions.Option;
using Mandara.IRM.Server.Services;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Services.Prices
{
    public class PricesCalculationService : IPricesCalculationService
    {
        private readonly ILivePricesProvider _originalLivePricesProvider;
        private readonly ISettlementPricesStorage _settlementPricesStorage;
        private readonly IProductsStorage _productsStorage;
        private readonly ITradesBusinessService _tradesBusinessService;

        public PricesCalculationService(
            ILivePricesProvider livePricesProvider,
            ISettlementPricesStorage settlementPricesStorage,
            ILogger log,
            IProductsStorage productsStorage,
            ITradesBusinessService tradesBusinessService)
        {
            _originalLivePricesProvider = livePricesProvider;
            _settlementPricesStorage = settlementPricesStorage;
            _productsStorage = productsStorage;
            _tradesBusinessService = tradesBusinessService;
        }

        public void Initialise()
        {
        }

        public virtual bool TryCalculateLivePrice(
            TradeCapture trade,
            DateTime riskDate,
            IPricesProvider livePrices,
            out LivePrice livePrice,
            out TradePrice tradePrice)
        {
            tradePrice = GetTradeInnerPrice(trade, riskDate);

            // TODO: Don't allow a null reference.  Provide a method that doesn't receive a live prices provider.
            if (livePrices == null)
            {
                livePrices = _originalLivePricesProvider;
            }

            TryGetResult<LivePrice> livePriceResult = TryCalculateLiveTradePrice(trade, riskDate, livePrices);

            livePrice = livePriceResult.HasValue ? livePriceResult.Value : new LivePrice();
            return livePriceResult.HasValue;
        }

        private TryGetResult<LivePrice> TryCalculateLiveTradePrice(
            TradeCapture trade,
            DateTime riskDate,
            IPricesProvider livePrices)
        {
            LivePrice livePrice = new LivePrice();
            Legs legs = GetLegs(trade, riskDate);

            if (!legs.Leg1Data.IsDefault())
            {
                TryGetResult<LivePrice> tryLivePrice = TryCalculateLivePriceFromLegs(trade, livePrices, legs, riskDate);

                if (tryLivePrice.HasValue)
                {
                    livePrice = tryLivePrice.Value;
                }
                else
                {
                    return new TryGetRef<LivePrice>() { Value = livePrice };
                }
            }

            if (livePrice.TradeLivePrice == null)
            {
                livePrice = livePrices.GetTradePrice(trade);
            }

            return new TryGetRef<LivePrice>() { Value = livePrice };
        }

        private TryGetResult<LivePrice> TryCalculateLivePriceFromLegs(
            TradeCapture trade,
            IPricesProvider livePrices,
            Legs legs,
            DateTime riskDate)
        {
            if (!legs.Leg2Data.IsDefault())
            {
                return TryCalculateComplexProductPrice(trade, livePrices, legs, riskDate);
            }

            return TryCalculateSimpleProductPrice(trade, livePrices, legs.Leg1Data, riskDate);
        }

        private TryGetResult<LivePrice> TryCalculateSimpleProductPrice(
            TradeCapture trade,
            IPricesProvider livePrices,
            LegData leg,
            DateTime riskDate)
        {
            decimal? strip1LivePrice = CalculateLegPrice(leg, livePrices, trade.Strip.Part1, trade, riskDate);

            if (!strip1LivePrice.HasValue)
            {
                return new TryGetRef<LivePrice>();
            }

            Money strip2Price = GetStrip2LivePrice(livePrices, leg, trade);

            LivePrice livePrice = new LivePrice();
            livePrice.TradeLivePrice = new Money(strip1LivePrice.Value, leg.Currency) - strip2Price;
            livePrice.Leg1LivePrice = new Money(strip1LivePrice.Value, leg.Currency);
            livePrice.Leg2LivePrice = strip2Price;

            return new TryGetRef<LivePrice> { Value = livePrice };
        }

        private TryGetResult<Money> TryGetStripSheetPrice(
            IPricesProvider livePrices,
            LegData leg,
            StripPart stripPart,
            TradeCapture trade)
        {
            return livePrices.TryGetProductPrice(
                leg.ProductId,
                stripPart.StartDate,
                stripPart.DateType,
                leg.MappingColumn,
                leg.OfficialProductId,
                trade.TradeStartDate,
                trade.TradeEndDate);
        }

        private TryGetResult<Money>[] GetStripSheetPricesByMonth(
            IPricesProvider livePrices,
            LegData leg,
            StripPart stripPart,
            TradeCapture trade)
        {
            return livePrices.GetProductPricesByMonth(
                leg.ProductId,
                stripPart.StartDate,
                stripPart.DateType,
                leg.MappingColumn,
                leg.OfficialProductId,
                trade.TradeStartDate,
                trade.TradeEndDate);
        }

        private decimal? GetSettlementPriceForMonth(LegData legData, Money? anotherLegPrice, DateTime targetMonth)
        {
            List<DayPosition> monthBusinessDays = legData.BusinessDays
                .Where(d => DateTimeCompare.EqualsWithMonthPrecision(d.Day, targetMonth))
                .ToList();
            if (monthBusinessDays.Count == 0)
            {
                return null;
            }
            Money? sumSettlement = GetSumSettlementPrices(
                legData.Currency,
                monthBusinessDays,
                anotherLegPrice ?? Money.CurrencyDefault(legData.Currency),
                legData.OfficialProductId);
            if (sumSettlement == null)
            {
                return null;
            }
            else
            {
                return sumSettlement.Value.Amount;
            }
        }

        private decimal ApplySettlementsToPrice(LegData legData, Money? anotherLegPrice, Money legLivePrice)
        {
            int totalBusinessDays = legData.BusinessDaysInMonth.Count;

            // if trade (or a complex product leg) has not started to roll off yet
            if (totalBusinessDays == 0)
            {
                return legLivePrice.Amount;
            }

            int businessDaysElapsed = Math.Max(0, legData.TradeDaysElapsedInMonth);

            Money sumSettlement = GetSumSettlementPrices(
                legLivePrice,
                legData.TradeDaysElapsedInMonth,
                legData.BusinessDaysInMonth,
                anotherLegPrice ?? Money.CurrencyDefault(legData.Currency),
                legData.OfficialProductId);

            return (sumSettlement / totalBusinessDays)
                   + legLivePrice * (totalBusinessDays - businessDaysElapsed) / totalBusinessDays;
        }

        private Money GetStrip2LivePrice(IPricesProvider livePrices, LegData leg, TradeCapture trade)
        {
            if (trade.Strip.Part2.IsDefault())
            {
                return Money.CurrencyDefault(leg.Currency);
            }

            TryGetResult<Money> strip2PriceOut = TryGetStripSheetPrice(livePrices, leg, trade.Strip.Part2, trade);

            return strip2PriceOut.HasValue ? strip2PriceOut.Value : Money.CurrencyDefault(leg.Currency);
        }

        private TryGetResult<LivePrice> TryCalculateComplexProductPrice(
            TradeCapture trade,
            IPricesProvider livePrices,
            Legs legs,
            DateTime riskDate)
        {
            LivePrice livePrice = new LivePrice();

            LegData leg1Data = legs.Leg1Data;
            LegData leg2Data = legs.Leg2Data;

            decimal pnlConversionFactor = trade.SecurityDefinition.Product.PnlFactor ?? 1M;

            decimal live1 = CalculateLegPrice(leg1Data, livePrices, trade.Strip.Part1, trade, riskDate)
                ?? Money.CurrencyDefault(leg1Data.Currency);

            decimal live2 = CalculateLegPrice(leg2Data, livePrices, trade.Strip.Part1, trade, riskDate)
                ?? Money.CurrencyDefault(leg2Data.Currency);

            live1 = GetLegPrice(live1, leg1Data, pnlConversionFactor);
            live2 = GetLegPrice(live2, leg2Data, pnlConversionFactor);

            livePrice.TradeLivePrice =
                livePrice.Leg1LivePrice = new Money(live1, leg1Data.Currency) - new Money(live2, leg2Data.Currency);

            if (!trade.Strip.Part2.IsDefault())
            {
                TryGetResult<Money> leg1St2LivePrice = livePrices.TryGetProductPrice(
                    leg1Data.ProductId,
                    trade.Strip.Part2.StartDate,
                    trade.Strip.Part2.DateType,
                    leg1Data.MappingColumn,
                    leg1Data.OfficialProductId,
                    trade.TradeStartDate,
                    trade.TradeEndDate);

                TryGetResult<Money> leg2St2LivePrice = livePrices.TryGetProductPrice(
                    leg2Data.ProductId,
                    trade.Strip.Part2.StartDate,
                    trade.Strip.Part2.DateType,
                    leg2Data.MappingColumn,
                    leg2Data.OfficialProductId,
                    trade.TradeStartDate,
                    trade.TradeEndDate);

                decimal l1Price = GetLegPrice(leg1St2LivePrice, leg1Data, pnlConversionFactor);
                decimal l2Price = GetLegPrice(leg2St2LivePrice, leg2Data, pnlConversionFactor);
                decimal timespreadLeg2Price = l1Price - l2Price;

                livePrice.Leg2LivePrice = new Money(timespreadLeg2Price, leg1Data.Currency);
                livePrice.TradeLivePrice -= timespreadLeg2Price;
            }

            return new TryGetRef<LivePrice> { Value = livePrice };
        }

        private Legs GetLegs(TradeCapture trade, DateTime riskDate)
        {
            List<PrecalcDetail> precalcDetails = GetPrecalcDetails(trade, trade.SecurityDefinition);
            DateRange riskMonthDateRange = GetRiskMonthDateRange(riskDate);
            Product product = trade.SecurityDefinition.Product;
            OfficialProduct officialProduct = product.OfficialProduct;
            LegData leg1Data = LegData.GetDefault();
            LegData leg2Data = LegData.GetDefault();

            leg1Data.ProductId = product.ProductId;
            leg1Data.OfficialProductId = officialProduct.SettlementProductId ?? officialProduct.OfficialProductId;
            leg1Data.MappingColumn = officialProduct.MappingColumn;
            leg1Data.Currency = officialProduct.Currency.IsoName;

            if (IsComplexProduct(product))
            {
                Product chosenProduct = product.BalmoOnComplexProduct ?? product;
                ComplexProduct chosenComplexProduct = chosenProduct.ComplexProduct;
                Product leg1Product = chosenComplexProduct.ChildProduct1;
                DateRange leg1DateRange = GetProductDateRange(
                    trade.Strip.Part1.StartDate,
                    riskMonthDateRange,
                    leg1Product);

                if (leg1DateRange.IsDefault())
                {
                    return Legs.Default;
                }

                SetLegDataProperties(
                    leg1Data,
                    riskDate,
                    precalcDetails,
                    leg1Product,
                    leg1DateRange.Start,
                    leg1DateRange.End,
                    chosenComplexProduct.ConversionFactor1 ?? 1M,
                    (decimal?)chosenComplexProduct.PnlFactor1);

                Product leg2Product = chosenComplexProduct.ChildProduct2;
                DateRange leg2DateRange = GetProductDateRange(
                    trade.Strip.Part1.StartDate,
                    riskMonthDateRange,
                    leg2Product);

                if (leg2DateRange.IsDefault())
                {
                    return Legs.Default;
                }

                SetLegDataProperties(
                    leg2Data,
                    riskDate,
                    precalcDetails,
                    leg2Product,
                    leg2DateRange.Start,
                    leg2DateRange.End,
                    chosenComplexProduct.ConversionFactor2 ?? 1M,
                    (decimal?)chosenComplexProduct.PnlFactor2);
            }
            else
            {
                Product chosenProduct = product.BalmoOnCrudeProduct ?? product;
                DateRange tradeDateRange = GetProductDateRange(
                    trade.Strip.Part1.StartDate,
                    riskMonthDateRange,
                    chosenProduct);

                if (tradeDateRange.IsDefault())
                {
                    return Legs.Default;
                }

                List<PrecalcDetail> details = precalcDetails.Where(x => x.ProductId == chosenProduct.ProductId).ToList();
                List<DayPosition> daysPositionsInMonth = GetDayPositionsInMonth(
                    details,
                    tradeDateRange.Start,
                    tradeDateRange.End);
                List<DayPosition> daysPositions = GetDayPositions(details);
                DateTime productRiskDate = chosenProduct.GetRiskDate(riskDate);

                leg1Data.BusinessDaysInMonth = daysPositionsInMonth;
                leg1Data.TradeDaysElapsedInMonth = daysPositionsInMonth.Count(p => p.Day <= productRiskDate);
                leg1Data.BusinessDays = daysPositions;
            }

            if (product.IsProductDaily)
            {
                leg2Data.BusinessDaysInMonth = new List<DayPosition>();
            }

            return new Legs(leg1Data, leg2Data);
        }

        private static bool IsComplexProduct(Product product)
        {
            return (product.ComplexProduct != null) || (product.BalmoOnComplexProduct != null);
        }

        private static DateRange GetRiskMonthDateRange(DateTime riskDate)
        {
            DateTime riskMonthStart = new DateTime(riskDate.Year, riskDate.Month, 1);
            DateTime riskMonthEnd = new DateTime(
                riskDate.Year,
                riskDate.Month,
                DateTime.DaysInMonth(riskDate.Year, riskDate.Month));

            return new DateRange(riskMonthStart, riskMonthEnd);
        }

        private DateRange GetProductDateRange(DateTime startDate, DateRange currentMonthRange, Product leg1Product)
        {
            if (leg1Product.Type == ProductType.TradeMonthSwap)
            {
                DateTime stripStartDate = startDate;
                DateTime month = new DateTime(stripStartDate.Year, stripStartDate.Month, 1);

                return GetTradeMonthSwapExpiryDates(month, leg1Product);
            }

            return currentMonthRange;
        }

        private void SetLegDataProperties(
            LegData legData,
            DateTime riskDate,
            List<PrecalcDetail> precalcDetails,
            Product legProduct,
            DateTime legMonthStart,
            DateTime legMonthEnd,
            decimal legPositionFactor,
            decimal? legPnlFactor)
        {
            List<PrecalcDetail> legDetails = precalcDetails.Where(x => x.ProductId == legProduct.ProductId).ToList();
            List<DayPosition> legDaysPositions = GetDayPositionsInMonth(legDetails, legMonthStart, legMonthEnd);

            DateTime productRiskDate = legProduct.GetRiskDate(riskDate);

            legData.BusinessDaysInMonth = legDaysPositions;
            legData.TradeDaysElapsedInMonth = legDaysPositions.Count(p => p.Day <= productRiskDate);
            legData.BusinessDays = GetDayPositions(legDetails);
            legData.MappingColumn = legProduct.OfficialProduct.MappingColumn;
            legData.Currency = legProduct.OfficialProduct.Currency.IsoName;
            legData.OfficialProductId = legProduct.OfficialProduct.SettlementProductId
                                        ?? legProduct.OfficialProduct.OfficialProductId;
            legData.ProductId = legProduct.ProductId;
            legData.PositionFactor = legProduct.PositionFactor ?? 1M;

            legData.LegPositionFactor = legPositionFactor;
            legData.LegPnlFactor = legPnlFactor;
        }

        private List<DayPosition> GetDayPositionsInMonth(
            List<PrecalcDetail> details,
            DateTime currentMonthStart,
            DateTime currentMonthEnd)
        {
            return
                details.Select(x => x.DaysPositions.Select(y => new DayPosition(y.Key, y.Value, x.Month)))
                       .SelectMany(x => x)
                       .Where(x => (x.Position != 0M) && (currentMonthStart <= x.Day) && (x.Day <= currentMonthEnd))
                       .ToList();
        }
        private List<DayPosition> GetDayPositions(
            List<PrecalcDetail> details)
        {
            return
                details.Select(x => x.DaysPositions.Select(y => new DayPosition(y.Key, y.Value, x.Month)))
                       .SelectMany(x => x)
                       .Where(x => x.Position != 0M)
                       .ToList();
        }
        private decimal GetLegPrice(TryGetResult<Money> legLivePrice, LegData legData, decimal pnlConversionFactor)
        {
            decimal legAmount = 0M;

            if (legLivePrice.HasValue)
            {
                legAmount = legLivePrice.Value.Amount;
            }

            return GetLegPrice(legAmount, legData, pnlConversionFactor);
        }

        private decimal GetLegPrice(decimal legAmount, LegData legData, decimal pnlConversionFactor)
        {
            decimal legPrice;

            if (legData.LegPnlFactor.HasValue)
            {
                legPrice = legAmount * legData.LegPnlFactor.Value;
            }
            else
            {
                // TODO: What if there was no price?
                legPrice = legAmount * (legData.LegPositionFactor / legData.PositionFactor);

                if (pnlConversionFactor > 0M)
                {
                    legPrice = legPrice / pnlConversionFactor;
                }
            }

            return legPrice;
        }

        private decimal? CalculateLegPriceForFullMonthDateType(
            LegData legData,
            IPricesProvider livePricesProvider,
            StripPart stripPart,
            TradeCapture trade,
            DateTime riskDate)
        {
            TryGetResult<Money>[] strip1SheetPricesByMonth =
                GetStripSheetPricesByMonth(livePricesProvider, legData, stripPart, trade);

            if (strip1SheetPricesByMonth.Length == 0)
            {
                return null;
            }

            if (trade.Strip.Part1.DateType == ProductDateType.Custom && !trade.TradeStartDate.HasValue)
            {
                return null;
            }

            decimal sum = 0;
            int membersInSum = 0;
            DateTime baseDate = trade.Strip.Part1.DateType == ProductDateType.Custom ? trade.TradeStartDate.Value
                : trade.Strip.Part1.StartDate;
            for (int i = 0; i < strip1SheetPricesByMonth.Length; i++)
            {
                DateTime date = baseDate.AddMonths(i);
                if (DateTimeCompare.LessWithMonthPrecision(date, riskDate))
                {
                    decimal? settlementForMonth = GetSettlementPriceForMonth(
                        legData,
                        Money.CurrencyDefault(legData.Currency),
                        trade.Strip.Part1.StartDate.AddMonths(i));
                    if (settlementForMonth.HasValue)
                    {
                        sum += settlementForMonth.Value;
                        membersInSum++;
                    }
                }
                else if (DateTimeCompare.EqualsWithMonthPrecision(date, riskDate))
                {
                    if (strip1SheetPricesByMonth[i].HasValue)
                    {
                        decimal settlementForMonth = ApplySettlementsToPrice(
                            legData,
                            Money.CurrencyDefault(legData.Currency),
                            strip1SheetPricesByMonth[i].Value);
                        sum += settlementForMonth;
                        membersInSum++;
                    }
                }
                else
                {
                    if (strip1SheetPricesByMonth[i].HasValue)
                    {
                        sum += strip1SheetPricesByMonth[i].Value;
                        membersInSum++;
                    }
                }
            }

            if (membersInSum == 0)
            {
                return null;
            }

            return sum / membersInSum;
        }

        private decimal? CalculateLegPriceForDailyDateType(
            LegData leg,
            IPricesProvider livePricesProvider,
            StripPart stripPart,
            TradeCapture trade)
        {
            TryGetResult<Money> livePrice = TryGetStripSheetPrice(livePricesProvider, leg, stripPart, trade);

            Money legLivePrice = !livePrice.HasValue
                ? Money.CurrencyDefault(leg.Currency)
                : livePrice.Value;

            return ApplySettlementsToPrice(leg, null, legLivePrice);
        }

        private decimal? CalculateLegPrice(
            LegData legData,
            IPricesProvider livePricesProvider,
            StripPart stripPart,
            TradeCapture trade,
            DateTime riskDate)
        {

            ProductDateType dateType = stripPart.DateType;
            if (dateType != ProductDateType.Quarter && dateType != ProductDateType.Year
                && dateType != ProductDateType.Custom)
            {
                return CalculateLegPriceForDailyDateType(legData, livePricesProvider, stripPart, trade);
            }
            else
            {
                return CalculateLegPriceForFullMonthDateType(legData, livePricesProvider, stripPart, trade, riskDate);
            }
        }

        private DateRange GetTradeMonthSwapExpiryDates(DateTime riskDate, Product product)
        {
            DateTime prevRiskMonth = riskDate.AddMonths(-1);
            DateTime? startDate = _productsStorage.GetExpiryDate(product, prevRiskMonth.Year, prevRiskMonth.Month);
            DateTime? endDate = _productsStorage.GetExpiryDate(product, riskDate.Year, riskDate.Month);

            if ((startDate == null) || (endDate == null))
            {
                return DateRange.Default;
            }

            DateTime currentMonthStart = startDate.Value.AddDays(1);
            DateTime currentMonthEnd = endDate.Value;

            return new DateRange(currentMonthStart, currentMonthEnd);
        }

        protected TradePrice GetTradeInnerPrice(TradeCapture trade, DateTime riskDate)
        {
            TryGetResult<Money> tradePriceVal = DoGetTradeInnerPrice(trade, riskDate);

            TradePrice tradePrice = new TradePrice
            {
                TradedPrice = tradePriceVal.HasValue ? tradePriceVal.Value : default(Money?),
            };

            if (trade.IsParentTimeSpread.HasValue && trade.IsParentTimeSpread.Value)
            {
                if (trade.Leg1Trade == null)
                {
                    tradePrice.Leg1Price = FindLegTradePrice(trade, trade.Strip.Part1);
                }
                else
                {
                    tradePrice.Leg1Price = new Money(
                        trade.Leg1Trade.Price.Value,
                        trade.Leg1Trade.SecurityDefinition.Product.OfficialProduct.Currency.IsoName);
                }

                if (trade.Leg2Trade == null)
                {
                    StripPart stripPart = trade.Strip.Part2.IsDefault() ? trade.Strip.Part1 : trade.Strip.Part2;
                    tradePrice.Leg2Price = FindLegTradePrice(trade, stripPart);
                }
                else
                {
                    tradePrice.Leg2Price = new Money(
                        trade.Leg2Trade.Price.Value,
                        trade.Leg2Trade.SecurityDefinition.Product.OfficialProduct.Currency.IsoName);
                }
            }

            return tradePrice;
        }

        private Money? FindLegTradePrice(TradeCapture trade, StripPart stripPart)
        {
            TradeCapture legTrade = _tradesBusinessService.GetLegTrade(trade, stripPart);

            if (legTrade == null)
                return null;

            return new Money(legTrade.Price.Value, trade.SecurityDefinition.Product.OfficialProduct.Currency.IsoName);
        }

        private TryGetResult<Money> DoGetTradeInnerPrice(TradeCapture trade, DateTime riskDate)
        {
            Product product = trade.SecurityDefinition.Product;

            if (trade.IsTas())
            {
                return GetTasTradePrice(trade, product, riskDate);
            }

            return new TryGetVal<Money>()
            {
                Value = new Money(trade.Price.Value, product.OfficialProduct.Currency.IsoName)
            };
        }

        private TryGetResult<Money> GetTasTradePrice(TradeCapture trade, Product product, DateTime riskDate)
        {
            if (!product.RolloffTimeWithTimezone.HasValue)
            {
                return new TryGetVal<Money>(val => true);
            }

            DateTime activationDate = _productsStorage.GetTasActivationDate(product, trade.TradeDate.Value)
                                                      .Date.Add(product.RolloffTimeWithTimezone.Value.TimeOfDay);

            if (trade.IsIce())
            {
                return GetIceTasTradePrice(trade, activationDate, product, riskDate);
            }

            if (trade.IsNymex())
            {
                return GetNymexTasTradePrice(trade, activationDate, product, riskDate);
            }

            throw new InvalidOperationException($"Trade [{trade}] is not a TAS trade.");
        }

        private TryGetResult<Money> GetIceTasTradePrice(
            TradeCapture trade,
            DateTime tasActivationDatetime,
            Product product,
            DateTime riskDate)
        {
            if (!HasIceTasActivationTimePassed(tasActivationDatetime, riskDate))
            {
                return new TryGetVal<Money>(val => true);
            }

            int officialProductId;
            bool hasOfficialProductId = TryGetOfficialProductId(trade, product, riskDate, out officialProductId);
            Money strip1Settlement;

            if (hasOfficialProductId
                && _settlementPricesStorage.TryGetPrice(
                    officialProductId,
                    tasActivationDatetime.Date,
                    trade.Strip.Part1.StartDate,
                    out strip1Settlement))
            {
                Money tradeInnerPrice = strip1Settlement + trade.Price.Value;
                Money strip2Settlement;

                if ((!trade.Strip.Part2.IsDefault())
                    && _settlementPricesStorage.TryGetPrice(
                        officialProductId,
                        tasActivationDatetime.Date,
                        trade.Strip.Part2.StartDate,
                        out strip2Settlement))
                {
                    tradeInnerPrice -= strip2Settlement;
                }

                return new TryGetVal<Money>() { Value = tradeInnerPrice };
            }

            return new TryGetVal<Money>(val => true);
        }

        private static bool HasIceTasActivationTimePassed(DateTime tasActivationDatetime, DateTime riskDate)
        {
            return riskDate > tasActivationDatetime;
        }

        private TryGetResult<Money> GetNymexTasTradePrice(
            TradeCapture trade,
            DateTime tasActivationDatetime,
            Product product,
            DateTime riskDate)
        {
            if (!HasNymexTasActivationTimePassed(trade, tasActivationDatetime))
            {
                return new TryGetVal<Money>(val => true);
            }

            int officialProductId;
            bool hasOfficialProductId = TryGetOfficialProductId(trade, product, riskDate, out officialProductId);
            Money settlementPrice;

            if (hasOfficialProductId
                && _settlementPricesStorage.TryGetPrice(
                    officialProductId,
                    tasActivationDatetime.Date,
                    trade.Strip.Part1.StartDate,
                    out settlementPrice))
            {
                return new TryGetVal<Money>()
                {
                    Value = new Money(trade.Price.Value, product.OfficialProduct.Currency.IsoName)
                };
            }

            return new TryGetVal<Money>(val => true);
        }

        private static bool HasNymexTasActivationTimePassed(TradeCapture trade, DateTime tasActivationDatetime)
        {
            return trade.LastReplacedTimestamp != null && trade.LastReplacedTimestamp > tasActivationDatetime;
        }

        private bool TryGetOfficialProductId(
            TradeCapture trade,
            Product product,
            DateTime riskDate,
            out int officialProductId)
        {
            OfficialProduct officialProduct = GetProductOfficialProduct(trade, product, riskDate);

            if (null == officialProduct)
            {
                officialProductId = -1;
                return false;
            }

            officialProductId = officialProduct.SettlementProductId ?? officialProduct.OfficialProductId;
            return true;
        }

        private OfficialProduct GetProductOfficialProduct(TradeCapture trade, Product product, DateTime riskDate)
        {
            OfficialProduct officialProduct;

            if (product.TradedAtSettlement())
            {
                if (IsNotYetActivated(trade, riskDate))
                {
                    officialProduct = null;
                }
                else if (IsActivationTimePastToday(trade, riskDate))
                {
                    officialProduct = product.TasOfficialProduct ?? product.OfficialProduct;
                }
                else
                {
                    officialProduct = product.OfficialProduct;
                }
            }
            else
            {
                officialProduct = product.OfficialProduct;
            }

            return officialProduct;
        }

        private bool IsNotYetActivated(TradeCapture trade, DateTime riskDate)
        {
            return CheckActivationDay(
                trade,
                riskDate,
                (tradeDay, tasActivationTime, today) => (today > tasActivationTime && today == tradeDay));
        }

        private bool CheckActivationDay(
            TradeCapture trade,
            DateTime riskDate,
            Func<DateTime, DateTime, DateTime, bool> activationTimePredicate)
        {
            if (!trade.SecurityDefinition.Product.TradedAtSettlement())
            {
                return false;
            }

            Product product = trade.SecurityDefinition.Product;
            DateTime now = riskDate;
            DateTime tradeDate = trade.TradeDate.Value.Date;
            DateTime activationDate = _productsStorage.GetTasActivationDate(product, tradeDate).Date;
            DateTime tradeNow = activationDate.Add(now.TimeOfDay);
            DateTime today = now.Date;
            DateTime tasActivationTime = product.GetTasActivationTime(tradeNow);

            return activationTimePredicate(activationDate, tasActivationTime, today);
        }

        protected bool IsActivationTimePastToday(TradeCapture trade, DateTime riskDate)
        {
            return CheckActivationDay(
                trade,
                riskDate,
                (tradeDay, tasActivationTime, today) => (today <= tasActivationTime && today == tradeDay));
        }

        protected virtual List<PrecalcDetail> GetPrecalcDetails(
            TradeCapture trade,
            SecurityDefinition securityDefinition)
        {
            List<PrecalcDetail> daysPositions = null;

            if ((securityDefinition != null) && (securityDefinition.PrecalcDetails != null)
                && (securityDefinition.PrecalcDetails.Count > 0))
            {
                daysPositions =
                    securityDefinition.PrecalcDetails.Select(
                        x =>
                            new PrecalcDetail
                            {
                                Month = x.Month,
                                ProductId = x.ProductId,
                                DaysPositions = x.DaysPositions
                            }).ToList();
            }

            if (daysPositions == null)
            {
                if ((trade != null) && (trade.PrecalcDetails != null) && (trade.PrecalcDetails.Count > 0))
                {
                    daysPositions =
                        trade.PrecalcDetails.Select(
                            x =>
                                new PrecalcDetail
                                {
                                    Month = x.Month,
                                    ProductId = x.ProductId,
                                    DaysPositions = x.DaysPositions
                                }).ToList();
                }
            }

            return daysPositions ?? new List<PrecalcDetail>();
        }

        private Money GetSumSettlementPrices(
            Money livePrice,
            int businessDaysElapsed,
            List<DayPosition> businessDays,
            Money? leg2price,
            int officialProductId)
        {
            Money sumSettlementPrices = new Money(0M, livePrice.Currency);
            int settlementPricesCount = 0;

            for (int i = 0; i < businessDaysElapsed; i++)
            {
                Money settlementPrice;
                if (_settlementPricesStorage.TryGetPrice(
                    officialProductId,
                    businessDays[i].Day,
                    businessDays[i].PositionMonth,
                    out settlementPrice))
                {
                    // in case it's a time spread we would calculate leg2price, otherwise it's zero
                    if (leg2price != null)
                    {
                        settlementPrice -= leg2price.Value;
                        ++settlementPricesCount;
                    }
                }
                else
                {
                    // if we haven't got settlement price for a date we don't subtract leg2price (cause we use trade
                    // live price)
                    settlementPrice = livePrice;
                }

                sumSettlementPrices += settlementPrice;
            }

            return sumSettlementPrices;
        }

        private Money? GetSumSettlementPrices(
            string currency,
            List<DayPosition> businessDays,
            Money? leg2price,
            int officialProductId)
        {
            Money sumSettlementPrices = new Money(0M, currency);
            int settlementPricesCount = 0;

            for (int i = 0; i < businessDays.Count; i++)
            {
                Money settlementPrice;
                if (_settlementPricesStorage.TryGetPrice(
                    officialProductId,
                    businessDays[i].Day,
                    businessDays[i].PositionMonth,
                    out settlementPrice))
                {
                    // in case it's a time spread we would calculate leg2price, otherwise it's zero
                    if (leg2price != null)
                    {
                        settlementPrice -= leg2price.Value;
                    }
                    ++settlementPricesCount;
                    sumSettlementPrices += settlementPrice;
                }
            }

            if (settlementPricesCount == 0)
                return null;

            return sumSettlementPrices / settlementPricesCount;
        }

        public PriceCalcResult TryCalculateLivePrice(
            TradeCapture trade,
            DateTime riskDate,
            IPricesProvider livePricesProvider)
        {
            LivePrice livePrice;
            TradePrice tradePrice;

            bool calcCompleted = TryCalculateLivePrice(
                trade,
                riskDate,
                livePricesProvider,
                out livePrice,
                out tradePrice);


            return calcCompleted ? new PriceCalcResult(livePrice, tradePrice) : PriceCalcResult.Default;
        }
    }
}