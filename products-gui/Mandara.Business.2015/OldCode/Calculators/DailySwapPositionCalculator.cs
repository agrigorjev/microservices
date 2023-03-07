using Mandara.Business.Calculators;
using Mandara.Business.Calculators.Daily;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Ninject.Extensions.Logging;

namespace Mandara.Business.OldCode.Calculators
{
    internal class DailySwapPositionCalculator : PositionCalculatorBase
    {
        private readonly ILogger _logger = new NLogLoggerFactory().GetCurrentClassLogger();

        private struct DateScaleFactor
        {
            public DateTime ProductDate { get; private set; }
            public List<DateTime> BusinessDays { get; private set; }
            public int BusinessDaysElapsed { get; private set; }
            public decimal ScaleFactor { get; private set; }

            public DateScaleFactor(DateTime prodDate, List<DateTime> busDays, int busDaysElapsed, decimal scale)
            {
                ProductDate = prodDate;
                BusinessDays = busDays;
                BusinessDaysElapsed = busDaysElapsed;
                ScaleFactor = scale;
            }
        }

        public DailySwapPositionCalculator(CalculationManager calculationManager) : base(calculationManager)
        {
        }

        public override List<CalculationDetail> CalculateFirstLeg(SourceDetail trade, CalculationContext context)
        {
            return Calculate(trade, context);
        }

        public override List<CalculationDetail> Calculate(
            SourceDetail sourceDetail,
            CalculationContext calculationContext)
        {
            Product product = calculationContext.Product;
            Product sourceProduct = calculationContext.SourceProduct;
            ProductDateType productDateType = calculationContext.ProductDateType;
            int dailyDiffMonthShift = calculationContext.DailyDiffMonthShift;

            List<DateTime> businessDays;
            int businessDaysElapsed;
            List<CalculationDetail> calculationDetails = DoCalculate(
                sourceDetail,
                calculationContext,
                out businessDays,
                out businessDaysElapsed);

            foreach (CalculationDetail detail in calculationDetails.ToList())
            {
                detail.FillDailyDetails(businessDays, businessDaysElapsed);

                if (detail.DailyDetails == null || detail.DailyDetails.Count == 0)
                    continue;

                List<DateTime> months = detail.DailyDetails
                                              .Select(
                                                  d => new DateTime(d.CalculationDate.Year, d.CalculationDate.Month, 1))
                                              .Distinct()
                                              .OrderBy(x => x)
                                              .ToList();

                // if we don't span accross multiple months or it's a BFOE
                if (months.Count <= 1 || dailyDiffMonthShift > Product.NoDailyDiffMonthShift)
                {
                    continue;
                }

                calculationDetails.Remove(detail);

                for (int i = 0; i < months.Count; i++)
                {
                    DateTime productDate = detail.CalculationDate.AddMonths(i);

                    DateTime month = months[i];
                    List<DailyDetail> dailyDetails = detail.DailyDetails
                                                           .Where(
                                                               x => x.CalculationDate.Year == month.Year
                                                                    && x.CalculationDate.Month == month.Month)
                                                           .ToList();

                    decimal amountAtMonth = dailyDetails.Sum(x => x.Amount);

                    CalculationDetail calculation = CalculationDetail.Create(
                        sourceDetail,
                        product,
                        sourceProduct,
                        productDate.Year,
                        productDate.Month,
                        amountAtMonth,
                        productDate,
                        productDateType,
                        detail.StripName,
                        detail.Product,
                        detail.Source,
                        dailyDiffMonthShift);
                    calculation.DailyDetails = new ConcurrentBag<DailyDetail>(dailyDetails);

                    calculationDetails.Add(calculation);
                }
            }

            return calculationDetails;
        }

        private List<CalculationDetail> DoCalculate(
            SourceDetail sourceDetail,
            CalculationContext calculationContext,
            out List<DateTime> businessDays,
            out int businessDaysElapsed)
        {
            DateTime riskDate = calculationContext.RiskDate;
            Product product = calculationContext.Product;
            Product sourceProduct = calculationContext.SourceProduct;
            bool suppressContractSize = calculationContext.SuppressContractSize;
            bool suppressPosConvFactor = calculationContext.SuppressPositionConversionFactor;
            decimal qty = calculationContext.Quantity;
            ProductDateType productDateType = calculationContext.ProductDateType;
            CalculationCache cache = calculationContext.CalculationCache;
            int dailyDiffMonthShift = calculationContext.DailyDiffMonthShift;

            DateScaleFactor dateScaleFactor = GetDateScaleFactor(
                cache,
                product.HolidaysCalendar,
                riskDate,
                product,
                sourceProduct.Type,
                sourceDetail,
                dailyDiffMonthShift);
            decimal contractSizeFactor = suppressContractSize ? 1 : product.ContractSize;
            decimal positionFactor = suppressPosConvFactor ? 1 : product.PositionFactor ?? 1;
            decimal amountAtMonth = qty * positionFactor * dateScaleFactor.ScaleFactor * contractSizeFactor;

            CalculationDetail calculation = CalculationDetail.Create(
                sourceDetail,
                product,
                sourceProduct,
                dateScaleFactor.ProductDate.Year,
                dateScaleFactor.ProductDate.Month,
                amountAtMonth,
                dateScaleFactor.ProductDate,
                productDateType,
                sourceDetail.StripName,
                null,
                null,
                dailyDiffMonthShift);

            businessDays = dateScaleFactor.BusinessDays;
            businessDaysElapsed = dateScaleFactor.BusinessDaysElapsed;
            return new List<CalculationDetail> { calculation };
        }

