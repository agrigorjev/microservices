using Mandara.Business.Contracts;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.OldCode.Calculators
{
    internal class TradeMonthSwapPositionCalculator : SwapPositionCalculator
    {
        private readonly IProductsStorage _productStorage;

        public TradeMonthSwapPositionCalculator(CalculationManager calculationManager, IProductsStorage productStorage)
            : base(calculationManager)
        {
            _productStorage = productStorage;
        }

        public override List<CalculationDetail> Calculate(
            SourceDetail sourceDetail,
            CalculationContext calculationContext)
        {
            DateTime riskDate = calculationContext.RiskDate;
            int productYear = calculationContext.ProductYear;
            int productMonth = calculationContext.ProductMonth;
            Product product = calculationContext.Product;
            Product sourceProduct = calculationContext.SourceProduct;
            bool suppressRolloff = calculationContext.SuppressRolloff;
            bool suppressContractSize = calculationContext.SuppressContractSize;
            bool suppressPosConvFactor = calculationContext.SuppressPositionConversionFactor;
            decimal qty = calculationContext.Quantity;
            decimal crudeSwapFactor = calculationContext.CrudeSwapFactor ?? 1M;
            ProductDateType productDateType = calculationContext.ProductDateType;
            CalculationCache cache = calculationContext.CalculationCache;

            DateTime productDate = new DateTime(productYear, productMonth, 1);

            decimal dateScaleFactor = 1;
            int currentYear = riskDate.Year;
            int currentMonth = riskDate.Month;

            int nextYear = riskDate.AddMonths(1).Year;
            int nextMonth = riskDate.AddMonths(1).Month;

            int prevProductYear = productDate.AddMonths(-1).Year;
            int prevProductMonth = productDate.AddMonths(-1).Month;
            DateTime prevProductDate = new DateTime(productYear, productMonth, 1);

            DateTime? value = _productStorage.GetExpiryDate(product, prevProductYear, prevProductMonth);
            if (value == null)
                return new List<CalculationDetail>();
            DateTime startExpiryDate = value.Value;

            value = _productStorage.GetExpiryDate(product, productYear, productMonth);
            if (value == null)
                return new List<CalculationDetail>();
            DateTime endExpiryDate = value.Value;

            List<DateTime> holidays = new List<DateTime>();
            holidays =
                cache.HolidaysBetweenDates(product.HolidaysCalendar.CalendarId, startExpiryDate, endExpiryDate, true)
                    .Select(it => it.HolidayDate)
                    .ToList();

            // take next working day 
            startExpiryDate = CalculationManager.AdjustRiskDateForHoliday(startExpiryDate.AddDays(1), holidays);

            decimal contractSizeFactor = suppressContractSize ? 1 : product.ContractSize;
            decimal positionFactor = suppressPosConvFactor
                ? 1
                : product.PositionFactor == null ? 1 : product.PositionFactor.Value;

            List<CalculationDetail> result = new List<CalculationDetail>();
            CalculationDetail calculation;
            if (!product.IsEnabledRiskDecomposition
                || startExpiryDate.Year == endExpiryDate.Year && startExpiryDate.Month == endExpiryDate.Month)
            {
                // all period is in the same month

                if (((productYear == currentYear && productMonth == currentMonth) ||
                     (productYear == nextYear && productMonth == nextMonth && product.UseExpiryCalendar.HasValue &&
                      product.UseExpiryCalendar.Value)) && !suppressRolloff)
                {
                    // calculate coefficient (between 0 and 1) of amount that left for the given month
                    dateScaleFactor = GetDateScaleFactorForCustomPeriod(
                        cache,
                        product.HolidaysCalendar,
                        riskDate,
                        startExpiryDate,
                        endExpiryDate,
                        product,
                        sourceDetail);
                }

                decimal amountAtMonth = qty * positionFactor *
                                        dateScaleFactor * crudeSwapFactor * contractSizeFactor;

                calculation = CalculationDetail.Create(
                    sourceDetail,
                    product,
                    sourceProduct,
                    productYear,
                    productMonth,
                    amountAtMonth,
                    productDate,
                    productDateType,
                    sourceDetail.StripName,
                    null,
                    null,
                    Product.NoDailyDiffMonthShift);
                result.Add(calculation);
            }
            else
            {
                // period in two months
                holidays =
                    cache.HolidaysBetweenDates(
                        product.HolidaysCalendar.CalendarId,
                        startExpiryDate,
                        endExpiryDate,
                        true)
                        .Select(it => it.HolidayDate)
                        .ToList();
                int holidaysTotal = holidays.Count;

                riskDate = CalculationManager.AdjustRiskDateForProduct(riskDate, product);
                riskDate = CalculationManager.AdjustRiskDateForHoliday(riskDate, holidays);

                int holidaysBeforeRiskDate = cache.CountHolidaysBetweenDates(
                    product.HolidaysCalendar.CalendarId,
                    startExpiryDate,
                    riskDate,
                    false);

                int businessDaysTotal = CalculationManager.GetBusinessDays(startExpiryDate, endExpiryDate) -
                                        holidaysTotal;

                int businessDaysElapsed = CalculationManager.GetBusinessDays(startExpiryDate, riskDate) -
                                          holidaysBeforeRiskDate - 1;

                if (riskDate.Year == endExpiryDate.Year && riskDate.Month == endExpiryDate.Month)
                {
                    if (businessDaysElapsed > businessDaysTotal)
                        businessDaysElapsed = businessDaysTotal;

                    decimal localDateScaleFactor = (decimal)(businessDaysTotal - businessDaysElapsed)
                                                   / businessDaysTotal;

                    localDateScaleFactor = AlignDateScaleFactor(
                        localDateScaleFactor,
                        riskDate,
                        startExpiryDate,
                        endExpiryDate);

                    decimal amountAtMonth = qty * positionFactor *
                                            localDateScaleFactor * crudeSwapFactor * contractSizeFactor;

                    calculation = CalculationDetail.Create(
                        sourceDetail,
                        product,
                        sourceProduct,
                        productYear,
                        productMonth,
                        amountAtMonth,
                        productDate,
                        productDateType,
                        sourceDetail.StripName,
                        null,
                        null,
                        Product.NoDailyDiffMonthShift);
                    result.Add(calculation);
                }
                else
                {
                    DateTime lastMonth = new DateTime(endExpiryDate.Year, endExpiryDate.Month, 1);

                    int holidaysInLastMonth = cache.CountHolidaysBetweenDates(
                        product.HolidaysCalendar.CalendarId,
                        lastMonth,
                        endExpiryDate,
                        false);
                    int businessDaysInLastMonth = CalculationManager.GetBusinessDays(lastMonth, endExpiryDate) -
                                                  holidaysInLastMonth;

                    // Qty for prev month
                    decimal localDateScaleFactor =
                        (decimal)(businessDaysTotal - businessDaysInLastMonth - businessDaysElapsed)
                        / businessDaysTotal;

                    localDateScaleFactor = AlignDateScaleFactor(
                        localDateScaleFactor,
                        riskDate,
                        startExpiryDate,
                        lastMonth.AddDays(-1));
                    if (localDateScaleFactor > 0M)
                    {
                        decimal amountAtMonthPrev = qty * positionFactor *
                                                    localDateScaleFactor * crudeSwapFactor * contractSizeFactor;

                        calculation = CalculationDetail.Create(
                            sourceDetail,
                            product,
                            sourceProduct,
                            prevProductYear,
                            prevProductMonth,
                            amountAtMonthPrev,
                            prevProductDate,
                            productDateType,
                            sourceDetail.StripName,
                            null,
                            null,
                            Product.NoDailyDiffMonthShift);
                        result.Add(calculation);
                    }

                    // Qty for current month
                    localDateScaleFactor = (decimal)businessDaysInLastMonth / businessDaysTotal;

                    localDateScaleFactor = AlignDateScaleFactor(
                        localDateScaleFactor,
                        riskDate,
                        startExpiryDate,
                        endExpiryDate);
                    decimal amountAtMonth = qty * positionFactor *
                                            localDateScaleFactor * crudeSwapFactor * contractSizeFactor;

                    calculation = CalculationDetail.Create(
                        sourceDetail,
                        product,
                        sourceProduct,
                        productYear,
                        productMonth,
                        amountAtMonth,
                        productDate,
                        productDateType,
                        sourceDetail.StripName,
                        null,
                        null,
                        Product.NoDailyDiffMonthShift);
                    result.Add(calculation);
                }
            }
            return result;
        }

        private decimal AlignDateScaleFactor(
            decimal dateScaleFactor,
            DateTime currentDate,
            DateTime startDate,
            DateTime endDate)
        {
            if (dateScaleFactor > 1M)
                dateScaleFactor = 1M;

            if (dateScaleFactor < 0M)
                dateScaleFactor = 0M;

            DateTime endDateWithTime = new DateTime(
                endDate.Year,
                endDate.Month,
                endDate.Day,
                23,
                59,
                59);

            if (currentDate > endDateWithTime)
                dateScaleFactor = 0M;

            if (currentDate < startDate)
                dateScaleFactor = 1M;

            return dateScaleFactor;
        }
    }
}