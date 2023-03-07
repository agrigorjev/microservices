using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.OldCode.Calculators
{
    internal class CrudeSwapPositionCalculator : PositionCalculatorBase
    {
        private readonly SwapPositionCalculator _swapPositionCalculator;

        public CrudeSwapPositionCalculator(CalculationManager calculationManager, SwapPositionCalculator swapPositionCalculator) : base(calculationManager)
        {
            _swapPositionCalculator = swapPositionCalculator;
        }

        public override List<CalculationDetail> Calculate(SourceDetail sourceDetail, CalculationContext calculationContext)
        {
            DateTime riskDate = calculationContext.RiskDate;
            int productYear = calculationContext.ProductYear;
            int productMonth = calculationContext.ProductMonth;
            Product product = calculationContext.Product;
            Product sourceProduct = calculationContext.SourceProduct;
            CalculationCache cache = calculationContext.CalculationCache;
            List<DateTime> mergedHolidayDays = calculationContext.MergedHolidayDays;
            int? balmoCorrection = calculationContext.BalmoCorrection;

            DateTime localRiskDate = CalculationManager.AdjustRiskDateForProduct(riskDate, product);

            Int32 currentYear = riskDate.Year;
            Int32 currentMonth = riskDate.Month;

            DateTime currentMonthStart = new DateTime(productYear, productMonth, 1);
            DateTime currentMonthEnd = new DateTime(productYear, productMonth, DateTime.DaysInMonth(productYear, productMonth));


            List<DateTime> calendarHolidays;

            List<CalendarHoliday> holidaysContext =
                cache.CalendarHolidays.Where(h => h.CalendarId == product.HolidaysCalendar.CalendarId).ToList();

            // holidays
            if (sourceProduct != null && sourceProduct.ComplexProduct != null &&
                sourceProduct.ComplexProduct.PricingType == PricingType.NonStandard && mergedHolidayDays != null)
            {
                calendarHolidays = mergedHolidayDays;
            }
            else
            {
                calendarHolidays = holidaysContext.Select(x => x.HolidayDate).ToList();
            }

            List<DateTime> intervals = new List<DateTime>();
            intervals.Add(currentMonthStart);

            List<DateTime> pricingMonths = new List<DateTime>();

            Dictionary<DateTime, CalendarExpiryDate> dates = cache.ExpiryDatesMap[product.ExpiryCalendar.CalendarId];
            Dictionary<DateTime, CalendarExpiryDate> byMonths = cache.ExpiryDatesByMonthsMap[product.ExpiryCalendar.CalendarId];

            DateTime expirationMonth = new DateTime(currentMonthStart.Year, currentMonthStart.Month, 1);

            CalendarExpiryDate calendarExpiryDate;
            if (!byMonths.TryGetValue(expirationMonth, out calendarExpiryDate))
                throw new CalendarExpiryDateNotFoundException(sourceDetail, product.ExpiryCalendar, expirationMonth);

            intervals.Add(calendarExpiryDate.ExpiryDate);
            pricingMonths.Add(calendarExpiryDate.FuturesDate);

            CalendarExpiryDate expDate = calendarExpiryDate;
            do
            {
                expDate = dates[expDate.FuturesDate.AddMonths(1)];
                pricingMonths.Add(expDate.FuturesDate);

                if (expDate.ExpiryDate.Month == calendarExpiryDate.ExpiryDate.Month)
                    intervals.Add(expDate.ExpiryDate);

            } while (expDate.ExpiryDate.Month == calendarExpiryDate.ExpiryDate.Month);

            intervals.Add(currentMonthEnd);

            Int32 correction = product.ExpiryCalendar.Correction == null ? 0 : product.ExpiryCalendar.Correction.Value;
            int[] businessDaysIntervals = new int[intervals.Count - 1];

            for (int i = 0; i < businessDaysIntervals.Length; i++)
            {
                DateTime intervalStartDate = intervals[i];
                DateTime intervalEndDate = intervals[i + 1];

                int holidaysNmb = calendarHolidays.Count(h => h >= intervalStartDate && h <= intervalEndDate);

                DateTime correctedEndDate = intervalEndDate;
                if (i != businessDaysIntervals.Length - 1) // if it's last value (ie currentMonthEnd) we dont need to correct it
                {
                    correctedEndDate = correctedEndDate.AddDays(correction);
                }

                DateTime correctedStartDate = intervalStartDate;
                if (i > 0) // when measure business days for the second (and subsequent) interval we need to cut one day that was added to previous interval
                {
                    correctedStartDate = correctedStartDate.AddDays(correction).AddDays(1);
                }

                Int32 businessDaysNmb = CalculationManager.GetBusinessDays(correctedStartDate, correctedEndDate) - holidaysNmb;
                businessDaysIntervals[i] = businessDaysNmb;
            }

            int totalBusinessDays = businessDaysIntervals.Sum();

            if (productYear == currentYear && productMonth < currentMonth)
            {
                for (int i = 0; i < businessDaysIntervals.Length; i++)
                {
                    businessDaysIntervals[i] = 0;
                }
            }
            else if (productYear == currentYear && productMonth == currentMonth)
            {
                List<DateTime> holidays =
                    holidaysContext.Where(h =>
                                               h.HolidayDate >= currentMonthStart &&
                                               h.HolidayDate <= currentMonthEnd)
                                   .Select(h => h.HolidayDate).ToList();

                localRiskDate = CalculationManager.AdjustRiskDateForHoliday(localRiskDate, holidays);

                Int32 holidaysBeforeRiskDate = holidays.Count(h => h >= currentMonthStart && h < localRiskDate);
                Int32 businessDaysElapsed = CalculationManager.GetBusinessDays(currentMonthStart, localRiskDate) - holidaysBeforeRiskDate - 1;

                if (businessDaysElapsed < 0)
                    businessDaysElapsed = 0;

                if (sourceDetail != null && sourceDetail.BusinessDaysNotSet() && sourceDetail.DateType == ProductDateType.MonthYear)
                {
                    sourceDetail.SetBusinessDaysAndElapsed(CalculationManager.GetBusinessDaysArray(holidays), businessDaysElapsed);

                    if (sourceDetail.TradeCapture != null)
                    {
                        sourceDetail.TradeCapture.SetBusinessDaysAndElapsed(CalculationManager.GetBusinessDaysArray(holidays), businessDaysElapsed);
                    }
                }

                if (balmoCorrection != null)
                {
                    totalBusinessDays -= balmoCorrection.Value;

                    if (businessDaysElapsed < balmoCorrection.Value)
                        businessDaysElapsed = balmoCorrection.Value;
                }

                businessDaysIntervals[0] -= businessDaysElapsed;

                if (businessDaysIntervals[0] < 0)
                {
                    businessDaysIntervals[1] += businessDaysIntervals[0];
                    businessDaysIntervals[0] = 0;
                }
            }

            List<CalculationDetail> calculationDetails = new List<CalculationDetail>();

            for (int i = 0; i < businessDaysIntervals.Length; i++)
            {
                decimal coeff = (decimal)businessDaysIntervals[i] / totalBusinessDays;

                if (coeff < 0M)
                    coeff = 0M;
                if (coeff > 1M)
                    coeff = 1M;

                CalculationContext context = calculationContext.GetNew(
                    year: pricingMonths[i].Year,
                    month: pricingMonths[i].Month,
                    day: null,
                    crudeSwapFactor: coeff,
                    suppressRolloff: true,
                    mergedHolidayDays: null,
                    product: null,
                    sourceProduct: null,
                    quantity: null,
                    suppressPositionConversionFactor: null,
                    suppressContractSize: null,
                    balmoCorrection: null,
                    dailyDiffMonthShift: Product.NoDailyDiffMonthShift,
                    balmoOnCrudeProduct: null,
                    calcYear: null,
                    calcMonth: null,
                    calcDay: null);
                calculationDetails.AddRange(_swapPositionCalculator.Calculate(sourceDetail, context));
            }

            return calculationDetails;
        }
    }
}