        private DateScaleFactor GetDateScaleFactor(
            CalculationCache cache,
            StockCalendar calendar,
            DateTime riskDate,
            Product product,
            ProductType sourceProductType,
            SourceDetail sourceDetail,
            int dailyDiffMonthShift)
        {
            riskDate = CalculationManager.AdjustRiskDateForProduct(riskDate, product);

            DateTime currentMonthStart = GetProductCurrentMonth(sourceProductType, sourceDetail);
            DateTime currentMonthEnd = GetTradeEndDate(sourceDetail);
            DateTime productDate =
                new DateTime(currentMonthStart.Year, currentMonthStart.Month, 1).AddMonths(dailyDiffMonthShift);

            List<DateTime> holidays = cache
                .HolidaysBetweenDates(calendar.CalendarId, currentMonthStart, currentMonthEnd, true)
                .Select(it => it.HolidayDate)
                .ToList();
            int holidaysTotal = holidays.Count;

            riskDate = CalculationManager.AdjustRiskDateForHoliday(riskDate, holidays);

            int holidaysBeforeRiskDate = cache.CountHolidaysBetweenDates(
                calendar.CalendarId,
                currentMonthStart,
                riskDate,
                false);

            int businessDaysElapsed = CalculationManager.GetBusinessDays(currentMonthStart, riskDate)
                                      - holidaysBeforeRiskDate
                                      - 1;
            Int32 businessDaysTotal = CalculationManager.GetBusinessDays(currentMonthStart, currentMonthEnd)
                                      - holidaysTotal;

            if (businessDaysElapsed > businessDaysTotal)
            {
                businessDaysElapsed = businessDaysTotal;
            }

            List<DateTime> businessDays =
                CalculationManager.GetBusinessDaysArray(currentMonthStart, currentMonthEnd, holidays);

            if (sourceDetail.BusinessDaysNotSet())
            {
                sourceDetail.SetBusinessDaysAndElapsed(businessDays, businessDaysElapsed);
                sourceDetail.TradeCapture?.SetBusinessDaysAndElapsed(businessDays, businessDaysElapsed);
            }

            if (businessDaysTotal == 0M)
            {
                return new DateScaleFactor(productDate, businessDays, businessDaysElapsed, 0M);
            }

            decimal dateScaleFactor = (Decimal)(businessDaysTotal - businessDaysElapsed) / businessDaysTotal;

            if (dateScaleFactor > 1M)
            {
                dateScaleFactor = 1M;
            }

            if (dateScaleFactor < 0M)
            {
                dateScaleFactor = 0M;
            }

            DateTime currentMonthEndWithTime = new DateTime(
                currentMonthEnd.Year,
                currentMonthEnd.Month,
                currentMonthEnd.Day,
                23,
                59,
                59);

            if (riskDate > currentMonthEndWithTime)
            {
                dateScaleFactor = 0M;
            }

            if (riskDate < currentMonthStart)
            {
                dateScaleFactor = 1M;
            }

            DateTime riskMonth = new DateTime(riskDate.Year, riskDate.Month, 1);
            if (dailyDiffMonthShift == 0 && riskMonth > productDate)
            {
                _logger.Trace(
                    "ProductDate {0} overwritten to {1} for date scale factor {2} on product '{3}' on source detail {4}",
                    productDate,
                    riskMonth,
                    dateScaleFactor,
                    product.Name,
                    sourceDetail.SourceDetailId);
                productDate = riskMonth;
            }
            
            return new DateScaleFactor(productDate, businessDays, businessDaysElapsed, dateScaleFactor);
        }

        private DateTime GetProductCurrentMonth(ProductType sourceProductType, SourceDetail sourceDetail)
        {
            return sourceDetail.TradeCapture != null
                ? GetProductCurrentMonth(sourceDetail.TradeCapture, sourceProductType)
                : sourceDetail.ProductDate;
        }

        private static DateTime GetTradeEndDate(SourceDetail sourceDetail)
        {
            return TradeHasEndDate(sourceDetail)
                ? sourceDetail.TradeCapture.TradeEndDate.Value
                : sourceDetail.ProductDate;
        }

