using Mandara.Entities;
using Mandara.Entities.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.OldCode.Calculators
{
    internal class BalmoPositionCalculator : PositionCalculatorBase
    {
        private readonly CrudeSwapPositionCalculator _crudeSwapPositionCalculator;

        public BalmoPositionCalculator(
            CalculationManager calculationManager,
            CrudeSwapPositionCalculator crudeSwapPositionCalculator)
            : base(calculationManager)
        {
            _crudeSwapPositionCalculator = crudeSwapPositionCalculator;
        }

        public CrackSpreadDiffPositionCalculator CrackSpreadDiffPositionCalculator { get; set; }

        public override List<CalculationDetail> Calculate(
            SourceDetail sourceDetail,
            CalculationContext calculationContext)
        {
            DateTime riskDate = calculationContext.RiskDate;
            int productYear = calculationContext.ProductYear;
            int productMonth = calculationContext.ProductMonth;
            int productDay = calculationContext.ProductDay;
            Product product = calculationContext.Product;
            Product sourceProduct = calculationContext.SourceProduct;
            bool suppressPosConvFactor = calculationContext.SuppressPositionConversionFactor;
            decimal qty = calculationContext.Quantity;
            ProductDateType productDateType = calculationContext.ProductDateType;
            CalculationCache cache = calculationContext.CalculationCache;
            Product balmoOnCrudeProduct = sourceProduct.BalmoOnCrudeProduct ?? calculationContext.BalmoOnCrudeProduct;

            DateTime productDate = new DateTime(productYear, productMonth, productDay);

            List<CalculationDetail> results = new List<CalculationDetail>();

            if (sourceProduct.BalmoOnComplexProduct != null)
            {
                CalculationContext complexContext = calculationContext.GetNew(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    sourceProduct.BalmoOnComplexProduct,
                    null,
                    null,
                    null,
                    null,
                    null,
                    Product.NoDailyDiffMonthShift,
                    null,
                    null,
                    null,
                    null);
                results.AddRange(CrackSpreadDiffPositionCalculator.Calculate(sourceDetail, complexContext));
                return results;
            }

            DateTime localRiskDate = CalculationManager.AdjustRiskDateForProduct(riskDate, product);

            bool riskDateRollofedToNextMonth = localRiskDate > riskDate && localRiskDate.Month != riskDate.Month;

            int currentYear = riskDateRollofedToNextMonth ? riskDate.Year : localRiskDate.Year;
            int currentMonth = riskDateRollofedToNextMonth ? riskDate.Month : localRiskDate.Month;

            if (riskDateRollofedToNextMonth)
                localRiskDate = riskDate;

            if ((productDate.Year <= currentYear && productDate.Month < currentMonth) || productDate.Year < currentYear)
            {
                return new List<CalculationDetail>();
            }

            DateTime currentMonthStart = new DateTime(productDate.Year, productDate.Month, 1);
            DateTime currentMonthEnd = new DateTime(
                productDate.Year,
                productDate.Month,
                DateTime.DaysInMonth(productDate.Year, productDate.Month));

            List<DateTime> holidays =
                cache.CalendarHolidays.Where(
                    h => h.CalendarId == product.HolidaysCalendar.CalendarId && h.HolidayDate >= currentMonthStart &&
                         h.HolidayDate <= currentMonthEnd).Select(x => x.HolidayDate).ToList();

            localRiskDate = CalculationManager.AdjustRiskDateForHoliday(localRiskDate, holidays);

            int holidaysTotal = holidays.Count;
            int businessDaysTotal = CalculationManager.GetBusinessDays(currentMonthStart, currentMonthEnd)
                                    - holidaysTotal;

            DateTime balmoPricingDate = productDate;

            int holidaysBeforePricingDate =
                cache.CalendarHolidays.Count(
                    h => h.CalendarId == product.HolidaysCalendar.CalendarId &&
                         h.HolidayDate >= currentMonthStart &&
                         h.HolidayDate < balmoPricingDate);

            int pricingBusinessDay = CalculationManager.GetBusinessDays(currentMonthStart, balmoPricingDate)
                                     - holidaysBeforePricingDate;
            int remainingBusinessDays = businessDaysTotal - pricingBusinessDay + 1;

            int holidaysCurrent =
                cache.CalendarHolidays.Count(
                    h => h.CalendarId == product.HolidaysCalendar.CalendarId &&
                         h.HolidayDate >= currentMonthStart && h.HolidayDate < localRiskDate);

            int currentBusinessDay = CalculationManager.GetBusinessDays(currentMonthStart, localRiskDate)
                                     - holidaysCurrent;
            decimal balmoFactor = currentBusinessDay <= pricingBusinessDay
                ? 1
                : (decimal)(businessDaysTotal - currentBusinessDay + 1) / remainingBusinessDays;

            decimal balmoProductFactor = suppressPosConvFactor
                ? 1
                : (product.PositionFactor == null ? 1 : product.PositionFactor.Value) * product.ContractSize;
            decimal amount = qty * balmoProductFactor;

            if (sourceDetail != null && sourceDetail.BusinessDaysNotSet() &&
                productDate.Year == currentYear && productDate.Month == currentMonth)
            {
                sourceDetail.SetBusinessDaysAndElapsed(
                    CalculationManager.GetBusinessDaysArray(holidays),
                    currentBusinessDay - 1,
                    pricingBusinessDay);

                if (sourceDetail.TradeCapture != null)
                {
                    sourceDetail.TradeCapture.SetBusinessDaysAndElapsed(
                        CalculationManager.GetBusinessDaysArray(holidays),
                        currentBusinessDay - 1,
                        pricingBusinessDay);
                }
            }

            if (riskDateRollofedToNextMonth)
            {
                if (sourceDetail != null)
                {
                    sourceDetail.SetElapsedToMax();

                    if (sourceDetail.TradeCapture != null)
                    {
                        sourceDetail.TradeCapture.SetElapsedToMax();
                    }
                }

                amount = 0;
            }

            if (balmoOnCrudeProduct != null)
            {
                CalculationContext context = calculationContext.GetNew(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    balmoOnCrudeProduct,
                    null,
                    amount,
                    true,
                    true,
                    pricingBusinessDay - 1,
                    Product.NoDailyDiffMonthShift,
                    null,
                    null,
                    null,
                    null);

                results.AddRange(_crudeSwapPositionCalculator.Calculate(sourceDetail, context));
            }
            else
            {
                decimal amountAtMonth = amount * balmoFactor;

                CalculationDetail calculation = CalculationDetail.Create(
                    sourceDetail,
                    product,
                    sourceProduct,
                    productDate.Year,
                    productDate.Month,
                    amountAtMonth,
                    productDate,
                    productDateType,
                    sourceDetail.StripName,
                    null,
                    null,
                    Product.NoDailyDiffMonthShift);

                results.Add(calculation);
            }

            return results;
        }
    }
}