using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mandara.Business.Contracts;
using Mandara.Business.Extensions;
using Mandara.Business.Model;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.Services;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Position
{
    public class PositionCalculator : IPositionCalculationService
    {
        private class TradePositionInput
        {
            public int Trade { get; }
            public decimal Quantity { get; }
            public int? Portfolio { get; }
            [Obsolete("Switch to non-nullable")]
            public DateTime? StartOn { get; }
            public CoeffEntityId PrecalcSourceId { get; }

            public TradePositionInput(
                int trade,
                decimal quantity,
                int? portfolio,
                DateTime? startOn,
                CoeffEntityId precalcSrcId)
            {
                Trade = trade;
                Quantity = quantity;
                Portfolio = portfolio;
                StartOn = startOn;
                PrecalcSourceId = precalcSrcId;
            }
        }

        private readonly IProductsStorage _productsStorage;
        private readonly ISecurityDefinitionsStorage _securityDefinitionsStorage;
        private readonly ILogger _log;
        private readonly ICalculationDetailIdentifierService _calculationDetailIdentifierService;

        public PositionCalculator(
            [NotNull] IProductsStorage productsStorage,
            [NotNull] ISecurityDefinitionsStorage securityDefinitionsStorage,
            [NotNull] ILogger log,
            ICalculationDetailIdentifierService calculationDetailIDService)
        {
            _productsStorage = productsStorage ?? throw new ArgumentNullException(nameof(productsStorage));

            _securityDefinitionsStorage = securityDefinitionsStorage
                                          ?? throw new ArgumentNullException(nameof(securityDefinitionsStorage));

            _log = log ?? throw new ArgumentNullException(nameof(log));
            _calculationDetailIdentifierService = calculationDetailIDService;
        }

        public List<CalculationDetailModel> CalculatePositions(
            List<TradeModel> trades,
            List<SdQuantityModel> sdQuantities,
            DateTime? riskDateParam = null,
            bool splitWeekends = false)
        {
            List<CalculationDetailModel> dailyPositions = new List<CalculationDetailModel>();
            DateTime riskDate = riskDateParam ?? InternalTime.LocalNow();
            DateTime riskDay = riskDate.Date;

            dailyPositions = GetTradeModelDailyPositions(trades, splitWeekends, riskDate);

            foreach (SdQuantityModel sdQuantity in sdQuantities)
            {
                SecurityDefinition securityDefinition = _securityDefinitionsStorage
                                                        .TryGetSecurityDefinition(sdQuantity.SecurityDefinitionId)
                                                        .Value;

                Product sourceProduct = _productsStorage.TryGetProduct(securityDefinition.product_id.Value).Value;

                if (sourceProduct.Type == ProductType.Spot)
                {
                    continue;
                }

                dailyPositions.AddRange(
                    CalculateDailyPositionForSecurityDefinition(
                        sdQuantity,
                        securityDefinition,
                        sourceProduct,
                        riskDate,
                        splitWeekends));
            }

            return dailyPositions;
        }

        private List<CalculationDetailModel> GetTradeModelDailyPositions(
            List<TradeModel> trades,
            bool splitWeekends,
            DateTime riskDate)
        {
            // TODO: put in stopwatch to measure time taken to calculate
            Func<Product, IEnumerable<DateTime>, DateTime, List<List<DateTime>>> getDays = splitWeekends
                ? (Func<Product, IEnumerable<DateTime>, DateTime, List<List<DateTime>>>)GetSplitWeekendsDaysList
                : GetDaysList;

            List<CalculationDetailModel> dailyPositions = trades.Where(trade => trade.IsParentTrade ?? true)
                                                                .SelectMany(
                                                                    trade => CalculateDailyPositionForTradeModel(
                                                                        trade,
                                                                        riskDate,
                                                                        getDays,
                                                                        splitWeekends))
                                                                .ToList();

            return dailyPositions;
        }

        private IEnumerable<CalculationDetailModel> CalculateDailyPositionForTradeModel(
            TradeModel trade,
            DateTime? riskDate,
            Func<Product, IEnumerable<DateTime>, DateTime, List<List<DateTime>>> getDays,
            bool splitWeekends)
        {
            SecurityDefinition securityDefinition =
                _securityDefinitionsStorage.TryGetSecurityDefinition(trade.SecurityDefinitionId).Value;

            Product sourceProduct = _productsStorage.TryGetProduct(securityDefinition.product_id.Value).Value;
            bool isDailyProduct = sourceProduct.IsProductDaily;
            int dailyDiffMonthShift = 0;

            if (isDailyProduct)
            {
                dailyDiffMonthShift = sourceProduct.DailyDiffMonthShift;
            }

            return trade.PrecalcDetails.SelectMany(
                precalcDetail =>
                {
                    TradePositionInput posInput = new TradePositionInput(
                        trade.TradeId,
                        trade.Quantity.Value,
                        trade.PortfolioId.Value,
                        trade.TradeStartDate,
                        new CoeffEntityId(CoeffEntity.TradeCapture, trade.TradeId));

                    return CalculateDailyPositions(
                        posInput,
                        riskDate,
                        getDays,
                        splitWeekends,
                        precalcDetail,
                        sourceProduct,
                        dailyDiffMonthShift,
                        securityDefinition);
                });
        }

        private IEnumerable<CalculationDetailModel> CalculateDailyPositions(
            TradePositionInput trade,
            DateTime? riskDate,
            Func<Product, IEnumerable<DateTime>, DateTime, List<List<DateTime>>> getDays,
            bool splitWeekends,
            PrecalcDetailModel precalcDetail,
            Product sourceProduct,
            int dailyDiffMonthShift,
            SecurityDefinition securityDefinition)
        {
            Product product = _productsStorage.TryGetProduct(precalcDetail.ProductId).Value;
            DateTime productRiskDate = product.GetRiskDate(riskDate);

            // There should be no need to go through the process of deserialising the precalc details if the
            // contract has already expired.
            if (productRiskDate > precalcDetail.MaxDay)
            {
                return new List<CalculationDetailModel>();
            }

            // TODO: Can the code be further simplified (i.e. more shared between TradeModel and TradeCapture calcs) by
            // constructing a PrecalcDetail using the PrecalcDetailModel and the deserialised positions?
            Dictionary<DateTime, decimal> daysPositions = DayPositionsSerialisation.DeserialiseDailyPositions(
                precalcDetail.DaysSerialized,
                precalcDetail.Month,
                precalcDetail.ProductId,
                nameof(TradeCapture),
                trade.Trade);

            List<List<DateTime>> daysList = getDays(product, daysPositions.Keys, productRiskDate);

            if (daysList.Count == 0)
            {
                return new List<CalculationDetailModel>();
            }

            return daysList.SelectMany(
                days => CalculateDailyPositions(
                    trade,
                    splitWeekends,
                    days,
                    daysPositions,
                    product,
                    sourceProduct,
                    precalcDetail.Month,
                    dailyDiffMonthShift,
                    securityDefinition));
        }

        private static DateTime? GetSwapSettleDate(Product product, List<DateTime> days)
        {
            return product.IsCalendarDaySwap && days.Count() == 1 && days[0].IsWeekendDay()
                ? days[0]
                : default(DateTime?);
        }

        private List<CalculationDetailModel> CalculateDailyPositions(
            TradePositionInput trade,
            bool splitWeekends,
            List<DateTime> days,
            Dictionary<DateTime, decimal> daysPositions,
            Product product,
            Product sourceProduct,
            DateTime positionMonth,
            int dailyDiffMonthShift,
            SecurityDefinition securityDefinition)
        {
            DateTime? calendarDaySwapSettlePriceDate =
                splitWeekends ? GetSwapSettleDate(product, days) : default(DateTime?);

            return days.Where(day => daysPositions[day] != 0M) //.Where(day => !day.IsWeekendDay())
                       .Select(
                           (day, dayOfMonth) =>
                           {
                               decimal dayQuantity = daysPositions[day] * trade.Quantity;

                               return new CalculationDetailModel(
                                   _calculationDetailIdentifierService,
                                   product,
                                   sourceProduct,
                                   positionMonth,
                                   day,
                                   dayQuantity,
                                   trade.Portfolio,
                                   trade.PrecalcSourceId,
                                   dailyDiffMonthShift,
                                   sourceProduct.IsProductDaily
                                       ? new List<DailyDetail>()
                                       {
                                           new DailyDetail(day, dayQuantity)
                                       }
                                       : new List<DailyDetail>(),
                                   StripHelper.GetBalmoStripNameWithDate(securityDefinition.StripName, trade.StartOn),
                                   calendarDaySwapSettlePriceDate);
                           })
                       .ToList();
        }

        private List<List<DateTime>> GetDaysList(Product product, IEnumerable<DateTime> days, DateTime productRiskDate)
        {
            return new List<List<DateTime>>()
            {
                days.Where(day => day > productRiskDate).ToList()
            };
        }

        private List<List<DateTime>> GetSplitWeekendsDaysList(
            Product product,
            IEnumerable<DateTime> days,
            DateTime productRiskDate)
        {
            return (product.IsCalendarDaySwap)
                ? Split(days.Where(day => day > productRiskDate), (day) => !day.IsWeekendDay())
                : new List<List<DateTime>>()
                {
                    days.Where(day => day > productRiskDate).ToList()
                };
        }

        private static List<List<T>> Split<T>(IEnumerable<T> coll, Func<T, bool> filter)
        {
            return coll.GroupBy(filter)
                       .SelectMany(
                           ts => ts.Key
                               ? new List<List<T>>()
                               {
                                   ts.ToList()
                               }
                               : ts.Select(
                                   val => new List<T>()
                                   {
                                       val
                                   }))
                       .ToList();
        }

        public List<CalculationDetailModel> CalculatePositions(
            [NotNull] TradeCapture trade,
            DateTime? riskDateParam = null,
            bool checkProductValidation = false,
            bool splitWeekends = false)
        {
            return CalculatePositions(
                trade,
                riskDateParam ?? SystemTime.Now(),
                checkProductValidation ? IsProductValid(trade.TradeId) : (Product ignoreProduct) => true,
                splitWeekends);
        }

        public List<CalculationDetailModel> CalculatePositions(
            [NotNull] TradeCapture trade,
            DateTime riskDate,
            Func<Product, bool> isProductValid,
            bool splitWeekends)
        {
            SecurityDefinition security = trade.SecurityDefinition
                                          ?? _securityDefinitionsStorage
                                             .TryGetSecurityDefinition(trade.SecurityDefinitionId)
                                             .Value;

            List<PrecalcDetail> daysPositions = GetPrecalcDetails(trade, security);

            if (daysPositions == null)
            {
                _log.Trace("CalculatePositions: No precalc details day positions found for trade [{0}]", trade.TradeId);
                return new List<CalculationDetailModel>();
            }

            Product sourceProduct = GetSourceProduct(security, isProductValid);

            if (sourceProduct.IsDefault)
            {
                return new List<CalculationDetailModel>();
            }

            int dailyDiffMonthShift = GetDailyDiffMonthShift(trade, sourceProduct.IsProductDaily, sourceProduct);

            var posInput = new TradePositionInput(
                trade.TradeId,
                trade.Quantity.Value,
                trade.PortfolioId,
                trade.TradeStartDate,
                GetPrecalcSourceId(trade, security));

            return daysPositions.SelectMany(
                                    precalc => PositionsForPrecalc(
                                        posInput,
                                        riskDate,
                                        splitWeekends,
                                        precalc,
                                        sourceProduct,
                                        dailyDiffMonthShift,
                                        security))
                                .ToList();
        }

        private IEnumerable<CalculationDetailModel> PositionsForPrecalc(
            TradePositionInput trade,
            DateTime riskDate,
            bool splitWeekends,
            PrecalcDetail precalcDetail,
            Product sourceProduct,
            int dailyDiffMonthShift,
            SecurityDefinition security)
        {
            Product product = _productsStorage.TryGetProduct(precalcDetail.ProductId).Value;
            DateTime productRiskDate = product.GetRiskDate(riskDate);

            _log.Trace(
                "CalculatePositions: precalc detail for trade [{0}], product [{1}] and risk date[{2}]",
                trade.Trade,
                product.ProductId,
                riskDate);

            IEnumerable<DateTime> precalcDays = precalcDetail.DailyPositions.Keys;

            List<List<DateTime>> daysList = splitWeekends
                ? GetSplitWeekendsDaysList(product, precalcDays, productRiskDate)
                : GetDaysList(product, precalcDays, productRiskDate);

            _log.Trace(
                "CalculatePositions: precalc detail for trade [{0}] has {1} days in its list, [{2}]",
                trade.Trade,
                daysList.Count,
                String.Join(";", precalcDays.Select(day => day.ToString())));

            return daysList.SelectMany(
                               days => CalculateDailyPositions(
                                   trade,
                                   splitWeekends,
                                   days,
                                   precalcDetail.DailyPositions,
                                   product,
                                   sourceProduct,
                                   precalcDetail.Month,
                                   dailyDiffMonthShift,
                                   security))
                           .Where(pos => !pos.IsDefault());
        }

        private CalculationDetailModel CalculatePositionForDayRange(
            TradeCapture trade,
            bool splitWeekends,
            List<DateTime> days,
            PrecalcDetail precalcDetail,
            bool isDailyProduct,
            Product product,
            Product sourceProduct,
            CoeffEntityId precalcSourceId,
            int dailyDiffMonthShift,
            SecurityDefinition security)
        {
            decimal totalAmount = 0;
            List<DailyDetail> dailyDetails = new List<DailyDetail>();

            foreach (var p in days)
            {
                decimal dayAmount = precalcDetail.DailyPositions[p];
                totalAmount += dayAmount;

                if (isDailyProduct)
                {
                    dailyDetails.Add(new DailyDetail(p, dayAmount * trade.Quantity.Value));
                }
            }

            if (totalAmount == 0M && dailyDetails.Count == 0)
            {
                _log.Trace(
                    "CalculatePositions: precalc detail for trade [{0}] resulted in zero total amount",
                    trade.TradeId);

                return CalculationDetailModel.Default;
            }

            totalAmount = totalAmount * trade.Quantity.Value;

            // if it was split then we need to remember date for Settlement price
            DateTime? calendarDaySwapSettlPriceDate = null;

            if (product.IsCalendarDaySwap
                && splitWeekends
                && days.Count() == 1
                && (days[0].DayOfWeek == DayOfWeek.Sunday || days[0].DayOfWeek == DayOfWeek.Saturday))
            {
                calendarDaySwapSettlPriceDate = days[0];
            }

            CalculationDetailModel calculationDetail = new CalculationDetailModel(
                _calculationDetailIdentifierService,
                product,
                sourceProduct,
                precalcDetail.Month.Year,
                precalcDetail.Month.Month,
                totalAmount,
                trade.PortfolioId,
                precalcSourceId,
                dailyDiffMonthShift,
                dailyDetails,
                security.StripName,
                calendarDaySwapSettlPriceDate);

            _log.Trace(
                "CalculatePositions: Adding calculation detail [{0}] for trade [{1}]",
                calculationDetail.Key,
                trade.TradeId);

            return calculationDetail;
        }

        private Product GetSourceProduct(SecurityDefinition security, Func<Product, bool> isProductValid)
        {
            Product sourceProduct = _productsStorage.GetProduct(security.product_id ?? Product.DefaultId)
                                                    .ValueOr(Product.Default);

            return (sourceProduct.IsDefault || sourceProduct.Type == ProductType.Spot || !isProductValid(sourceProduct))
                ? Product.Default
                : sourceProduct;
        }

        private CoeffEntityId GetPrecalcSourceId(TradeCapture trade, SecurityDefinition security)
        {
            _log.Trace(
                "CalculatePositions: Trade [{0}] has precalc details? {1}",
                trade.TradeId,
                trade.PrecalcDetails.Any());

            return trade.PrecalcDetails.Any()
                ? new CoeffEntityId(CoeffEntity.TradeCapture, trade.TradeId)
                : new CoeffEntityId(CoeffEntity.SecurityDefinition, security.SecurityDefinitionId);
        }

        private int GetDailyDiffMonthShift(TradeCapture trade, bool isDailyProduct, Product sourceProduct)
        {
            if (!isDailyProduct)
            {
                return 0;
            }

            _log.Trace("CalculatePositions: Trade [{0}] is a daily product", trade.TradeId);
            return sourceProduct.DailyDiffMonthShift;

        }

        private Func<Product, bool> IsProductValid(int tradeId)
        {
            return (sourceProduct) =>
            {
                DateTime now = SystemTime.Now();

                if (now <= (sourceProduct.ValidTo ?? now) && now >= (sourceProduct.ValidFrom ?? now))
                {
                    return true;
                }

                _log.Trace("CalculatePositions: Product for trade [{0}] is no longer valid", tradeId);
                return false;
            };
        }

        private List<PrecalcDetail> GetPrecalcDetails(TradeCapture trade, SecurityDefinition securityDefinition)
        {
            var securityPrecalcs = TryGetSecurityPrecalcs();

            return securityPrecalcs.Any() ? securityPrecalcs : TryGetTradePrecalcs();

            List<PrecalcDetail> TryGetSecurityPrecalcs()
            {
                return securityDefinition?.PrecalcDetails?.Select(CopyPrecalc).ToList();
            }

            List<PrecalcDetail> TryGetTradePrecalcs()
            {
                return (trade?.PrecalcDetails?.Count ?? 0) > 0
                    ? trade.PrecalcDetails.Select(CopyPrecalc).ToList()
                    : new List<PrecalcDetail>();
            }
        }

        private static PrecalcDetail CopyPrecalc(PrecalcDetail precalcPos)
        {
            return new PrecalcDetail
            {
                Month = precalcPos.Month,
                ProductId = precalcPos.ProductId,
                DailyPositions = precalcPos.DailyPositions
            };
        }

        private List<CalculationDetailModel> CalculateDailyPositionForSecurityDefinition(
            SdQuantityModel sdQuantity,
            SecurityDefinition securityDefinition,
            Product sourceProduct,
            DateTime? riskDate,
            bool splitWeekends)
        {
            List<CalculationDetailModel> dailyPositions = new List<CalculationDetailModel>();
            List<PrecalcDetail> daysPositions = GetPrecalcDetails(null, securityDefinition);

            if (daysPositions == null)
            {
                return dailyPositions;
            }

            foreach (PrecalcDetail precalcPos in daysPositions)
            {
                Product product = _productsStorage.TryGetProduct(precalcPos.ProductId).Value;
                DateTime productRiskDate = product.GetRiskDate(riskDate);
                IEnumerable<DateTime> precalcDays = precalcPos.DailyPositions.Keys;

                List<List<DateTime>> daysList = splitWeekends
                    ? GetSplitWeekendsDaysList(product, precalcDays, productRiskDate)
                    : GetDaysList(product, precalcDays, productRiskDate);

                List<CalculationDetailModel> dailyPos = CalculatePositionsForPrecalc(
                    sdQuantity,
                    securityDefinition,
                    sourceProduct,
                    splitWeekends,
                    daysList,
                    precalcPos,
                    product);

                dailyPositions.AddRange(dailyPos);
            }

            return dailyPositions;
        }

        private List<CalculationDetailModel> CalculatePositionsForPrecalc(
            SdQuantityModel sdQuantity,
            SecurityDefinition securityDefinition,
            Product sourceProduct,
            bool splitWeekends,
            List<List<DateTime>> daysList,
            PrecalcDetail precalcDetail,
            Product product)
        {
            return daysList.SelectMany(
                               days =>
                               {
                                   decimal amount = precalcDetail
                                                    .DailyPositions.Where(dayPos => days.Contains(dayPos.Key))
                                                    .Sum(dayPos => dayPos.Value);

                                   if (amount == 0M)
                                   {
                                       return Enumerable.Empty<CalculationDetailModel>();
                                   }

                                   return CalculateDailyPositions(
                                       sdQuantity,
                                       securityDefinition,
                                       sourceProduct,
                                       splitWeekends,
                                       precalcDetail,
                                       product,
                                       days);
                               })
                           .ToList();
        }

        private IEnumerable<CalculationDetailModel> CalculateDailyPositions(
            SdQuantityModel sdQuantity,
            SecurityDefinition securityDefinition,
            Product sourceProduct,
            bool splitWeekends,
            PrecalcDetail precalcDetail,
            Product product,
            List<DateTime> days)
        {
            return days.Select(
                day => new CalculationDetailModel(
                    _calculationDetailIdentifierService,
                    product,
                    sourceProduct,
                    precalcDetail.Month,
                    day,
                    precalcDetail.DailyPositions[day] * sdQuantity.TradesQuantity,
                    sdQuantity.PortfolioId,
                    new CoeffEntityId(CoeffEntity.SecurityDefinition, sdQuantity.SecurityDefinitionId),
                    0,
                    new List<DailyDetail>(),
                    securityDefinition.StripName,
                    splitWeekends ? GetSwapSettleDate(product, days) : default(DateTime?)));
        }
    }
}