        private static bool TradeHasEndDate(SourceDetail sourceDetail)
        {
            return sourceDetail.TradeCapture != null && sourceDetail.TradeCapture.TradeEndDate.HasValue;
        }

        private DateTime GetProductCurrentMonth(TradeCapture trade, ProductType sourceProductType)
        {
            DateTime tradeStartDate = trade.TradeStartDate.Value;

            if (ProductType.DayVsMonthFullWeek == sourceProductType && DayOfWeek.Monday != tradeStartDate.DayOfWeek)
            {
                if (tradeStartDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    return tradeStartDate.AddDays(-6);
                }
                else
                {
                    return tradeStartDate.AddDays(-1 * (int)tradeStartDate.DayOfWeek + 1);
                }
            }

            return tradeStartDate;
        }

        public override List<CalculationDetail> CalculateSecondLeg(SourceDetail trade, CalculationContext context)
        {
            if (ProductType.DailyVsDaily == context.SourceProduct.Type)
            {
                Expiry expiry = CalculationManager.TryGetExpiryInRange(
                    context.CalculationCache,
                    context.Product,
                    trade.BusinessDays1);

                return expiry.HasDate()
                    ? CalculateDailyVsDaily(trade, context, expiry.Expires)
                    : Calculate(trade, context);
            }

            return Calculate(trade, context);
        }

        private List<CalculationDetail> CalculateDailyVsDaily(
            SourceDetail trade,
            CalculationContext context,
            DateTime inRangeExpiry)
        {
            SplitTradeAtExpiry tradeSplitting = new SplitTradeAtExpiry(trade, context, inRangeExpiry);

            List<CalculationDetail> positionsBeforeExpiry = GetPositionsBeforeExpiry(trade, context, tradeSplitting);
            List<CalculationDetail> positionsAfterExpiry = tradeSplitting.HasDatesAfterExpiry
                ? GetPositionsAfterExpiry(trade, context, tradeSplitting)
                : new List<CalculationDetail>();

            return positionsBeforeExpiry.Concat(positionsAfterExpiry).ToList();
        }

        private List<CalculationDetail> GetPositionsBeforeExpiry(
            SourceDetail trade,
            CalculationContext context,
            SplitTradeAtExpiry tradeSplitting)
        {
            trade.BusinessDays1 = tradeSplitting.DaysBeforeExpiry.ToList();
            trade.TradeCapture.Quantity = tradeSplitting.TradeQuantityBeforeExpiry;
            context.Quantity = tradeSplitting.CalculationQuantityBeforeExpiry;
            trade.TradeCapture.TradeEndDate = tradeSplitting.EndDateTradeBeforeExpiry;
            trade.TradeEndDate = trade.TradeCapture.TradeEndDate.Value;

            List<CalculationDetail> positionsBeforeExpiry = Calculate(trade, context);
            return positionsBeforeExpiry;
        }

        private List<CalculationDetail> GetPositionsAfterExpiry(
            SourceDetail trade,
            CalculationContext context,
            SplitTradeAtExpiry tradeSplitting)
        {
            trade.BusinessDays1 = tradeSplitting.DaysFromExpiry.ToList();
            trade.TradeCapture.Quantity = tradeSplitting.TradeQuantityFromExpiry;
            context.Quantity = tradeSplitting.CalculationQuantityFromExpiry;
            trade.TradeCapture.TradeStartDate = tradeSplitting.StartDateTradeFromExpiry;
            trade.TradeCapture.TradeEndDate = tradeSplitting.EndDateTradeAfterExpiry;
            context.RiskDate = trade.TradeCapture.TradeStartDate.Value;
            context.ProductMonth += context.ProductMonth == 12 ? -11 : 1;
            context.ProductYear += context.ProductMonth == 1 ? 1 : 0;

            List<CalculationDetail> positionsAfterExpiry = Calculate(trade, context);

            positionsAfterExpiry.ForEach(
                positions => positions.CalculationDate = positions.CalculationDate.AddMonths(1));
            RestoreOriginalTradeData(trade, context, tradeSplitting);
            return positionsAfterExpiry;
        }

        private void RestoreOriginalTradeData(
            SourceDetail trade,
            CalculationContext context,
            SplitTradeAtExpiry tradeSplitting)
        {
            trade.TradeCapture.Quantity = tradeSplitting.FullTradeQuantity;
            context.Quantity = tradeSplitting.FullCalculationQuantity;
            trade.TradeCapture.TradeStartDate = tradeSplitting.OriginalBusinessDays.First();
            trade.BusinessDays1 = tradeSplitting.OriginalBusinessDays;
            context.RiskDate = trade.TradeCapture.TradeStartDate.Value;
            context.ProductMonth -= context.ProductMonth == 1 ? -11 : 1;
            context.ProductYear -= context.ProductMonth == 12 ? 1 : 0;
        }
    }
}