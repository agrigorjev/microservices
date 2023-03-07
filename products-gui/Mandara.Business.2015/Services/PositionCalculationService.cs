using JetBrains.Annotations;
using Mandara.Business.Contracts;
using Mandara.Business.Extensions;
using Mandara.Business.Model;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.Services;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Services
{
    [Obsolete("Use PositionCalculator")]
    public class PositionCalculationService : IPositionCalculationService
    {
        private readonly IProductsStorage _productsStorage;
        private readonly ISecurityDefinitionsStorage _securityDefinitionsStorage;
        private readonly ILogger _log;
        private readonly ICalculationDetailIdentifierService _calculationDetailIdentifierService;

        public PositionCalculationService(
            [NotNull] IProductsStorage productsStorage,
            [NotNull] ISecurityDefinitionsStorage securityDefinitionsStorage,
            [NotNull] ILogger log,
            ICalculationDetailIdentifierService calculationDetailIDService)
        {
            if (productsStorage == null)
                throw new ArgumentNullException("productsStorage");
            if (securityDefinitionsStorage == null)
                throw new ArgumentNullException("securityDefinitionsStorage");
            if (log == null)
                throw new ArgumentNullException("log");

            _productsStorage = productsStorage;
            _securityDefinitionsStorage = securityDefinitionsStorage;
            _log = log;
            _calculationDetailIdentifierService = calculationDetailIDService;
        }

        public List<CalculationDetailModel> CalculatePositions(
            List<TradeModel> trades,
            List<SdQuantityModel> sdQuantities,
            DateTime? riskDateParam = null,
            bool splitWeekends = false)
        {
            List<CalculationDetailModel> result = new List<CalculationDetailModel>();
            DateTime riskDate = riskDateParam ?? SystemTime.Now();
            DateTime riskDay = riskDate.Date;

            foreach (TradeModel trade in trades)
            {
                if (trade.IsParentTrade == false)
                {
                    continue;
                }

                result.AddRange(CalculatePositionForTradeModel(trade, riskDate, splitWeekends));
            }

            foreach (SdQuantityModel sdQuantity in sdQuantities)
            {
                SecurityDefinition securityDefinition =
                    _securityDefinitionsStorage.TryGetSecurityDefinition(sdQuantity.SecurityDefinitionId).Value;
                Product sourceProduct = _productsStorage.TryGetProduct(securityDefinition.product_id.Value).Value;

                if (sourceProduct.Type == ProductType.Spot)
                {
                    continue;
                }

                result.AddRange(
                    CalculatePositionForSecurityDefinition(sdQuantity, securityDefinition, sourceProduct, riskDate, splitWeekends));
            }

            return result;
        }

        //private void LogZeroPositions(List<CalculationDetailModel> positions, string prefix)
        //{
        //    _log.Debug(
        //        "{0} with 0 position: {1}",
        //        prefix,
        //        String.Join(
        //            ", ",
        //            positions.OrderBy(pos => pos.ProductId)
        //                     .ThenBy(pos => pos.SourceProductId)
        //                     .Select(
        //                         pos =>
        //                             $"{pos.Product} - {pos.ProductId} - {pos.SourceProductId} - {pos.StripName} - {pos.PositionFactor}")
        //                     .Distinct()));
        //}

        private List<CalculationDetailModel> CalculatePositionForTradeModel(
            TradeModel trade,
            DateTime? riskDate,
            bool splitWeekends = false)
        {
            List<CalculationDetailModel> result = new List<CalculationDetailModel>();
            SecurityDefinition securityDefinition =
                _securityDefinitionsStorage.TryGetSecurityDefinition(trade.SecurityDefinitionId).Value;
            Product sourceProduct = _productsStorage.TryGetProduct(securityDefinition.product_id.Value).Value;
            bool isDailyProduct = sourceProduct.IsProductDaily;
            int dailyDiffMonthShift = 0;

            if (isDailyProduct)
            {
                dailyDiffMonthShift = sourceProduct.DailyDiffMonthShift;
            }

            foreach (PrecalcDetailModel precalcDetail in trade.PrecalcDetails)
            {
                Product product = _productsStorage.TryGetProduct(precalcDetail.ProductId).Value;
                DateTime productRiskDate = product.GetRiskDate(riskDate);

                // There should be no need to go through the process of deserialising the precalc details if the contract
                // has already expired.
                //if (productRiskDate > precalcDetail.MaxDay)
                //{
                //    continue;
                //}

                Dictionary<DateTime, decimal?> daysPositions =
                    DayPositionsSerialisation.DeserializeDaysNullablePositions(
                        precalcDetail.DaysSerialized,
                        precalcDetail.Month,
                        precalcDetail.ProductId,
                        "TradeCapture",
                        trade.TradeId);

                List<List<DateTime>> daysList = GetDaysList(
                    product,
                    daysPositions.Keys,
                    splitWeekends,
                    productRiskDate);

                if (daysList.Count == 0)
                {
                    continue;
                }

                foreach (List<DateTime> days in daysList)
                {
                    decimal totalAmount = 0;
                    List<DailyDetail> dailyDetails = new List<DailyDetail>();

                    foreach (DateTime posDate in days)
                    {
                        decimal dayAmount = daysPositions[posDate] ?? 0M;

                        totalAmount += dayAmount;

                        if (isDailyProduct)
                        {
                            dailyDetails.Add(new DailyDetail(posDate, dayAmount * trade.Quantity.Value));
                        }
                    }

                    if (totalAmount == 0M && dailyDetails.Count == 0)
                    {
                        continue;
                    }

                    totalAmount = totalAmount * trade.Quantity.Value;

                    // if it was split then we need to remember date for Settlement price
                    DateTime? calendarDaySwapSettlPriceDate = null;

                    if (product.IsCalendarDaySwap && splitWeekends && days.Count() == 1
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
                        new CoeffEntityId(CoeffEntity.TradeCapture, trade.TradeId),
                        dailyDiffMonthShift,
                        dailyDetails,
                        StripHelper.GetBalmoStripNameWithDate(
                                securityDefinition.StripName,
                                trade.TradeStartDate),
                        calendarDaySwapSettlementPriceDate: calendarDaySwapSettlPriceDate);

                    result.Add(calculationDetail);
                }
            }

            return result;
        }


        private List<CalculationDetailModel> CalculatePositionForSecurityDefinition(
            SdQuantityModel sdQuantity,
            SecurityDefinition securityDefinition,
            Product sourceProduct,
            DateTime? riskDate,
            bool splitWeekends)
        {
            List<CalculationDetailModel> result = new List<CalculationDetailModel>();
            List<PrecalcDetail> daysPositions = GetPrecalcDetails(null, securityDefinition);

            if (daysPositions == null)
            {
                return result;
            }

            foreach (PrecalcDetail precalcDetail in daysPositions)
            {
                Product product = _productsStorage.TryGetProduct(precalcDetail.ProductId).Value;
                DateTime productRiskDate = product.GetRiskDate(riskDate);

                List<List<DateTime>> daysList = GetDaysList(
                    product,
                    precalcDetail.DaysPositions.Select(p => p.Key),
                    splitWeekends,
                    productRiskDate);

                foreach (List<DateTime> days in daysList)
                {
                    decimal amount = precalcDetail.DaysPositions.Where(p => days.Contains(p.Key))
                                                  .Sum(x => x.Value ?? 0M);

                    if (amount == 0M)
                    {
                        continue;
                    }

                    amount = amount * sdQuantity.TradesQuantity;

                    // if it was split then we need to remember date for Settlement price
                    DateTime? calendarDaySwapSettlPriceDate = null;

                    if (product.IsCalendarDaySwap && splitWeekends && days.Count() == 1 &&
                        (days[0].DayOfWeek == DayOfWeek.Sunday || days[0].DayOfWeek == DayOfWeek.Saturday))
                    {
                        calendarDaySwapSettlPriceDate = days[0];
                    }

                    CalculationDetailModel calculationDetail = new CalculationDetailModel(
                        _calculationDetailIdentifierService,
                        product,
                        sourceProduct,
                        precalcDetail.Month.Year,
                        precalcDetail.Month.Month,
                        amount,
                        sdQuantity.PortfolioId,
                        new CoeffEntityId(CoeffEntity.SecurityDefinition, sdQuantity.SecurityDefinitionId),
                        0,
                        null,
                        securityDefinition.StripName,
                        calendarDaySwapSettlPriceDate);

                    result.Add(calculationDetail);
                }
            }

            return result;
        }

        private List<List<DateTime>> GetDaysList(Product product, IEnumerable<DateTime> days, bool splitWeekends, DateTime productRiskDate)
        {
            List<List<DateTime>> result = new List<List<DateTime>>();
            if (product.IsCalendarDaySwap && splitWeekends)
            {
                // separate regular business days from weekends
                List<DateTime> businessDays = new List<DateTime>();

                foreach (var p in days.Where(p => p > productRiskDate))
                {
                    if (p.DayOfWeek == DayOfWeek.Saturday || p.DayOfWeek == DayOfWeek.Sunday)
                    {
                        result.Add(new List<DateTime>() { p });
                    }
                    else
                    {
                        businessDays.Add(p);
                    }
                }

                if (businessDays.Any())
                {
                    result.Add(businessDays);
                }
            }
            else
            {
                result.Add(days.Where(it => it > productRiskDate).ToList());
            }

            return result;
        }

        public List<CalculationDetailModel> CalculatePositions(
            [NotNull] TradeCapture trade,
            DateTime? riskDateParam = null,
            bool checkProductValidation = false,
            bool splitWeekends = false)
        {
            List<CalculationDetailModel> result = new List<CalculationDetailModel>();
            SecurityDefinition securityDefinition = trade.SecurityDefinition
                                                    ?? _securityDefinitionsStorage.TryGetSecurityDefinition(
                                                        trade.SecurityDefinitionId).Value;
            List<PrecalcDetail> daysPositions = GetPrecalcDetails(trade, securityDefinition);

            if (daysPositions == null)
            {
                _log.Trace("CalculatePositions: No precalc details day positions found for trade [{0}]", trade.TradeId);
                return result;
            }

            Product sourceProduct = _productsStorage.TryGetProduct(securityDefinition.product_id.Value).Value;
            DateTime riskDate = riskDateParam ?? SystemTime.Now();
            DateTime riskDay = riskDate.Date;

            if (sourceProduct.Type == ProductType.Spot)
            {
                return result;
            }

            if (checkProductValidation)
            {
                DateTime now = SystemTime.Now();

                if (sourceProduct.ValidTo.HasValue && now > sourceProduct.ValidTo
                    || sourceProduct.ValidFrom.HasValue && now < sourceProduct.ValidFrom)
                {
                    _log.Trace(
                        "CalculatePositions: Checking product for trade [{0}] valid failed - source product date "
                            + "range does not include now",
                        trade.TradeId);
                    return result;
                }
            }

            CoeffEntity coeffEntity = CoeffEntity.SecurityDefinition;
            int entityId = securityDefinition.SecurityDefinitionId;

            _log.Trace(
                "CalculatePositions: Trade [{0}] has precalc details? {1}",
                trade.TradeId,
                trade.PrecalcDetails.Any());

            if (trade.PrecalcDetails.Any())
            {
                coeffEntity = CoeffEntity.TradeCapture;
                entityId = trade.TradeId;
            }

            var coeffEntityId = new CoeffEntityId(coeffEntity, entityId);
            bool isDailyProduct = sourceProduct.IsProductDaily;

            int dailyDiffMonthShift = 0;

            if (isDailyProduct)
            {
                _log.Trace("CalculatePositions: Trade [{0}] is a daily product", trade.TradeId);

                dailyDiffMonthShift = sourceProduct.DailyDiffMonthShift;
            }

            foreach (PrecalcDetail precalcDetail in daysPositions)
            {
                Product product = _productsStorage.TryGetProduct(precalcDetail.ProductId).Value;
                DateTime productRiskDate = product.GetRiskDate(riskDate);

                _log.Trace(
                    "CalculatePositions: precalc detail for trade [{0}], product [{1}] and risk date[{2}]",
                    trade.TradeId,
                    product.ProductId,
                    riskDate);

                List<List<DateTime>> daysList = GetDaysList(
                        product,
                        precalcDetail.DaysPositions.Keys,
                        splitWeekends,
                        productRiskDate);

                _log.Trace(
                    "CalculatePositions: precalc detail for trade [{0}] has {1} days in its list, [{2}]",
                    trade.TradeId,
                    daysList.Count,
                    String.Join(";", precalcDetail.DaysPositions.Keys.Select(key => key.ToString())));

                foreach (var days in daysList)
                {
                    decimal totalAmount = 0;
                    List<DailyDetail> dailyDetails = new List<DailyDetail>();

                    foreach (var p in days)
                    {
                        decimal dayAmount = precalcDetail.DaysPositions[p] ?? 0M;
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
                        continue;
                    }

                    totalAmount = totalAmount * trade.Quantity.Value;

                    // if it was split then we need to remember date for Settlement price
                    DateTime? calendarDaySwapSettlPriceDate = null;

                    if (product.IsCalendarDaySwap && splitWeekends && days.Count() == 1 &&
                        (days[0].DayOfWeek == DayOfWeek.Sunday || days[0].DayOfWeek == DayOfWeek.Saturday))
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
                        coeffEntityId,
                        dailyDiffMonthShift,
                        dailyDetails,
                        securityDefinition.StripName,
                        calendarDaySwapSettlPriceDate);

                    _log.Trace(
                        "CalculatePositions: Adding calculation detail [{0}] for trade [{1}]",
                        calculationDetail.Key,
                        trade.TradeId);
                    result.Add(calculationDetail);
                }
            }

            return result;
        }

        private List<PrecalcDetail> GetPrecalcDetails(TradeCapture trade, SecurityDefinition securityDefinition)
        {
            List<PrecalcDetail> daysPositions = null;

            if (securityDefinition != null && securityDefinition.PrecalcDetails != null &&
                securityDefinition.PrecalcDetails.Count > 0)
            {
                daysPositions = securityDefinition.PrecalcDetails
                    .Select(x => new PrecalcDetail
                    {
                        Month = x.Month,
                        ProductId = x.ProductId,
                        DaysPositions = x.DaysPositions
                    }).ToList();
            }

            if (daysPositions == null)
            {
                if (trade != null && trade.PrecalcDetails != null && trade.PrecalcDetails.Count > 0)
                {
                    daysPositions = trade.PrecalcDetails
                        .Select(x => new PrecalcDetail
                        {
                            Month = x.Month,
                            ProductId = x.ProductId,
                            DaysPositions = x.DaysPositions
                        }).ToList();
                }
            }

            return daysPositions;
        }
    }
}
