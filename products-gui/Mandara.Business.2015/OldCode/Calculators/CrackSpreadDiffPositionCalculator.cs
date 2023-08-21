using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.ErrorReporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.OldCode.Calculators
{
    internal class CrackSpreadDiffPositionCalculator : PositionCalculatorBase
    {
        private readonly CrudeSwapPositionCalculator _crudeSwapPositionCalculator;
        private readonly MandaraEntities _efContext;
        private readonly FuturesPositionCalculator _futuresPositionCalculator;
        private readonly SwapPositionCalculator _swapPositionCalculator;
        private readonly TradeMonthSwapPositionCalculator _tradeMonthSwapPositionCalculator;

        public CrackSpreadDiffPositionCalculator(
            CalculationManager calculationManager,
            MandaraEntities efContext,
            FuturesPositionCalculator futuresPositionCalculator,
            CrudeSwapPositionCalculator crudeSwapPositionCalculator,
            SwapPositionCalculator swapPositionCalculator,
            TradeMonthSwapPositionCalculator tradeMonthSwapPositionCalculator)
            : base(calculationManager)
        {
            _efContext = efContext;
            _futuresPositionCalculator = futuresPositionCalculator;
            _crudeSwapPositionCalculator = crudeSwapPositionCalculator;
            _swapPositionCalculator = swapPositionCalculator;
            _tradeMonthSwapPositionCalculator = tradeMonthSwapPositionCalculator;
        }

        public BalmoPositionCalculator BalmoPositionCalculator { get; set; }

        public override List<CalculationDetail> Calculate(
            SourceDetail sourceDetail,
            CalculationContext calculationContext)
        {
            DateTime riskDate = calculationContext.RiskDate;
            int productMonth = calculationContext.ProductMonth;
            Product product = calculationContext.Product;
            ProductDateType productDateType = calculationContext.ProductDateType;
            CalculationCache cache = calculationContext.CalculationCache;

            DateTime productDate = new DateTime(calculationContext.ProductYear, productMonth, 1);

            if (product.ComplexProduct == null)
            {
                _efContext.Entry(product).Reference(p => p.ComplexProduct).Load();
                _efContext.Entry(product.ComplexProduct).Reference(p => p.ChildProduct1).Load();
                _efContext.Entry(product.ComplexProduct).Reference(p => p.ChildProduct2).Load();
                _efContext.Entry(product.ComplexProduct.ChildProduct1).Reference(p => p.Category).Load();
                _efContext.Entry(product.ComplexProduct.ChildProduct1).Reference(p => p.ExpiryCalendar).Load();
                _efContext.Entry(product.ComplexProduct.ChildProduct1).Reference(p => p.HolidaysCalendar).Load();
                _efContext.Entry(product.ComplexProduct.ChildProduct1).Reference(p => p.OfficialProduct).Load();
                _efContext.Entry(product.ComplexProduct.ChildProduct2).Reference(p => p.Category).Load();
                _efContext.Entry(product.ComplexProduct.ChildProduct2).Reference(p => p.ExpiryCalendar).Load();
                _efContext.Entry(product.ComplexProduct.ChildProduct2).Reference(p => p.HolidaysCalendar).Load();
                _efContext.Entry(product.ComplexProduct.ChildProduct2).Reference(p => p.OfficialProduct).Load();
            }

            List<CalendarHoliday> calendarHolidays = cache.CalendarHolidays;

            DateTime currentMonthStart = new DateTime(productDate.Year, productMonth, 1);
            DateTime currentMonthEnd = new DateTime(
                productDate.Year,
                productMonth,
                DateTime.DaysInMonth(productDate.Year, productMonth));

            List<DateTime> mergedHolidays = null;
            if (product.ComplexProduct.PricingType == PricingType.NonStandard && productDateType != ProductDateType.Day)
            {
                Product leg1Product = product.ComplexProduct.ChildProduct1;

                CalendarExpiryDate leg1ExpiryDate;

                Dictionary<DateTime, CalendarExpiryDate> leg1ExpiryMap =
                    cache.ExpiryDatesMap[leg1Product.ExpiryCalendar.CalendarId];
                leg1ExpiryDate = leg1ExpiryMap[currentMonthStart];

                IEnumerable<CalendarHoliday> leg1Holidays =
                    calendarHolidays.Where(
                        h => h.CalendarId == leg1Product.HolidaysCalendar.CalendarId &&
                             h.HolidayDate >= currentMonthStart &&
                             h.HolidayDate <= currentMonthEnd);

                Product leg2Product = product.ComplexProduct.ChildProduct2;

                CalendarExpiryDate leg2ExpiryDate;

                Dictionary<DateTime, CalendarExpiryDate> leg2ExpiryMap =
                    cache.ExpiryDatesMap[leg2Product.ExpiryCalendar.CalendarId];
                leg2ExpiryDate = leg2ExpiryMap[currentMonthStart];

                IEnumerable<CalendarHoliday> leg2Holidays =
                    calendarHolidays.Where(
                        h => h.CalendarId == leg2Product.HolidaysCalendar.CalendarId &&
                             h.HolidayDate >= currentMonthStart &&
                             h.HolidayDate <= currentMonthEnd);

                mergedHolidays =
                    leg1Holidays.Select(x => x.HolidayDate).Union(leg2Holidays.Select(x => x.HolidayDate)).ToList();

                if (mergedHolidays.Count(x => x == leg1ExpiryDate.ExpiryDate || x == leg2ExpiryDate.ExpiryDate) > 0)
                {
                    ErrorReportingHelper.GlobalQueue.Enqueue(
                        new Error(
                            "Live Positions",
                            ErrorType.CalculationError,
                            string.Format(
                                "Product {0} {1} uses non-standard pricing model but one of its legs has a holiday on expiry date of another leg",
                                product.Name,
                                sourceDetail.StripName),
                            product.ProductId.ToString(),
                            product,
                            ErrorLevel.Critical));

                    return new List<CalculationDetail>();
                }
            }

            DateTime localRiskDate = CalculationManager.AdjustRiskDateForProduct(riskDate, product);

            int currentYear = localRiskDate.Year;
            int currentMonth = localRiskDate.Month;

            if (productDate.Year == currentYear && productMonth == currentMonth)
            {
                List<DateTime> holidays = mergedHolidays ??
                                          calendarHolidays.Where(
                                              h => h.CalendarId == product.HolidaysCalendar.CalendarId &&
                                                   h.HolidayDate >= currentMonthStart &&
                                                   h.HolidayDate <= currentMonthEnd)
                                              .Select(h => h.HolidayDate).ToList();

                localRiskDate = CalculationManager.AdjustRiskDateForHoliday(localRiskDate, holidays);

                int holidaysBeforeRiskDate = holidays.Count(h => h >= currentMonthStart && h < localRiskDate);
                int businessDaysElapsed = CalculationManager.GetBusinessDays(currentMonthStart, localRiskDate)
                                          - holidaysBeforeRiskDate - 1;

                if (businessDaysElapsed < 0)
                    businessDaysElapsed = 0;
            }

            List<CalculationDetail> results = new List<CalculationDetail>();

            if (product.TreatTimespreadStripAsLegs && sourceDetail.IsTimeSpread)
            {
                if (sourceDetail.UseFirstLegForBusinessDays)
                {
                    CalculationContext context1 = calculationContext.GetNew(
                        null,
                        null,
                        null,
                        null,
                        null,
                        mergedHolidays,
                        null,
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
                    results.AddRange(CalculateComplexProductLeg(sourceDetail, context1, LegType.Leg1));
                }
                else
                {
                    CalculationContext context2 = calculationContext.GetNew(
                        null,
                        null,
                        null,
                        null,
                        null,
                        mergedHolidays,
                        null,
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
                    results.AddRange(CalculateComplexProductLeg(sourceDetail, context2, LegType.Leg2));
                }
            }
            else
            {
                CalculationContext context1 = calculationContext.GetNew(
                    null,
                    null,
                    null,
                    null,
                    null,
                    mergedHolidays,
                    null,
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
                results.AddRange(CalculateComplexProductLeg(sourceDetail, context1, LegType.Leg1));

                CalculationContext context2 = calculationContext.GetNew(
                    null,
                    null,
                    null,
                    null,
                    null,
                    mergedHolidays,
                    null,
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
                results.AddRange(CalculateComplexProductLeg(sourceDetail, context2, LegType.Leg2));
            }

            return results;
        }

        private List<CalculationDetail> CalculateComplexProductLeg(
            SourceDetail sourceDetail,
            CalculationContext calculationContext,
            LegType legType)
        {
            DateTime riskDate = calculationContext.RiskDate;
            int productMonth = calculationContext.ProductMonth;
            Product product = calculationContext.Product;
            decimal productQty = calculationContext.Quantity;
            ProductDateType productDateType = calculationContext.ProductDateType;
            CalculationCache cache = calculationContext.CalculationCache;

            DateTime productDate = new DateTime(calculationContext.ProductYear, productMonth, 1);

            List<CalculationDetail> results = new List<CalculationDetail>();

            if (legType == LegType.Leg2)
            {
                sourceDetail.ChangeToSecondLeg();
                sourceDetail.TradeCapture?.ChangeToSecondLeg();
            }

            Product legProduct = legType == LegType.Leg1
                ? product.ComplexProduct.ChildProduct1
                : product.ComplexProduct.ChildProduct2;
            decimal legSignFactor = sourceDetail.IsTimeSpread && product.TreatTimespreadStripAsLegs
                ? 1M
                : (legType == LegType.Leg1 ? 1M : -1M);
            decimal legFactor = legType == LegType.Leg1
                ? product.ComplexProduct.ConversionFactor1.Value
                : product.ComplexProduct.ConversionFactor2.Value;

            sourceDetail.SetOfficialProductProps(legProduct, legFactor);

            if (sourceDetail.TradeCapture != null)
            {
                sourceDetail.TradeCapture.SetOfficialProductProps(legProduct, legFactor);
            }

            switch (legProduct.Type)
            {
                case ProductType.TradeMonthSwap:
                DateTime periodStart = new DateTime(productDate.Year, productMonth, 1).AddMonths(-1);
                DateTime periodEnd = new DateTime(productDate.Year, productMonth, 1);

                decimal dateScaleFactor = _swapPositionCalculator.GetDateScaleFactorForCustomPeriod(
                    cache,
                    legProduct.HolidaysCalendar,
                    riskDate,
                    periodStart,
                    periodEnd,
                    legProduct,
                    sourceDetail,
                    true);

                decimal qty = productQty * legSignFactor * legFactor * dateScaleFactor * product.ContractSize;
                decimal amountAtMonth = qty * dateScaleFactor;

                CalculationDetail calculation = CalculationDetail.Create(
                    sourceDetail,
                    legProduct,
                    product,
                    productDate.Year,
                    productMonth,
                    amountAtMonth,
                    productDate,
                    productDateType,
                    sourceDetail.StripName,
                    null,
                    null,
                    Product.NoDailyDiffMonthShift);

                results.Add(calculation);
                break;

                case ProductType.FuturesBasedSwap:
                decimal qtyCrudeSwap = productQty * legSignFactor * legFactor * product.ContractSize;

                if (productDateType == ProductDateType.Day)
                {
                    CalculationContext balmoContext = calculationContext.GetNew(
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        legProduct,
                        product,
                        qtyCrudeSwap,
                        true,
                        null,
                        null,
                        Product.NoDailyDiffMonthShift,
                        legProduct,
                        null,
                        null,
                        null);

                    results.AddRange(BalmoPositionCalculator.Calculate(sourceDetail, balmoContext));
                }
                else
                {
                    CalculationContext crudeSwapContext = calculationContext.GetNew(
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        legProduct,
                        product,
                        qtyCrudeSwap,
                        true,
                        true,
                        null,
                        Product.NoDailyDiffMonthShift,
                        null,
                        null,
                        null,
                        null);

                    results.AddRange(_crudeSwapPositionCalculator.Calculate(sourceDetail, crudeSwapContext));
                }
                break;

                case ProductType.Futures:
                decimal futuresQty = productQty * legSignFactor * legFactor;

                CalculationContext futuresContext = calculationContext.GetNew(
                    null,
                    null,
                    null,
                    null,
                    true,
                    null,
                    legProduct,
                    product,
                    futuresQty,
                    true,
                    null,
                    null,
                    Product.NoDailyDiffMonthShift,
                    null,
                    null,
                    null,
                    null);

                results.AddRange(_futuresPositionCalculator.Calculate(sourceDetail, futuresContext));
                break;

                default:
                dateScaleFactor = 1M;
                qty = productQty * legSignFactor * legFactor * dateScaleFactor * product.ContractSize;

                if (productDateType == ProductDateType.Day)
                {
                    CalculationContext balmoContext = calculationContext.GetNew(
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        legProduct,
                        product,
                        qty,
                        true,
                        null,
                        null,
                        Product.NoDailyDiffMonthShift,
                        null,
                        null,
                        null,
                        null);

                    results.AddRange(BalmoPositionCalculator.Calculate(sourceDetail, balmoContext));
                }
                else
                {
                    DateTime localRiskDate = CalculationManager.AdjustRiskDateForProduct(riskDate, legProduct);
                    DateTime nextMonthDate = localRiskDate.AddMonths(1);

                    if ((productDate.Year == localRiskDate.Year && productMonth == localRiskDate.Month) ||
                        productDate.Year == nextMonthDate.Year && productMonth == nextMonthDate.Month &&
                        legProduct.UseExpiryCalendar.HasValue &&
                        legProduct.UseExpiryCalendar.Value)
                    {
                        dateScaleFactor = _swapPositionCalculator.GetDateScaleFactor(
                            cache,
                            legProduct.HolidaysCalendar,
                            riskDate,
                            productDate.Year,
                            productMonth,
                            legProduct,
                            sourceDetail,
                            calculationContext.MergedHolidayDays);
                    }
                    else if ((productMonth < localRiskDate.Month && productDate.Year <= localRiskDate.Year) ||
                             productDate.Year < localRiskDate.Year)
                    {
                        // we need to do that in order to set TradeCapture.BusinessDays count and so on.
                        _swapPositionCalculator.GetDateScaleFactor(
                            cache,
                            legProduct.HolidaysCalendar,
                            riskDate,
                            productDate.Year,
                            productMonth,
                            legProduct,
                            sourceDetail,
                            calculationContext.MergedHolidayDays);

                        dateScaleFactor = 0M;

                        sourceDetail.SetElapsedToMax();

                        if (sourceDetail.TradeCapture != null)
                            sourceDetail.TradeCapture.SetElapsedToMax();
                    }

                    amountAtMonth = qty * dateScaleFactor;

                    calculation = CalculationDetail.Create(
                        sourceDetail,
                        legProduct,
                        product,
                        productDate.Year,
                        productMonth,
                        amountAtMonth,
                        productDate,
                        productDateType,
                        sourceDetail.StripName,
                        null,
                        null,
                        Product.NoDailyDiffMonthShift);

                    results.Add(calculation);
                }
                break;
            }

            return results;
        }
    }
}