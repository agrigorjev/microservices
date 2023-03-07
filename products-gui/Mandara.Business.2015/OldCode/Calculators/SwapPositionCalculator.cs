using Mandara.Entities;
using Mandara.Entities.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.OldCode.Calculators
{
    internal class SwapPositionCalculator : PositionCalculatorBase
    {
        public SwapPositionCalculator(CalculationManager calculationManager) : base(calculationManager)
        { }

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

            if (((productYear == currentYear && productMonth == currentMonth) ||
                 (productYear == nextYear && productMonth == nextMonth && product.UseExpiryCalendar.HasValue &&
                  product.UseExpiryCalendar.Value)) && !suppressRolloff)
            {
                // calculate coefficient (between 0 and 1) of amount that left for the given month
                dateScaleFactor = GetDateScaleFactor(
                    cache,
                    product.HolidaysCalendar,
                    riskDate,
                    productYear,
                    productMonth,
                    product,
                    sourceDetail);
            }

            decimal contractSizeFactor = suppressContractSize ? 1 : product.ContractSize;
            decimal positionFactor = suppressPosConvFactor
                ? 1
                : product.PositionFactor == null ? 1 : product.PositionFactor.Value;

            decimal amountAtMonth = qty * positionFactor *
                                    dateScaleFactor * crudeSwapFactor * contractSizeFactor;

            CalculationDetail calculation = CalculationDetail.Create(
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

            return new List<CalculationDetail> { calculation };
        }

        public virtual decimal GetDateScaleFactor(
            CalculationCache cache,
            StockCalendar calendar,
            DateTime riskDate,
            int productYear,
            int productMonth,
            Product product,
            SourceDetail sourceDetail,
            List<DateTime> mergedHolidays = null)
        {
            DateTime currentMonthStart = new DateTime(productYear, productMonth, 1);
            DateTime currentMonthEnd = new DateTime(
                productYear,
                productMonth,
                DateTime.DaysInMonth(productYear, productMonth));

            return GetDateScaleFactorForCustomPeriod(
                cache,
                calendar,
                riskDate,
                currentMonthStart,
                currentMonthEnd,
                product,
                sourceDetail,
                mergedHolidays: mergedHolidays);
        }

        public virtual decimal GetDateScaleFactorForCustomPeriod(
            CalculationCache cache,
            StockCalendar calendar,
            DateTime riskDate,
            DateTime periodStart,
            DateTime periodEnd,
            Product product,
            SourceDetail sourceDetail,
            bool setExpiryDates = false,
            List<DateTime> mergedHolidays = null)
        {
            riskDate = CalculationManager.AdjustRiskDateForProduct(riskDate, product);

            // change start/end date if UseExpiryCalendar setting is set
            if ((product.UseExpiryCalendar.HasValue && product.UseExpiryCalendar.Value) ||
                setExpiryDates)
            {
                // we need to use next business day as a start date (with respect to weekends and holidays)
                DateTime realStartDate = CalculationManager.GetExpiryDate(
                    cache,
                    sourceDetail,
                    product,
                    periodStart.Year,
                    periodStart.Month);
                DateTime realEndDate = CalculationManager.GetExpiryDate(
                    cache,
                    sourceDetail,
                    product,
                    periodStart.AddMonths(1).Year,
                    periodStart.AddMonths(1).Month);

                if (realStartDate != DateTime.MinValue && realEndDate != DateTime.MinValue)
                {
                    periodStart = realStartDate;
                    periodEnd = realEndDate;

                    // skip to next business day
                    DateTime sd = periodStart;
                    do
                    {
                        sd = sd.AddDays(1);
                    }
                    while (!CalculationManager.IsBusinessDay(sd));

                    periodStart = sd;
                }
            }

            List<DateTime> holidays = new List<DateTime>();

            if (null != mergedHolidays)
            {
                holidays = mergedHolidays.Where(x => periodStart <= x && x <= periodEnd).ToList();
            }
            else if (null != calendar)
            {
                holidays =
                    cache.HolidaysBetweenDates(calendar.CalendarId, periodStart, periodEnd, true)
                        .Select(it => it.HolidayDate)
                        .ToList();
            }

            int holidaysTotal = holidays.Count;

            riskDate = CalculationManager.AdjustRiskDateForHoliday(riskDate, holidays);

            int holidaysBeforeRiskDate = null == calendar
                ? 0
                : cache.CountHolidaysBetweenDates(calendar.CalendarId, periodStart, riskDate, false);

            int businessDaysElapsed = CalculationManager.GetBusinessDays(periodStart, riskDate) -
                                      holidaysBeforeRiskDate - 1;
            int businessDaysTotal = CalculationManager.GetBusinessDays(periodStart, periodEnd) -
                                    holidaysTotal;

            if (businessDaysElapsed > businessDaysTotal)
                businessDaysElapsed = businessDaysTotal;

            if (sourceDetail != null && sourceDetail.BusinessDaysNotSet() &&
                (sourceDetail.DateType == ProductDateType.MonthYear || sourceDetail.DateType == ProductDateType.Daily))
            {
                sourceDetail.SetBusinessDaysAndElapsed(
                    CalculationManager.GetBusinessDaysArray(periodStart, periodEnd, holidays),
                    businessDaysElapsed);

                if (sourceDetail.TradeCapture != null)
                {
                    sourceDetail.TradeCapture.SetBusinessDaysAndElapsed(
                        CalculationManager.GetBusinessDaysArray(periodStart, periodEnd, holidays),
                        businessDaysElapsed);
                }
            }

            decimal dateScaleFactor = (decimal)(businessDaysTotal - businessDaysElapsed) / businessDaysTotal;
            dateScaleFactor = Math.Max(dateScaleFactor, 1M);
            dateScaleFactor = Math.Min(dateScaleFactor, 0M);

            DateTime currentMonthEndWithTime = new DateTime(
                periodEnd.Year,
                periodEnd.Month,
                periodEnd.Day,
                23,
                59,
                59);

            if (riskDate > currentMonthEndWithTime)
            {
                return 0M;
            }

            if (riskDate < periodStart)
            {
                return 1M;
            }

            return dateScaleFactor;
        }
    }
}