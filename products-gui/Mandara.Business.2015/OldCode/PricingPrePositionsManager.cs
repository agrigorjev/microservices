using Mandara.Business.Calculators;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.ErrorReporting;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mandara.Date;
using Mandara.Entities.EntitiesCustomization;

namespace Mandara.Business.OldCode
{
    public class PricingPrePositionsManager
    {
        private readonly ILogger _logger;

        public PricingPrePositionsManager(ILogger logger)
        {
            _logger = logger;
        }

        public PricingPrePositionsManager() : this(new NLogLoggerFactory().GetCurrentClassLogger())
        {
        }

        public virtual List<PricingCalculationDetail> CalculateTasReport(
            DateTime startDate,
            DateTime endDate,
            List<SourceDetail> details,
            CalculationCache cache = null)
        {
            List<PricingCalculationDetail> tasPositions;

            using (MandaraEntities context = new MandaraEntities(
                MandaraEntities.DefaultConnStrName,
                nameof(PricingPrePositionsManager)))
            {
                if (cache == null)
                {
                    cache = new CalculationCache();
                    cache.Initialize(context);
                }

                tasPositions = CalculatePricingPositions(cache, context, details, startDate, endDate);
            }

            return tasPositions;
        }

        private List<PricingCalculationDetail> CalculatePricingPositions(
            CalculationCache cache,
            MandaraEntities context,
            List<SourceDetail> sourceDetails,
            DateTime startDate,
            DateTime endDate)
        {
            List<PricingCalculationDetail> result = new List<PricingCalculationDetail>();
            Dictionary<int, bool> timeSpreadsCalculated = new Dictionary<int, bool>();

            for (int index = 0; index <= sourceDetails.Count; index++)
            {
                if ((index > 0) && sourceDetails[index - 1].IsTimeSpread)
                {
                    SourceDetail timeSpread = sourceDetails[index - 1];

                    if (!timeSpreadsCalculated.ContainsKey(timeSpread.SourceDetailId))
                    {
                        index = index - 1;

                        timeSpreadsCalculated.Add(timeSpread.SourceDetailId, true);
                    }
                }

                if (index == sourceDetails.Count)
                {
                    break;
                }

                SourceDetail sourceItem = sourceDetails[index];

                try
                {
                    (DateTime productDate, ProductDateType dateType, decimal quantity) =
                        StripQuantities(timeSpreadsCalculated, sourceItem);
                    Product product = PrecalcProduct(cache, sourceItem);

                    switch (product.Type)
                    {
                        case ProductType.FuturesBasedSwap:
                        {
                            FutureBasedSwapPrecalcs(
                                cache,
                                context,
                                startDate,
                                endDate,
                                dateType,
                                result,
                                product,
                                quantity,
                                productDate,
                                sourceItem);
                        }
                        break;

                        case ProductType.Swap:
                        {
                            SwapPrecalcs(
                                cache,
                                context,
                                startDate,
                                endDate,
                                dateType,
                                result,
                                product,
                                quantity,
                                productDate,
                                sourceItem);
                        }
                        break;

                        case ProductType.TradeMonthSwap:
                        {
                            TradeMonthSwapPrecalcs(
                                cache,
                                context,
                                startDate,
                                endDate,
                                dateType,
                                result,
                                product,
                                quantity,
                                productDate,
                                sourceItem);
                        }
                        break;

                        case ProductType.Diff:
                        {
                            DiffPrecalcs(
                                cache,
                                context,
                                startDate,
                                endDate,
                                dateType,
                                result,
                                product,
                                quantity,
                                productDate,
                                sourceItem);
                        }
                        break;

                        case ProductType.Balmo:
                        {
                            BalmoPrecalcs(
                                cache,
                                context,
                                dateType,
                                sourceItem,
                                product,
                                result,
                                quantity,
                                productDate);
                        }
                        break;

                        case ProductType.Futures:
                        {
                            FuturesPreCalcs(
                                cache,
                                productDate,
                                dateType,
                                sourceItem,
                                product,
                                quantity,
                                result);
                        }
                        break;
                    }
                }
                catch (Exception ex)
                {
                    ReportCalculationErrorButWhatHappened(ex, sourceItem);
                }
            }

            return result;
        }

        private (DateTime productDate, ProductDateType dateType, decimal quantity) StripQuantities(
            Dictionary<int, bool> isSpreadCalculated,
            SourceDetail trade)
        {
            if (!isSpreadCalculated.TryGetValue(trade.SourceDetailId, out bool useSecondStrip))
            {
                useSecondStrip = false;
            }

            DateTime productDate = !useSecondStrip ? trade.ProductDate : trade.ProductDate2;
            ProductDateType productDateType = !useSecondStrip ? trade.DateType : trade.DateType2;
            decimal quantity = !useSecondStrip ? trade.Quantity.Value : trade.Quantity.Value * -1M;

            return (productDate, productDateType, quantity);
        }

        private static Product PrecalcProduct(CalculationCache cache, SourceDetail sourceItem)
        {
            int productId = sourceItem.ProductId;

            if ((productId == 0) && (sourceItem.Product != null))
            {
                productId = sourceItem.Product.ProductId;
            }

            Product product = cache == null ? sourceItem.Product : cache.GetProductById(productId);
            return product;
        }

        private void FutureBasedSwapPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            DateTime startDate,
            DateTime endDate,
            ProductDateType productDateType,
            List<PricingCalculationDetail> result,
            Product product,
            decimal quantity,
            DateTime productDate,
            SourceDetail sourceItem)
        {
            switch (productDateType)
            {
                case ProductDateType.MonthYear:
                {
                    CalculateCrudeSwap(
                        cache,
                        context,
                        result,
                        product,
                        quantity,
                        productDate.Year,
                        productDate.Month,
                        sourceItem);
                }
                break;

                case ProductDateType.Quarter:
                case ProductDateType.Year:
                {
                    QOrCalFutureBasedSwapPrecalcs(
                        cache,
                        context,
                        startDate,
                        endDate,
                        productDateType,
                        result,
                        product,
                        quantity,
                        productDate,
                        sourceItem);
                }
                break;

                case ProductDateType.Custom:
                {
                    CustomFutureBasedSwapPrecalcs(
                        cache,
                        context,
                        startDate,
                        endDate,
                        result,
                        product,
                        quantity,
                        productDate,
                        sourceItem);
                }
                break;

                case ProductDateType.Day:
                {
                    CalculateBalmo(
                        cache,
                        context,
                        result,
                        product,
                        quantity,
                        productDate,
                        sourceItem,
                        product);
                }
                break;
            }
        }

        private void QOrCalFutureBasedSwapPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            DateTime startDate,
            DateTime endDate,
            ProductDateType productDateType,
            List<PricingCalculationDetail> result,
            Product product,
            decimal quantity,
            DateTime productDate,
            SourceDetail sourceItem)
        {
            int addonMonths = PeriodDuration(productDateType);
            DateTime productEndDate = productDate.AddMonths(addonMonths).AddDays(-1);

            DateTime reportStart = startDate > productDate ? startDate : productDate;
            DateTime reportEnd = endDate < productEndDate ? endDate : productEndDate;

            for (int month = reportStart.Month; month <= reportEnd.Month; month++)
            {
                CalculateCrudeSwap(
                    cache,
                    context,
                    result,
                    product,
                    quantity,
                    productDate.Year,
                    month,
                    sourceItem);
            }
        }

        private static int PeriodDuration(ProductDateType dateType)
        {
            return dateType == ProductDateType.Quarter ? 3 : 12;
        }

        private void CustomFutureBasedSwapPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            DateTime startDate,
            DateTime endDate,
            List<PricingCalculationDetail> result,
            Product product,
            decimal quantity,
            DateTime productDate,
            SourceDetail sourceItem)
        {
            DateTime prStartDate = productDate;
            DateTime prEndDate = sourceItem.TradeEndDate;

            DateTime customStart = startDate > prStartDate ? startDate : prStartDate;
            DateTime customEnd = endDate < prEndDate ? endDate : prEndDate;

            int numMonths = customEnd.MonthsSince(customStart) + 1;

            for (int i = 0; i < numMonths; i++)
            {
                DateTime date = customStart.AddMonths(i);
                int year = date.Year;
                int month = date.Month;

                CalculateCrudeSwap(
                    cache,
                    context,
                    result,
                    product,
                    quantity,
                    year,
                    month,
                    sourceItem);
            }
        }

        private void SwapPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            DateTime startDate,
            DateTime endDate,
            ProductDateType productDateType,
            List<PricingCalculationDetail> result,
            Product product,
            decimal quantity,
            DateTime productDate,
            SourceDetail sourceItem)
        {
            switch (productDateType)
            {
                case ProductDateType.MonthYear:
                {
                    CalculateSwap(
                    cache,
                    context,
                    result,
                    product,
                    quantity,
                    productDate.Year,
                    productDate.Month,
                    sourceItem);
                }
                break;

                case ProductDateType.Quarter:
                case ProductDateType.Year:
                {
                    QOrCalSwapPrecalcs(
                        cache,
                        context,
                        startDate,
                        endDate,
                        productDateType,
                        result,
                        product,
                        quantity,
                        productDate,
                        sourceItem);
                }
                break;

                case ProductDateType.Day:
                {
                    CalculateBalmo(
                        cache,
                        context,
                        result,
                        product,
                        quantity,
                        productDate,
                        sourceItem);
                }
                break;

                case ProductDateType.Custom:
                {
                    CustomSwapPrecalcs(
                        cache,
                        context,
                        startDate,
                        endDate,
                        result,
                        product,
                        quantity,
                        productDate,
                        sourceItem);
                }
                break;
            }
        }

        private void QOrCalSwapPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            DateTime startDate,
            DateTime endDate,
            ProductDateType productDateType,
            List<PricingCalculationDetail> result,
            Product product,
            decimal quantity,
            DateTime productDate,
            SourceDetail sourceItem)
        {
            int addonMonths = PeriodDuration(productDateType);
            DateTime productEndDate = productDate.AddMonths(addonMonths).AddDays(-1);

            DateTime reportStart = startDate > productDate ? startDate : productDate;
            DateTime reportEnd = endDate < productEndDate ? endDate : productEndDate;

            for (int month = reportStart.Month; month <= reportEnd.Month; month++)
            {
                CalculateSwap(
                    cache,
                    context,
                    result,
                    product,
                    quantity,
                    productDate.Year,
                    month,
                    sourceItem);
            }
        }

        private void CustomSwapPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            DateTime startDate,
            DateTime endDate,
            List<PricingCalculationDetail> result,
            Product product,
            decimal quantity,
            DateTime productDate,
            SourceDetail sourceItem)
        {
            DateTime prStartDate = productDate;
            DateTime prEndDate = sourceItem.TradeEndDate;

            DateTime customStart = startDate > prStartDate ? startDate : prStartDate;
            DateTime customEnd = endDate < prEndDate ? endDate : prEndDate;

            int numMonths = customEnd.MonthsSince(customStart) + 1;

            for (int i = 0; i < numMonths; i++)
            {
                DateTime date = customStart.AddMonths(i);
                int year = date.Year;
                int month = date.Month;

                CalculateSwap(
                    cache,
                    context,
                    result,
                    product,
                    quantity,
                    year,
                    month,
                    sourceItem);
            }
        }

        private void TradeMonthSwapPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            DateTime startDate,
            DateTime endDate,
            ProductDateType productDateType,
            List<PricingCalculationDetail> result,
            Product product,
            decimal quantity,
            DateTime productDate,
            SourceDetail sourceItem)
        {
            switch (productDateType)
            {
                case ProductDateType.MonthYear:
                {
                    CalculateTradeMonthSwap(
                    cache,
                    context,
                    result,
                    product,
                    quantity,
                    productDate.Year,
                    productDate.Month,
                    sourceItem);
                }
                break;

                case ProductDateType.Quarter:
                case ProductDateType.Year:
                {
                    QOrCalTradeMonthSwapPrecalcs(cache, context, startDate, endDate,
                    productDateType,
                    result,
                    product,
                    quantity,
                    productDate,
                    sourceItem);
                }
                break;

                case ProductDateType.Day:
                {
                    CalculateBalmo(
                    cache,
                    context,
                    result,
                    product,
                    quantity,
                    productDate,
                    sourceItem);
                }
                break;

                case ProductDateType.Custom:
                {
                    CustomTradeMonthSwapPrecalcs(cache, context, startDate, endDate,
                    result,
                    product,
                    quantity,
                    productDate,
                    sourceItem);
                }
                break;
            }
        }

        private void CalculateTradeMonthSwap(
            CalculationCache cache,
            MandaraEntities context,
            List<PricingCalculationDetail> calculations,
            Product product,
            decimal qty,
            int productYear,
            int productMonth,
            SourceDetail detail,
            bool suppressPosConvFactor = false,
            bool suppressContractSize = false,
            List<DateTime> mergedHolidays = null)
        {
            DateTime currentMonthStart = new DateTime(productYear, productMonth, 1);
            DateTime currentMonthEnd = new DateTime(
                productYear,
                productMonth,
                DateTime.DaysInMonth(productYear, productMonth));

            DateTime realStartDate = CalculationManager.GetExpiryDate(
                cache,
                detail,
                product,
                currentMonthStart.AddMonths(-1).Year,
                currentMonthStart.AddMonths(-1).Month,
                true);
            DateTime realEndDate = CalculationManager.GetExpiryDate(
                cache,
                detail,
                product,
                currentMonthStart.Year,
                currentMonthStart.Month);

            if ((realStartDate != DateTime.MinValue) && (realEndDate != DateTime.MinValue))
            {
                currentMonthStart = realStartDate;
                currentMonthEnd = realEndDate;
            }

            List<DateTime> holidays;
            if (mergedHolidays == null)
            {
                List<CalendarHoliday> calendarHolidays;

                if (cache == null)
                {
                    DbSet<CalendarHoliday> calendarContext = context.CalendarHolidays;
                    calendarHolidays =
                        calendarContext.Where(
                            h =>
                                (h.CalendarId == product.HolidaysCalendar.CalendarId)
                                && (h.HolidayDate >= currentMonthStart) && (h.HolidayDate <= currentMonthEnd)).ToList();
                }
                else
                {
                    calendarHolidays = cache.HolidaysBetweenDates(
                        product.HolidaysCalendar.CalendarId,
                        currentMonthStart,
                        currentMonthEnd,
                        true);
                }

                holidays = calendarHolidays.Select(x => x.HolidayDate).ToList();
            }
            else
            {
                holidays = mergedHolidays.Where(x => (currentMonthStart <= x) && (x <= currentMonthEnd)).ToList();
            }

            int businessDaysTotal = GetBusinessDaysCount(currentMonthStart, currentMonthEnd) - holidays.Count;
            decimal positionFactor = suppressPosConvFactor
                ? 1
                : product.PositionFactor == null ? 1 : product.PositionFactor.Value;
            decimal contractSize = suppressContractSize ? 1 : product.ContractSize;
            decimal baseAmount = qty * positionFactor * contractSize;
            decimal amount = suppressContractSize
                ? baseAmount
                : ContractSizeCalculator.ApplyContractSizeMultiplier(
                    baseAmount,
                    product.ContractSizeMultiplier,
                    productYear,
                    productMonth);
            int day = 0;

            for (int i = 1; i <= businessDaysTotal; i++)
            {
                DateTime calculationDate;

                do
                {
                    calculationDate = currentMonthStart.AddDays(day);
                    day++;
                }
                while (!IsBusinessDay(calculationDate, holidays));

                decimal amountAtDay = amount / businessDaysTotal;
                DateTime period = product.IsEnabledRiskDecomposition
                    ? new DateTime(calculationDate.Year, calculationDate.Month, 1)
                    : new DateTime(productYear, productMonth, 1);

                PricingCalculationDetail calculation = GetPricingCalculationDetail(
                    calculations,
                    product,
                    period,
                    calculationDate,
                    detail,
                    amountAtDay);

                calculation.Amount += amountAtDay;
            }
        }

        private void QOrCalTradeMonthSwapPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            DateTime startDate,
            DateTime endDate,
            ProductDateType productDateType,
            List<PricingCalculationDetail> result,
            Product product,
            decimal quantity,
            DateTime productDate,
            SourceDetail sourceItem)
        {
            int addonMonths = PeriodDuration(productDateType);
            DateTime productEndDate = productDate.AddMonths(addonMonths).AddDays(-1);

            DateTime reportStart = startDate > productDate ? startDate : productDate;
            DateTime reportEnd = endDate < productEndDate ? endDate : productEndDate;

            for (int month = reportStart.Month; month <= reportEnd.Month; month++)
            {
                CalculateTradeMonthSwap(
                    cache,
                    context,
                    result,
                    product,
                    quantity,
                    productDate.Year,
                    month,
                    sourceItem);
            }
        }

        private void CustomTradeMonthSwapPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            DateTime startDate,
            DateTime endDate,
            List<PricingCalculationDetail> result,
            Product product,
            decimal quantity,
            DateTime productDate,
            SourceDetail sourceItem)
        {
            DateTime prStartDate = productDate;
            DateTime prEndDate = sourceItem.TradeEndDate;

            DateTime customStart = startDate > prStartDate ? startDate : prStartDate;
            DateTime customEnd = endDate < prEndDate ? endDate : prEndDate;

            int numMonths = customEnd.MonthsSince(customStart) + 1;

            for (int i = 0; i < numMonths; i++)
            {
                DateTime date = customStart.AddMonths(i);
                int year = date.Year;
                int month = date.Month;

                CalculateTradeMonthSwap(
                    cache,
                    context,
                    result,
                    product,
                    quantity,
                    year,
                    month,
                    sourceItem);
            }
        }

        private void DiffPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            DateTime startDate,
            DateTime endDate,
            ProductDateType productDateType,
            List<PricingCalculationDetail> result,
            Product product,
            decimal quantity,
            DateTime productDate,
            SourceDetail sourceItem)
        {
            switch (productDateType)
            {
                case ProductDateType.MonthYear:
                {
                    DailyDiffPrecalcs(
                        cache,
                        context,
                        result,
                        product,
                        quantity,
                        productDate,
                        productDateType,
                        productDate.Month,
                        sourceItem);
                }
                break;

                case ProductDateType.Quarter:
                case ProductDateType.Year:
                {
                    QOrCalDiffPrecalcs(
                        cache,
                        context,
                        startDate,
                        endDate,
                        productDateType,
                        result,
                        product,
                        quantity,
                        productDate,
                        sourceItem);
                }
                break;

                case ProductDateType.Day:
                {
                    DailyDiffPrecalcs(
                        cache,
                        context,
                        result,
                        product,
                        quantity,
                        productDate,
                        productDateType,
                        productDate.Month,
                        sourceItem);
                }
                break;

                case ProductDateType.Custom:
                {
                    CustomDiffPrecalcs(cache, context, startDate, endDate,
                    productDateType,
                    result,
                    product,
                    quantity,
                    productDate,
                    sourceItem);
                }
                break;
            }
        }

        private void QOrCalDiffPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            DateTime startDate,
            DateTime endDate,
            ProductDateType productDateType,
            List<PricingCalculationDetail> result,
            Product product,
            decimal quantity,
            DateTime productDate,
            SourceDetail sourceItem)
        {
            int stripLength = PeriodDuration(productDateType);
            DateTime productEndDate = productDate.AddMonths(stripLength).AddDays(-1);

            DateTime reportStart = startDate > productDate ? startDate : productDate;
            DateTime reportEnd = endDate < productEndDate ? endDate : productEndDate;

            for (int month = reportStart.Month; month <= reportEnd.Month; month++)
            {
                DailyDiffPrecalcs(
                    cache,
                    context,
                    result,
                    product,
                    quantity,
                    productDate,
                    productDateType,
                    month,
                    sourceItem);
            }
        }

        //private void CalculateCrackSpreadDiff(
        private void DailyDiffPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            List<PricingCalculationDetail> calculations,
            Product product,
            decimal qty,
            DateTime productDate,
            ProductDateType dateType,
            int productMonth,
            SourceDetail detail)
        {
            if (product.ComplexProduct == null)
            {
                product = context.Products.Single(x => x.ProductId == product.ProductId);

                context.Entry(product).Reference(p => p.ComplexProduct).Load();
                context.Entry(product.ComplexProduct).Reference(p => p.ChildProduct1).Load();
                context.Entry(product.ComplexProduct).Reference(p => p.ChildProduct2).Load();
                context.Entry(product.ComplexProduct.ChildProduct1).Reference(p => p.Category).Load();
                context.Entry(product.ComplexProduct.ChildProduct1).Reference(p => p.ExpiryCalendar).Load();
                context.Entry(product.ComplexProduct.ChildProduct1).Reference(p => p.HolidaysCalendar).Load();
                context.Entry(product.ComplexProduct.ChildProduct1).Reference(p => p.OfficialProduct).Load();
                context.Entry(product.ComplexProduct.ChildProduct2).Reference(p => p.Category).Load();
                context.Entry(product.ComplexProduct.ChildProduct2).Reference(p => p.ExpiryCalendar).Load();
                context.Entry(product.ComplexProduct.ChildProduct2).Reference(p => p.HolidaysCalendar).Load();
                context.Entry(product.ComplexProduct.ChildProduct2).Reference(p => p.OfficialProduct).Load();
            }

            List<DateTime> mergedHolidays = null;
            if (product.ComplexProduct.PricingType == PricingType.NonStandard)
            {
                DateTime currentMonthStart = new DateTime(productDate.Year, productMonth, 1);
                DateTime currentMonthEnd = new DateTime(
                    productDate.Year,
                    productMonth,
                    DateTime.DaysInMonth(productDate.Year, productMonth));

                Product leg1Product = product.ComplexProduct.ChildProduct1;

                IQueryable<CalendarExpiryDate> calendarExpiryDates = cache == null
                    ? context.CalendarExpiryDates
                    : cache.CalendarExpiryDates.AsQueryable();
                IQueryable<CalendarHoliday> calendarHolidays = cache == null
                    ? context.CalendarHolidays
                    : cache.CalendarHolidays.AsQueryable();

                CalendarExpiryDate leg1ExpiryDate =
                    calendarExpiryDates.First(
                        h =>
                            (h.CalendarId == leg1Product.ExpiryCalendar.CalendarId)
                            && (h.FuturesDate == currentMonthStart));
                IQueryable<CalendarHoliday> leg1Holidays =
                    calendarHolidays.Where(
                        h =>
                            (h.CalendarId == leg1Product.HolidaysCalendar.CalendarId)
                            && (h.HolidayDate >= currentMonthStart) && (h.HolidayDate <= currentMonthEnd));

                Product leg2Product = product.ComplexProduct.ChildProduct2;

                CalendarExpiryDate leg2ExpiryDate =
                    calendarExpiryDates.First(
                        h =>
                            (h.CalendarId == leg2Product.ExpiryCalendar.CalendarId)
                            && (h.FuturesDate == currentMonthStart));
                IQueryable<CalendarHoliday> leg2Holidays =
                    calendarHolidays.Where(
                        h =>
                            (h.CalendarId == leg2Product.HolidaysCalendar.CalendarId)
                            && (h.HolidayDate >= currentMonthStart) && (h.HolidayDate <= currentMonthEnd));

                mergedHolidays =
                    leg1Holidays.Select(x => x.HolidayDate).Union(leg2Holidays.Select(x => x.HolidayDate)).ToList();

                if (mergedHolidays.Any(x => (x == leg1ExpiryDate.ExpiryDate) || (x == leg2ExpiryDate.ExpiryDate)))
                {
                    ErrorReportingHelper.GlobalQueue.Enqueue(
                        new Error(
                            "Pricing Report",
                            ErrorType.CalculationError,
                            string.Format(
                                "Product {0} {1} uses non-standard pricing model but one of its legs has a holiday on expiry date of another leg",
                                product.Name,
                                detail.StripName),
                            product.ProductId.ToString(),
                            product,
                            ErrorLevel.Critical));

                    return;
                }
            }

            CalculateComplexProductLeg(
                cache,
                context,
                calculations,
                product,
                qty,
                productDate,
                dateType,
                LegType.Leg1,
                productMonth,
                detail,
                mergedHolidays);
            CalculateComplexProductLeg(
                cache,
                context,
                calculations,
                product,
                qty,
                productDate,
                dateType,
                LegType.Leg2,
                productMonth,
                detail,
                mergedHolidays);
        }

        private void CalculateComplexProductLeg(
            CalculationCache cache,
            MandaraEntities context,
            List<PricingCalculationDetail> calculations,
            Product product,
            decimal productQty,
            DateTime productDate,
            ProductDateType dateType,
            LegType legType,
            int productMonth,
            SourceDetail detail,
            List<DateTime> mergedHolidayDays)
        {
            Product legProduct = legType == LegType.Leg1
                ? product.ComplexProduct.ChildProduct1
                : product.ComplexProduct.ChildProduct2;
            decimal legSignFactor = legType == LegType.Leg1 ? 1M : -1M;
            decimal legFactor = legType == LegType.Leg1
                ? product.ComplexProduct.ConversionFactor1.Value
                : product.ComplexProduct.ConversionFactor2.Value;
            decimal legContractSizeMultiplier = ContractSizeCalculator.ApplyContractSizeMultiplier(
                1M,
                product.ContractSizeMultiplier,
                productDate.Year,
                productMonth);
            decimal adjustedContractSize = product.ContractSize * legContractSizeMultiplier;

            switch (legProduct.Type)
            {
                case ProductType.TradeMonthSwap:
                {
                    ComplexLegTradeMonthSwapPrecalcs(
                        cache,
                        context,
                        calculations,
                        productQty,
                        productDate,
                        productMonth,
                        detail,
                        mergedHolidayDays,
                        legSignFactor,
                        legFactor,
                        adjustedContractSize,
                        legProduct);
                }
                break;

                case ProductType.FuturesBasedSwap:
                {
                    ComplexLegFutureBasedSwapPrecalcs(
                        cache,
                        context,
                        calculations,
                        product,
                        productQty,
                        productDate,
                        dateType,
                        productMonth,
                        detail,
                        mergedHolidayDays,
                        legSignFactor,
                        legFactor,
                        adjustedContractSize,
                        legProduct);
                }
                break;

                case ProductType.Swap:
                {
                    ComplexLegSwapPrecalcs(
                        cache,
                        context,
                        calculations,
                        productQty,
                        productDate,
                        dateType,
                        productMonth,
                        detail,
                        mergedHolidayDays,
                        legSignFactor,
                        legFactor,
                        adjustedContractSize,
                        legProduct);
                }
                break;

                case ProductType.Balmo:
                {
                    ComplexLegBalmoPrecalcs(
                        cache,
                        context,
                        calculations,
                        productQty,
                        productDate,
                        detail,
                        legSignFactor,
                        legFactor,
                        adjustedContractSize,
                        legProduct);
                }
                break;

                case ProductType.Futures:
                {
                    ComplexLegFuturesPrecalcs(
                        cache,
                        calculations,
                        product,
                        productQty,
                        productDate,
                        dateType,
                        detail,
                        legProduct,
                        legSignFactor,
                        legFactor);
                }
                break;
            }
        }

        private void ComplexLegTradeMonthSwapPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            List<PricingCalculationDetail> calculations,
            decimal productQty,
            DateTime productDate,
            int productMonth,
            SourceDetail detail,
            List<DateTime> mergedHolidayDays,
            decimal legSignFactor,
            decimal legFactor,
            decimal adjustedContractSize,
            Product legProduct)
        {
            decimal tradeMonthQty = productQty * legSignFactor * legFactor * adjustedContractSize;

            CalculateTradeMonthSwap(
                cache,
                context,
                calculations,
                legProduct,
                tradeMonthQty,
                productDate.Year,
                productMonth,
                detail,
                true,
                true,
                mergedHolidayDays);
        }

        private void ComplexLegFutureBasedSwapPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            List<PricingCalculationDetail> calculations,
            Product product,
            decimal productQty,
            DateTime productDate,
            ProductDateType dateType,
            int productMonth,
            SourceDetail detail,
            List<DateTime> mergedHolidayDays,
            decimal legSignFactor,
            decimal legFactor,
            decimal adjustedContractSize,
            Product legProduct)
        {
            decimal posConvFactor = product.PositionFactor == null ? 1 : product.PositionFactor.Value;
            decimal qtyCrudeSwap = productQty * legSignFactor * legFactor * adjustedContractSize * posConvFactor;

            if (dateType == ProductDateType.Day)
            {
                CalculateBalmo(
                    cache,
                    context,
                    calculations,
                    legProduct,
                    qtyCrudeSwap,
                    productDate,
                    detail,
                    legProduct,
                    true,
                    true,
                    mergedHolidayDays);
            }
            else
            {
                CalculateCrudeSwap(
                    cache,
                    context,
                    calculations,
                    legProduct,
                    qtyCrudeSwap,
                    productDate.Year,
                    productMonth,
                    detail,
                    true,
                    true,
                    null,
                    null,
                    mergedHolidayDays,
                    product);
            }
        }

        private void ComplexLegSwapPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            List<PricingCalculationDetail> calculations,
            decimal productQty,
            DateTime productDate,
            ProductDateType dateType,
            int productMonth,
            SourceDetail detail,
            List<DateTime> mergedHolidayDays,
            decimal legSignFactor,
            decimal legFactor,
            decimal adjustedContractSize,
            Product legProduct)
        {
            decimal dateScaleFactor = 1;
            decimal qty = productQty * legSignFactor * legFactor * dateScaleFactor * adjustedContractSize;

            if (dateType == ProductDateType.Day)
            {
                CalculateBalmo(
                    cache,
                    context,
                    calculations,
                    legProduct,
                    qty,
                    productDate,
                    detail,
                    null,
                    true,
                    true,
                    mergedHolidayDays);
            }
            else
            {
                CalculateSwap(
                    cache,
                    context,
                    calculations,
                    legProduct,
                    qty,
                    productDate.Year,
                    productMonth,
                    detail,
                    true,
                    true,
                    mergedHolidayDays);
            }
        }

        private void ComplexLegBalmoPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            List<PricingCalculationDetail> calculations,
            decimal productQty,
            DateTime productDate,
            SourceDetail detail,
            decimal legSignFactor,
            decimal legFactor,
            decimal adjustedContractSize,
            Product legProduct)
        {
            decimal dateScaleFactorBalmo = 1;
            decimal qtyBalmo = productQty * legSignFactor * legFactor * dateScaleFactorBalmo * adjustedContractSize;

            CalculateBalmo(
                cache,
                context,
                calculations,
                legProduct,
                qtyBalmo,
                productDate,
                detail,
                null,
                true,
                true);
        }

        private static void ComplexLegFuturesPrecalcs(
            CalculationCache cache,
            List<PricingCalculationDetail> calculations,
            Product product,
            decimal productQty,
            DateTime productDate,
            ProductDateType dateType,
            SourceDetail detail,
            Product legProduct,
            decimal legSignFactor,
            decimal legFactor)
        {
            DateTime startFuturesDate = productDate;
            DateTime endFuturesDate = ComplexFuturesEndDate(productDate, dateType, detail, startFuturesDate);
            DateTime monthIdx = startFuturesDate;

            while (monthIdx < endFuturesDate)
            {
                DateTime expiryDate = CalculationManager.GetExpiryDate(
                    cache,
                    detail,
                    legProduct,
                    monthIdx.Year,
                    monthIdx.Month);
                decimal amount = ContractAmount(
                    product,
                    productQty,
                    legProduct,
                    legSignFactor,
                    legFactor,
                    monthIdx);

                calculations.Add(
                    new PricingCalculationDetail
                    {
                        Amount = amount,
                        CalculationDate = expiryDate,
                        Product = legProduct.Name,
                        ProductReference = legProduct,
                        ProductId = legProduct.ProductId,
                        Period = monthIdx
                    });

                monthIdx = monthIdx.AddMonths(1);
            }
        }

        private static DateTime ComplexFuturesEndDate(
            DateTime productDate,
            ProductDateType dateType,
            SourceDetail detail,
            DateTime startFuturesDate)
        {
            switch (dateType)
            {
                case ProductDateType.Quarter:
                case ProductDateType.Year:
                {
                    return startFuturesDate.AddMonths(PeriodDuration(dateType));
                }

                case ProductDateType.Custom:
                {
                    return detail.TradeEndDate;
                }

                default:
                {
                    return productDate.AddMonths(1);
                }
            }
        }

        private static decimal ContractAmount(
            Product product,
            decimal productQty,
            Product legProduct,
            decimal legSignFactor,
            decimal legFactor,
            DateTime monthIdx)
        {
            decimal baseAmount = productQty * legSignFactor * legFactor * legProduct.ContractSize;
            decimal amount = ContractSizeCalculator.ApplyContractSizeMultiplier(
                baseAmount,
                product.ContractSizeMultiplier,
                monthIdx.Year,
                monthIdx.Month);
            return amount;
        }

        private void CustomDiffPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            DateTime startDate,
            DateTime endDate,
            ProductDateType productDateType,
            List<PricingCalculationDetail> result,
            Product product,
            decimal quantity,
            DateTime productDate,
            SourceDetail sourceItem)
        {
            DateTime prStartDate = productDate;
            DateTime prEndDate = sourceItem.TradeEndDate;

            DateTime customStart = startDate > prStartDate ? startDate : prStartDate;
            DateTime customEnd = endDate < prEndDate ? endDate : prEndDate;

            int numMonths = customEnd.MonthsSince(customStart) + 1;

            for (int i = 0; i < numMonths; i++)
            {
                DateTime date = customStart.AddMonths(i);
                int month = date.Month;

                DailyDiffPrecalcs(
                    cache,
                    context,
                    result,
                    product,
                    quantity,
                    date,
                    productDateType,
                    month,
                    sourceItem);
            }
        }

        private void BalmoPrecalcs(
            CalculationCache cache,
            MandaraEntities context,
            ProductDateType productDateType,
            SourceDetail sourceItem,
            Product product,
            List<PricingCalculationDetail> result,
            decimal quantity,
            DateTime productDate)
        {
            switch (productDateType)
            {
                case ProductDateType.Custom:
                case ProductDateType.MonthYear:
                case ProductDateType.Quarter:
                case ProductDateType.Year:
                ErrorReportingHelper.GlobalQueue.Enqueue(
                    new Error(
                        "Pricing Report",
                        ErrorType.CalculationError,
                        "Source data is incorrect. Date type must be ProductDateType.Day for balmo.",
                        null,
                        sourceItem,
                        ErrorLevel.Critical));
                break;

                case ProductDateType.Day:
                if (product.BalmoOnComplexProduct != null)
                {
                    DailyDiffPrecalcs(
                        cache,
                        context,
                        result,
                        product.BalmoOnComplexProduct,
                        quantity,
                        productDate,
                        productDateType,
                        productDate.Month,
                        sourceItem);
                }
                else
                {
                    CalculateBalmo(
                        cache,
                        context,
                        result,
                        product,
                        quantity,
                        productDate,
                        sourceItem,
                        product.BalmoOnCrudeProduct);
                }

                break;
            }
        }

        private static void FuturesPreCalcs(
            CalculationCache cache,
            DateTime productDate,
            ProductDateType productDateType,
            SourceDetail sourceItem,
            Product product,
            decimal quantity,
            List<PricingCalculationDetail> result)
        {
            DateTime startFuturesDate = productDate;
            DateTime endFuturesDate = productDate.AddMonths(1);

            switch (productDateType)
            {
                case ProductDateType.Quarter:
                case ProductDateType.Year:
                {
                    endFuturesDate = startFuturesDate.AddMonths(PeriodDuration(productDateType));
                }
                break;

                case ProductDateType.Custom:
                {
                    endFuturesDate = sourceItem.TradeEndDate;
                }
                break;
            }

            DateTime monthIdx = startFuturesDate;

            while (monthIdx < endFuturesDate)
            {
                DateTime expiryDate = CalculationManager.GetExpiryDate(
                    cache,
                    sourceItem,
                    product,
                    monthIdx.Year,
                    monthIdx.Month);

                decimal baseAmount = quantity * product.ContractSize * (product.PositionFactor ?? 1M);
                decimal amount = ContractSizeCalculator.ApplyContractSizeMultiplier(
                    baseAmount,
                    product.ContractSizeMultiplier,
                    monthIdx.Year,
                    monthIdx.Month);

                result.Add(
                    new PricingCalculationDetail
                    {
                        Amount = amount,
                        CalculationDate = expiryDate,
                        Product = product.Name,
                        ProductReference = product,
                        ProductId = product.ProductId,
                        Period = monthIdx
                    });

                monthIdx = monthIdx.AddMonths(1);
            }
        }

        private void ReportCalculationErrorButWhatHappened(Exception ex, SourceDetail sourceItem)
        {
            _logger.Error(
                ex,
                "PricingPrePositionsManager: Could not calculate pricing positions for trade [{0}]",
                sourceItem.TradeCaptureId);

            ErrorReportingHelper.GlobalQueue.Enqueue(
                new Error(
                    "IRM Server",
                    ErrorType.Exception,
                    string.Format(
                        "Could not calculate pricing positions for trade id [{0}]",
                        sourceItem.TradeCaptureId),
                    sourceItem.TradeCaptureId.ToString(),
                    ex,
                    ErrorLevel.Critical));

            ErrorReportingHelper.GlobalQueue.Enqueue(
                new Error(
                    "IRM Server",
                    ErrorType.TradeError,
                    string.Format(
                        "Could not calculate pricing positions for trade id [{0}]",
                        sourceItem.TradeCaptureId),
                    sourceItem.TradeCaptureId.ToString(),
                    sourceItem.TradeCapture,
                    ErrorLevel.Critical));
        }

        private void CalculateCrudeSwap(
            CalculationCache cache,
            MandaraEntities context,
            List<PricingCalculationDetail> calculations,
            Product product,
            decimal qty,
            int productYear,
            int productMonth,
            SourceDetail detail,
            bool suppressPosConvFactor = false,
            bool suppressContractSize = false,
            int? balmoCorrection = null,
            int? balmoProductDay = null,
            List<DateTime> mergedHolidayDays = null,
            Product sourceProduct = null)
        {
            DateTime currentMonthStart = new DateTime(productYear, productMonth, 1);
            DateTime currentMonthEnd = new DateTime(
                productYear,
                productMonth,
                DateTime.DaysInMonth(productYear, productMonth));

            List<DateTime> calendarHolidays;

            if (mergedHolidayDays != null)
            {
                calendarHolidays = mergedHolidayDays;
            }
            else
            {
                if (cache == null)
                {
                    calendarHolidays =
                        context.CalendarHolidays.Where(h => h.CalendarId == product.HolidaysCalendar.CalendarId)
                               .Select(x => x.HolidayDate)
                               .ToList();
                }
                else
                {
                    calendarHolidays = new List<DateTime>();
                    SortedSet<CalendarHoliday> holidaysSet;
                    if (cache.CalendarHolidaysMap.TryGetValue(product.HolidaysCalendar.CalendarId, out holidaysSet))
                    {
                        calendarHolidays = holidaysSet.Select(it => it.HolidayDate).ToList();
                    }
                }
            }

            if (cache == null)
            {
                cache = new CalculationCache();
                cache.Initialize(context);
            }

            List<DateTime> intervals = new List<DateTime>();
            intervals.Add(currentMonthStart);

            List<DateTime> pricingMonths = new List<DateTime>();

            Dictionary<DateTime, CalendarExpiryDate> dates = cache.ExpiryDatesMap[product.ExpiryCalendar.CalendarId];
            Dictionary<DateTime, CalendarExpiryDate> byMonths =
                cache.ExpiryDatesByMonthsMap[product.ExpiryCalendar.CalendarId];
            CalendarExpiryDate calendarExpiryDate =
                byMonths[new DateTime(currentMonthStart.Year, currentMonthStart.Month, 1)];

            intervals.Add(calendarExpiryDate.ExpiryDate);
            pricingMonths.Add(calendarExpiryDate.FuturesDate);

            CalendarExpiryDate expDate = calendarExpiryDate;
            do
            {
                expDate = dates[expDate.FuturesDate.AddMonths(1)];
                pricingMonths.Add(expDate.FuturesDate);

                if (expDate.ExpiryDate.Month == calendarExpiryDate.ExpiryDate.Month)
                {
                    intervals.Add(expDate.ExpiryDate);
                }
            }
            while (expDate.ExpiryDate.Month == calendarExpiryDate.ExpiryDate.Month);

            intervals.Add(currentMonthEnd);

            int correction = product.ExpiryCalendar.Correction == null ? 0 : product.ExpiryCalendar.Correction.Value;
            int[] businessDaysIntervals = new int[intervals.Count - 1];

            for (int i = 0; i < businessDaysIntervals.Length; i++)
            {
                DateTime intervalStartDate = intervals[i];
                DateTime intervalEndDate = intervals[i + 1];

                int holidaysNmb = calendarHolidays.Count(h => (h >= intervalStartDate) && (h <= intervalEndDate));

                DateTime correctedEndDate = intervalEndDate;
                if (i != businessDaysIntervals.Length - 1)
                // if it's last value (ie currentMonthEnd) we dont need to correct it
                {
                    correctedEndDate = correctedEndDate.AddDays(correction);
                }

                DateTime correctedStartDate = intervalStartDate;
                if (i > 0)
                // when measure business days for the second (and subsequent) interval we need to cut one day that was added to previous interval
                {
                    correctedStartDate = correctedStartDate.AddDays(correction).AddDays(1);
                }

                int businessDaysNmb = GetBusinessDaysCount(correctedStartDate, correctedEndDate) - holidaysNmb;
                businessDaysIntervals[i] = businessDaysNmb;
            }

            int totalBusinessDays = businessDaysIntervals.Sum();
            int businessDaysElapsed = 0;

            if (balmoCorrection != null)
            {
                totalBusinessDays -= balmoCorrection.Value;

                if (businessDaysElapsed < balmoCorrection.Value)
                {
                    businessDaysElapsed = balmoCorrection.Value;
                }
            }

            businessDaysIntervals[0] -= businessDaysElapsed;

            if (businessDaysIntervals[0] < 0)
            {
                businessDaysIntervals[1] += businessDaysIntervals[0];
                businessDaysIntervals[0] = 0;
            }

            decimal positionFactor = suppressPosConvFactor
                ? 1
                : product.PositionFactor == null ? 1 : product.PositionFactor.Value;
            decimal contractSize = suppressContractSize ? 1 : product.ContractSize;

            int day = balmoProductDay == null ? 1 : balmoProductDay.Value;

            DateTime calculationDate;
            decimal[] coefficients = new decimal[businessDaysIntervals.Length];

            for (int i = 0; i < businessDaysIntervals.Length; i++)
            {
                decimal coeff = (decimal)businessDaysIntervals[i] / totalBusinessDays;

                if (coeff < 0M)
                {
                    coeff = 0M;
                }
                if (coeff > 1M)
                {
                    coeff = 1M;
                }

                coefficients[i] = coeff;

                DateTime productDate = pricingMonths[i];
                decimal baseAmount = qty * positionFactor * coeff * contractSize;
                decimal amount =
                    suppressContractSize
                    ? baseAmount
                    : ContractSizeCalculator.ApplyContractSizeMultiplier(
                        baseAmount,
                        product.ContractSizeMultiplier,
                        productYear,
                        productMonth);

                if (businessDaysIntervals[i] > 0)
                {
                    for (int j = 1; j <= businessDaysIntervals[i]; j++)
                    {
                        do
                        {
                            calculationDate = new DateTime(productYear, productMonth, day);
                            day++;
                        }
                        while (!IsBusinessDay(calculationDate, calendarHolidays));

                        decimal amountAtDay = amount / businessDaysIntervals[i];

                        PricingCalculationDetail calculation = GetPricingCalculationDetail(
                            calculations,
                            product,
                            productDate,
                            calculationDate,
                            detail,
                            amountAtDay);

                        calculation.Amount += amountAtDay;
                    }
                }
            }
        }

        private void CalculateSwap(
            CalculationCache cache,
            MandaraEntities context,
            List<PricingCalculationDetail> calculations,
            Product product,
            decimal qty,
            int productYear,
            int productMonth,
            SourceDetail detail,
            bool suppressPosConvFactor = false,
            bool suppressContractSize = false,
            List<DateTime> mergedHolidays = null)
        {
            DateTime currentMonthStart = new DateTime(productYear, productMonth, 1);
            DateTime currentMonthEnd = new DateTime(
                productYear,
                productMonth,
                DateTime.DaysInMonth(productYear, productMonth));

            DateTime period = currentMonthStart;

            if (product.UseExpiryCalendar.HasValue && product.UseExpiryCalendar.Value)
            {
                // we need to use next business day as a start date (with respect to weekends and holidays)
                DateTime realStartDate = CalculationManager.GetExpiryDate(
                    cache,
                    detail,
                    product,
                    currentMonthStart.AddMonths(-1).Year,
                    currentMonthStart.AddMonths(-1).Month);
                DateTime realEndDate = CalculationManager.GetExpiryDate(
                    cache,
                    detail,
                    product,
                    currentMonthStart.Year,
                    currentMonthStart.Month);

                if ((realStartDate != DateTime.MinValue) && (realEndDate != DateTime.MinValue))
                {
                    currentMonthStart = realStartDate;
                    currentMonthEnd = realEndDate;
                }
            }

            List<DateTime> holidays;
            if (mergedHolidays == null)
            {
                List<CalendarHoliday> calendarHolidays;

                if (cache == null)
                {
                    DbSet<CalendarHoliday> calendarContext = context.CalendarHolidays;
                    calendarHolidays =
                        calendarContext.Where(
                            h =>
                                (h.CalendarId == product.HolidaysCalendar.CalendarId)
                                && (h.HolidayDate >= currentMonthStart) && (h.HolidayDate <= currentMonthEnd)).ToList();
                }
                else
                {
                    calendarHolidays = cache.HolidaysBetweenDates(
                        product.HolidaysCalendar.CalendarId,
                        currentMonthStart,
                        currentMonthEnd,
                        true);
                }

                holidays = calendarHolidays.Select(x => x.HolidayDate).ToList();
            }
            else
            {
                holidays = mergedHolidays.Where(x => (currentMonthStart <= x) && (x <= currentMonthEnd)).ToList();
            }

            List<DateTime> days = GetSwapDays(currentMonthStart, currentMonthEnd, holidays, product.IsCalendarDaySwap);
            int businessDaysTotal = days.Count();

            decimal positionFactor = suppressPosConvFactor
                ? 1
                : product.PositionFactor == null ? 1 : product.PositionFactor.Value;
            decimal contractSize = suppressContractSize ? 1 : product.ContractSize;

            decimal baseAmount = qty * positionFactor * contractSize;
            decimal amount = suppressContractSize
                ? baseAmount
                : ContractSizeCalculator.ApplyContractSizeMultiplier(
                    baseAmount,
                    product.ContractSizeMultiplier,
                    productYear,
                    productMonth);

            foreach (DateTime calculationDate in days)
            {
                decimal amountAtDay = amount / businessDaysTotal;

                PricingCalculationDetail calculation = GetPricingCalculationDetail(
                    calculations,
                    product,
                    period,
                    calculationDate,
                    detail,
                    amountAtDay);

                calculation.Amount += amountAtDay;
            }
        }

        private void CalculateBalmo(
            CalculationCache cache,
            MandaraEntities context,
            List<PricingCalculationDetail> calculations,
            Product product,
            decimal qty,
            DateTime productDate,
            SourceDetail detail,
            Product balmoOnCrudeSwapProduct = null,
            bool suppressBalmoFactor = false,
            bool suppressPositionFactor = false,
            List<DateTime> mergedHolidays = null)
        {
            int productYear = productDate.Year;
            int productMonth = productDate.Month;

            DateTime currentMonthStart = new DateTime(productDate.Year, productDate.Month, 1);
            DateTime currentMonthEnd = new DateTime(
                productYear,
                productMonth,
                DateTime.DaysInMonth(productYear, productMonth));

            if (product.Type == ProductType.TradeMonthSwap)
            {
                DateTime realStartDate = CalculationManager.GetExpiryDate(
                    cache,
                    detail,
                    product,
                    currentMonthStart.AddMonths(-1).Year,
                    currentMonthStart.AddMonths(-1).Month);
                DateTime realEndDate = CalculationManager.GetExpiryDate(
                    cache,
                    detail,
                    product,
                    currentMonthStart.Year,
                    currentMonthStart.Month);

                if (realStartDate != DateTime.MinValue && realEndDate != DateTime.MinValue)
                {
                    currentMonthStart = realStartDate;
                    currentMonthEnd = realEndDate;
                }
            }

            IQueryable<CalendarHoliday> calendarContext = cache == null
                ? context.CalendarHolidays
                : cache.CalendarHolidays.AsQueryable();

            List<DateTime> holidays;
            if (mergedHolidays == null)
            {
                List<CalendarHoliday> calendarHolidays =
                    calendarContext.Where(
                        h =>
                            (h.CalendarId == product.HolidaysCalendar.CalendarId)
                            && (h.HolidayDate >= currentMonthStart) && (h.HolidayDate <= currentMonthEnd)).ToList();
                holidays = calendarHolidays.Select(x => x.HolidayDate).ToList();
            }
            else
            {
                holidays = mergedHolidays.Where(x => (currentMonthStart <= x) && (x <= currentMonthEnd)).ToList();
            }

            DateTime balmoPricingDate = productDate;
            List<DateTime> days = GetSwapDays(productDate, currentMonthEnd, holidays, product.IsCalendarDaySwap);
            int daysTotal = days.Count();

            int holidaysBeforePricingDate = holidays.Count(h => (h >= currentMonthStart) && (h < balmoPricingDate));
            int pricingBusinessDay = GetBusinessDaysCount(currentMonthStart, balmoPricingDate)
                                     - holidaysBeforePricingDate;

            decimal positionFactor = suppressBalmoFactor
                ? 1M
                : product.PositionFactor == null ? 1M : product.PositionFactor.Value;
            decimal contractSize = suppressPositionFactor ? 1M : product.ContractSize;

            decimal baseAmount = qty * positionFactor * contractSize;
            decimal amount = suppressPositionFactor
                ? baseAmount
                : ContractSizeCalculator.ApplyContractSizeMultiplier(
                    baseAmount,
                    product.ContractSizeMultiplier,
                    productYear,
                    productMonth);

            if (balmoOnCrudeSwapProduct == null)
            {
                foreach (DateTime calculationDate in days)
                {
                    decimal amountAtDay = amount / daysTotal;

                    DateTime period = currentMonthStart;
                    if (product.Type == ProductType.TradeMonthSwap)
                    {
                        period = new DateTime(calculationDate.Year, calculationDate.Month, 1);
                    }

                    PricingCalculationDetail calculation = GetPricingCalculationDetail(
                        calculations,
                        product,
                        period,
                        calculationDate,
                        detail,
                        amountAtDay);

                    calculation.Amount += amountAtDay;
                }
            }
            else
            {
                CalculateCrudeSwap(
                    cache,
                    context,
                    calculations,
                    balmoOnCrudeSwapProduct,
                    amount,
                    productYear,
                    productMonth,
                    detail,
                    true,
                    true,
                    pricingBusinessDay - 1,
                    balmoPricingDate.Day,
                    mergedHolidays);
            }
        }


        private PricingCalculationDetail GetPricingCalculationDetail(
            List<PricingCalculationDetail> calculations,
            Product product,
            DateTime period,
            DateTime calculationDate,
            SourceDetail detail,
            decimal amountAtDay)
        {
            IEnumerable<PricingCalculationDetail> q = from c in calculations
                                                      where
                                                          (c.ProductId == product.ProductId) && (c.Period == period)
                                                          && (c.CalculationDate == calculationDate)
                                                      select c;

            if (q.Count() > 0)
            {
                PricingCalculationDetail calculationDetail = q.Single();

                AddSourceDetailToPricingCalculationDetail(calculationDetail, detail, amountAtDay);

                return calculationDetail;
            }

            PricingCalculationDetail calculation = new PricingCalculationDetail
            {
                DetailId = Guid.NewGuid(),
                Period = period,
                CalculationDate = calculationDate,
                Product = product.Name,
                Source = product.Name,
                ProductCategory = product.Category == null ? product.Name : product.Category.Name,
                Amount = 0,
                ProductId = product.ProductId,
                ProductReference = product
            };

            AddSourceDetailToPricingCalculationDetail(calculation, detail, amountAtDay);

            calculations.Add(calculation);

            return calculation;
        }

        private void AddSourceDetailToPricingCalculationDetail(
            PricingCalculationDetail calculation,
            SourceDetail sourceDetail,
            decimal amountAtDay)
        {
            if (calculation.SourceDetails == null)
            {
                calculation.SourceDetails = new List<SourceDetail>();
            }

            if (calculation.SourceDetailAmountsDict == null)
            {
                calculation.SourceDetailAmountsDict = new Dictionary<int, decimal>();
            }

            if (calculation.SourceDetailAmountsDict.ContainsKey(sourceDetail.SourceDetailId))
            {
                decimal sdAmount;
                int key = sourceDetail.SourceDetailId;

                if (calculation.SourceDetailAmountsDict.TryGetValue(key, out sdAmount))
                {
                    calculation.SourceDetailAmountsDict.Remove(key);
                    calculation.SourceDetailAmountsDict.Add(key, sdAmount + amountAtDay);
                }
            }
            else
            {
                calculation.SourceDetails.Add(sourceDetail);
                calculation.SourceDetailAmountsDict.Add(sourceDetail.SourceDetailId, amountAtDay);
            }
        }

        private List<DateTime> GetSwapDays(
            DateTime start,
            DateTime end,
            List<DateTime> holidays,
            bool isCalendarDaySwap)
        {
            List<DateTime> result = new List<DateTime>();

            if (isCalendarDaySwap)
            {
                DateTime calculationDate = start.Date;
                DateTime endDate = end.Date;
                while (calculationDate <= endDate)
                {
                    result.Add(calculationDate);
                    calculationDate = calculationDate.AddDays(1);
                }
            }
            else
            {
                return GetBusinessDaysList(start, end, holidays);
            }

            return result;
        }

        private int GetBusinessDaysCount(DateTime start, DateTime end)
        {
            List<DateTime> holidays = new List<DateTime>();
            return GetBusinessDaysList(start, end, holidays).Count;
        }

        private List<DateTime> GetBusinessDaysList(DateTime start, DateTime end, List<DateTime> holidays)
        {
            DateTime dateCount = start.Date;
            List<DateTime> workingDays = new List<DateTime>();

            while (dateCount.Date <= end.Date)
            {
                if (IsBusinessDay(dateCount, holidays))
                {
                    workingDays.Add(dateCount);
                }

                dateCount = dateCount.AddDays(1);
            }

            return workingDays;
        }

        private bool IsBusinessDay(DateTime day, List<DateTime> holidays)
        {
            if ((day.DayOfWeek == DayOfWeek.Saturday) || (day.DayOfWeek == DayOfWeek.Sunday) || holidays.Contains(day))
            {
                return false;
            }

            return true;
        }
    }
}