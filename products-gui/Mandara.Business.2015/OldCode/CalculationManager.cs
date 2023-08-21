using JetBrains.Annotations;
using Mandara.Business.Calculators;
using Mandara.Database.Query;
using Mandara.Database.Sql;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Calculation;
using Mandara.Entities.Exceptions;
using Mandara.Entities.Import;
using Optional;
using Optional.Collections;
using Optional.Unsafe;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace Mandara.Business.OldCode
{
    public enum CalendarFailure
    {
        None,
        CalendarNotFound,
        ExpiryDateNotFound
    }

    public class CalculationManager
    {
        private const string LastPxColumnName = "LastPx";
        private const string LastQtyColumnName = "LastQty";

        private static List<string> AliasNotFoundErrors = new List<string>();
        private static Dictionary<int, bool> TasTradesWithZeroPriceErrors = new Dictionary<int, bool>();

        private HashSet<DateTime> _holidays;

        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Lazy<SqlConnectionStringBuilder> PriceDbConnectionString;

        public CalculationManager()
        {
            PriceDbConnectionString = new Lazy<SqlConnectionStringBuilder>(()=> ConnectionString.GetConnectionStringBuild("PriceDatabase"));
        }

        public Dictionary<string, decimal> GetSettlementPrices(DateTime? maxSettlementDate = null)
        {
            try
            {
                return SqlServerCommandExecution
                    .ReadToList(
                        PriceDbConnectionString.Value,
                        (conn) => GetSettlementPricesQuery(maxSettlementDate, conn),
                        GetSettlementPrice).ToDictionary(
                        offProdSettle => offProdSettle.Item1,
                        offProdSettle => offProdSettle.Item2);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Error reading settlement prices");
            }

            return null;
        }

        public ClassicPositionsCalculator CreateClassicPositionCalculator()
        {
            using (var context = CreateMandaraProductsDbContext())
            {
                CalculationCache cache = new CalculationCache();
                cache.Initialize(context);
                ClassicPositionsCalculator positionsCalculator = new ClassicPositionsCalculator(context, cache);
                return positionsCalculator;
            }
        }

        private static (string, decimal) GetSettlementPrice(SqlDataReader reader)
        {
            int officialProductId = reader.GetInt32(0);
            DateTime settlementDate = reader.GetDateTime(1);
            decimal price = reader.GetDecimal(2);

            return (officialProductId + settlementDate.ToShortDateString(), price);
        }

        private static SqlCommand GetSettlementPricesQuery(DateTime? maxSettlementDate, SqlConnection conn)
        {
            string sql = "SELECT * FROM dbo.settlement_prices ";
            SqlParameter dateParam = null;

            if (maxSettlementDate != null)
            {
                sql += "WHERE settlement_date <= @MaxSettlementDate";
                dateParam = new SqlParameter("@MaxSettlementDate", maxSettlementDate.Value);
            }

            SqlCommand comm = new SqlCommand(sql, conn);

            if (dateParam != null)
            {
                comm.Parameters.Add(dateParam);
            }
            return comm;
        }

        public decimal? GetLivePriceForProduct(DataTable pricesTable, DateTime productDate,
                                               ProductDateType priceDateType, string mappingColumn,
                                               object tradeStartDateObject = null, object tradeEndDateObject = null)
        {
            decimal? price = 0;
            int productMonth = productDate.MonthsSince(SystemTime.Now()) + 72;

            if (mappingColumn == null)
            {
                return null;
            }

            switch (priceDateType)
            {
                case ProductDateType.Day:
                case ProductDateType.MonthYear:
                    {
                        if (productMonth < 0)
                        {
                            return null;
                        }

                        DataRow priceRow = pricesTable.Rows.Find(new object[] { productMonth });
                        price = priceRow == null ? null :
                            priceRow.IsNull(mappingColumn) ? null :
                            (decimal?)priceRow.Field<double>(mappingColumn);
                    }
                    break;

                case ProductDateType.Quarter:
                    {
                        string expression = string.Format("AVG({0})", mappingColumn);
                        string filter = string.Format("rdate >= {0} AND rdate <= {1}", productMonth, productMonth + 2);
                        object quarterPriceObj = pricesTable.Compute(expression, filter);
                        price = quarterPriceObj is DBNull ? null : (decimal?)(double)quarterPriceObj;
                    }
                    break;

                case ProductDateType.Year:
                    {
                        string expression = string.Format("AVG({0})", mappingColumn);
                        string filter = string.Format("rdate >= {0} AND rdate <= {1}", productMonth, productMonth + 11);
                        object yearPriceObj = pricesTable.Compute(expression, filter);
                        price = yearPriceObj is DBNull ? null : (decimal?)(double)yearPriceObj;
                    }
                    break;

                case ProductDateType.Custom:
                    {
                        DateTime? tradeStartDate = StripHelper.ParseDate(tradeStartDateObject, Formats.SortableShortDate);
                        DateTime? tradeEndDate = StripHelper.ParseDate(tradeEndDateObject, Formats.SortableShortDate);

                        if (tradeStartDate.HasValue && tradeEndDate.HasValue)
                        {
                            int numAddOnMonths = tradeEndDate.Value.MonthsSince(tradeStartDate.Value);

                            productMonth = tradeStartDate.Value.MonthsSince(SystemTime.Now()) + 72;

                            string expression = string.Format("AVG({0})", mappingColumn);
                            string filter = string.Format(
                                "rdate >= {0} AND rdate <= {1}",
                                productMonth,
                                productMonth + numAddOnMonths);
                            object customPriceObj = pricesTable.Compute(expression, filter);
                            price = customPriceObj is DBNull ? null : (decimal?)(double)customPriceObj;
                        }
                    }
                    break;
            }
            return price;
        }

        private double GetPriceFromExternalPrices(SourceDetail sourceItem, List<ProductPriceDetail> externalPrices)
        {
            double price = 0;

            switch (sourceItem.DateType)
            {
                case ProductDateType.Day:
                case ProductDateType.MonthYear:
                    {
                        DateTime productDateAtMonth =
                            new DateTime(sourceItem.ProductDate.Year, sourceItem.ProductDate.Month, 1);

                        ProductPriceDetail productPriceDetailAtMonth =
                            externalPrices.Where(
                                x => x.OfficialProduct.OfficialProductId ==
                                        sourceItem.Product.OfficialProduct.OfficialProductId &&
                                     x.Date.Date == productDateAtMonth).
                                SingleOrDefault();

                        if (productPriceDetailAtMonth == null)
                        {
                            return 0;
                        }

                        price = Convert.ToDouble(productPriceDetailAtMonth.Price);
                    }
                break;

                case ProductDateType.Quarter:
                    {
                        DateTime quarterStart =
                            new DateTime(sourceItem.ProductDate.Year, sourceItem.ProductDate.Month, 1);
                        DateTime quarterEnd = quarterStart.AddMonths(2);

                        List<ProductPriceDetail> productPriceDetailAtQuarter =
                            externalPrices.Where(
                                x => x.OfficialProduct.OfficialProductId ==
                                        sourceItem.Product.OfficialProduct.OfficialProductId &&
                                     x.Date.Date >= quarterStart && x.Date.Date <= quarterEnd).
                                ToList();

                        if (productPriceDetailAtQuarter.Count == 0)
                        {
                            return 0;
                        }

                        price = Convert.ToDouble(productPriceDetailAtQuarter.Average(x => x.Price));
                    }
                    break;

                case ProductDateType.Year:
                    {
                        DateTime yearStart =
                            new DateTime(sourceItem.ProductDate.Year, sourceItem.ProductDate.Month, 1);
                        DateTime yearEnd = yearStart.AddYears(1);

                        List<ProductPriceDetail> productPriceDetailAtYear =
                            externalPrices.Where(
                                x => x.OfficialProduct.OfficialProductId ==
                                        sourceItem.Product.OfficialProduct.OfficialProductId &&
                                     x.Date.Date >= yearStart && x.Date.Date <= yearEnd).
                                ToList();

                        if (productPriceDetailAtYear.Count == 0)
                        {
                            return 0;
                        }

                        price = Convert.ToDouble(productPriceDetailAtYear.Average(x => x.Price));
                    }
                    break;
            }

            return price;
        }

        public DataTable GetPriceData(DateTime snapshotDatetime)
        {
            int totalSeconds = EpochConverter.ToEpochTime(snapshotDatetime);

            return GetPriceData(totalSeconds);
        }

        public DataTable GetPriceData(int priceTimestamp)
        {
            DataTable priceData = new DataTable();

            SqlServerCommandExecution.ExecuteReaderQuery(
                priceData,
                PriceDbConnectionString.Value,
                (conn) => GetPricesQuery(conn, priceTimestamp));
            priceData.PrimaryKey = new DataColumn[] { priceData.Columns["sdate"], priceData.Columns["rdate"] };
            return priceData;
        }

        private static SqlCommand GetPricesQuery(SqlConnection conn, int priceTimestamp)
        {
            SqlCommand comm = new SqlCommand("SELECT * FROM dbo.pnl_PriceView WHERE sdate = @timestamp", conn);
            comm.Parameters.Add(new SqlParameter("@timestamp", priceTimestamp));
            return comm;
        }

        /// <summary>
        /// Calculate live positions for the provided source details.
        /// </summary>
        /// <param name="globalCache">Calculation cache (products, expiry dates, holidays) to use,
        /// if null a new one will be created.</param>
        /// <param name="sourceDetails">SourceDetails for which live positions will be calculated.</param>
        /// <param name="calculationErrors">An output list of calculation errors.</param>
        /// <param name="riskDate">Risk date for which live positions will be calculated.
        /// Default value is current datetime.</param>
        /// <returns>Calculated positions as a CalculationDetail list.</returns>
        public List<CalculationDetail> CalculateLivePositions(
            List<SourceDetail> sourceDetails,
            ClassicPositionsCalculator calculator,
            out List<CalculationError> calculationErrors, DateTime? riskDate = null,
            bool reportErrors = true)
        {
            if (!riskDate.HasValue)
            {
                riskDate = SystemTime.Now();
            }

            return CalculatePositions(
                sourceDetails,
                riskDate.Value,
                calculator,
                out calculationErrors,
                reportErrors);
        }

        private static MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(CalculationManager));
        }

        [Obsolete("Use TryGetExpiryDate which does not throw exceptions.")]
        public static DateTime GetExpiryDate(
            CalculationCache cache,
            SourceDetail sourceDetail,
            Product product,
            int productYear,
            int productMonth,
            bool getNextBusinessDay = false)
        {
            Expiry expiry = TryGetExpiryDate(
                cache,
                sourceDetail,
                product,
                productYear,
                productMonth,
                getNextBusinessDay ? 1 : 0);

            if (!expiry.HasDate())
            {
                switch (expiry.FailureReason)
                {
                    case CalendarFailure.CalendarNotFound:
                    {
                        throw new CalendarNotFoundException(sourceDetail, product.ExpiryCalendar);
                    }

                    case CalendarFailure.ExpiryDateNotFound:
                    {
                        throw new CalendarExpiryDateNotFoundException(
                            sourceDetail,
                            product.ExpiryCalendar,
                            new DateTime(productYear, productMonth, 1));
                    }
                }
            }

            return expiry.Expires;
        }

        private class ExpiryLookupResult
        {
            public CalendarExpiryDate ExpiryInfo { get; }
            public CalendarFailure FailureReason { get; }

            public ExpiryLookupResult(CalendarExpiryDate expiryInfo)
            {
                ExpiryInfo = expiryInfo;
                FailureReason = CalendarFailure.None;
            }

            public ExpiryLookupResult(CalendarFailure failure)
            {
                ExpiryInfo = CalendarExpiryDate.Default;
                FailureReason = failure;
            }
        }

        public static Expiry TryGetExpiryDate(
            CalculationCache cache,
            SourceDetail sourceDetail,
            Product product,
            int productYear,
            int productMonth,
            int offsetToNextBusinessDay)
        {
            ExpiryLookupResult expiry =
                TryGetExpiryDate(cache, product, GetExpiry(productYear, productMonth));

            if (CalendarFailure.None != expiry.FailureReason)
            {
                return new Expiry(expiry.FailureReason);
            }

            List<DateTime> holidays = GetHolidays(cache, product.HolidaysCalendar.CalendarId);
            DateTime expiryDate = GetExpiryBusinessDay(
                holidays,
                expiry.ExpiryInfo.ExpiryDate.AddDays(offsetToNextBusinessDay));

            return new Expiry(expiryDate);
        }

        private static Func<Dictionary<DateTime, CalendarExpiryDate>, ExpiryLookupResult> GetExpiry(
            int productYear,
            int productMonth)
        {
            return (expiryDates) =>
            {
                DateTime expirationMonth = new DateTime(productYear, productMonth, 1);

                if (!expiryDates.TryGetValue(expirationMonth, out CalendarExpiryDate calendarExpiryDate))
                {
                    return new ExpiryLookupResult(CalendarFailure.ExpiryDateNotFound);
                }

                return new ExpiryLookupResult(calendarExpiryDate);
            };
        }

        private static ExpiryLookupResult TryGetExpiryDate(
            CalculationCache cache,
            Product product,
            Func<Dictionary<DateTime, CalendarExpiryDate>, ExpiryLookupResult> getExpiryDate)
        {
            if (!cache.ExpiryDatesMap.TryGetValue(
                product.ExpiryCalendar.CalendarId,
                out Dictionary<DateTime, CalendarExpiryDate> calendarExpiryDates))
            {
                return new ExpiryLookupResult(CalendarFailure.CalendarNotFound);
            }

            return getExpiryDate(calendarExpiryDates);
        }

        private static DateTime GetExpiryBusinessDay(
            List<DateTime> holidays,
            DateTime baseExpiryDate)
        {
            DateTime expiryDate = baseExpiryDate;

            while (expiryDate.IsWeekendDay() || holidays.Contains(expiryDate))
            {
                expiryDate=expiryDate.AddDays(1);
            }

            return expiryDate;
        }

        private static List<DateTime> GetHolidays(CalculationCache cache, int calendarId)
        {
            return cache.CalendarHolidaysMap.TryGetValue(calendarId, out SortedSet<CalendarHoliday> holidaysSet)
                ? holidaysSet.Select(holiday => holiday.HolidayDate).ToList()
                : new List<DateTime>();
        }

        public static Expiry TryGetExpiryInRange(
            CalculationCache cache,
            Product product,
            List<DateTime> possibleExpiries)
        {
            ExpiryLookupResult expiry = TryGetExpiryDate(cache, product, GetExpiryInRange(possibleExpiries));

            return CalendarFailure.None == expiry.FailureReason
                ? new Expiry(expiry.ExpiryInfo.ExpiryDate)
                : new Expiry(expiry.FailureReason);
        }

        private static Func<Dictionary<DateTime, CalendarExpiryDate>, ExpiryLookupResult> GetExpiryInRange(
            List<DateTime> possibleExpiries)
        {
            return (expiryDates) =>
            {
                Option<CalendarExpiryDate> expiryDate = expiryDates.Values.FirstOrNone(
                    expiry => possibleExpiries.Contains(expiry.ExpiryDate));

                return expiryDate.HasValue
                    ? new ExpiryLookupResult(expiryDate.ValueOrFailure())
                    : new ExpiryLookupResult(CalendarFailure.ExpiryDateNotFound);
            };
        }

        public DateTime AdjustRiskDateForHoliday(DateTime riskDate, List<DateTime> holidays)
        {
            while (holidays.Any(x => x.Date == riskDate.Date))
            {
                riskDate = riskDate.AddDays(1);

                // check that we didn't hit weekend after the holiday
                riskDate = AdjustRiskDateForWeekends(riskDate);
            }

            return riskDate;
        }

        public static DateTime AdjustRiskDateForProduct(DateTime riskDate, Product product)
        {
            DateTime rd = riskDate;

            if (product.LocalRolloffTime.HasValue && product.RolloffTimeToday.HasValue)
            {
                if (product.LocalRolloffTime.Value.Date == product.RolloffTimeToday.Value.Date)
                {
                    rd = riskDate.Subtract(product.LocalRolloffTime.Value.TimeOfDay);
                }
                else if (product.LocalRolloffTime.Value.Date < product.RolloffTimeToday.Value.Date)
                {
                    TimeSpan oneDay = TimeSpan.FromHours(24);

                    rd = riskDate.Add(oneDay - product.LocalRolloffTime.Value.TimeOfDay);
                }
                else if (product.LocalRolloffTime.Value.Date > product.RolloffTimeToday.Value.Date)
                {
                    TimeSpan oneDay = TimeSpan.FromHours(24);

                    rd = riskDate.Subtract(oneDay + product.LocalRolloffTime.Value.TimeOfDay);
                }

                rd = rd.AddDays(1);
                rd = AdjustRiskDateForWeekends(rd);
            }

            return rd;
        }

        public static DateTime AdjustRiskDateForWeekends(DateTime riskDate)
        {
            if (riskDate.DayOfWeek == DayOfWeek.Saturday)
            {
                riskDate = riskDate.AddDays(2).Date;
            }

            if (riskDate.DayOfWeek == DayOfWeek.Sunday)
            {
                riskDate = riskDate.AddDays(1).Date;
            }

            return riskDate;
        }


        public PnlCalculationDetail GetPnlCalculationDetail(List<PnlCalculationDetail> calculations, SourceDetail sourceInfo)
        {
            ProductDateType dateType = sourceInfo.DateType == ProductDateType.Day ? ProductDateType.MonthYear : sourceInfo.DateType;
            string timeSpreadStrip = null;

            if (sourceInfo.IsTimeSpread)
            {
                timeSpreadStrip = StripParser.Parse(sourceInfo.StripName, sourceInfo.ProductDate).ToPeriodOnlyString();
            }

            IEnumerable<PnlCalculationDetail> details = from c in calculations
                where
                    c.ProductId == sourceInfo.Product.ProductId &&
                    c.CalculationDate == sourceInfo.ProductDate &&
                    c.DateType == dateType &&
                    c.TimeSpreadStrip == timeSpreadStrip
                select c;

            if (details.Count() > 0)
            {
                return details.Single();
            }

            PnlCalculationDetail calculation = new PnlCalculationDetail
            {
                CalculationDate = sourceInfo.ProductDate,
                DateType = dateType,
                ProductId = sourceInfo.Product.ProductId,
                Product = sourceInfo.Product.Name,
                ProductCategory = sourceInfo.Product.Category == null ? sourceInfo.Product.Name : sourceInfo.Product.Category.Name,
                Amount = 0
            };
            calculations.Add(calculation);

            return calculation;
        }

        private CalculationDetail GetPositionCalculationDetail(
            List<CalculationDetail> calculations,
            SourceDetail sourceDetail,
            Product product,
            Product sourceProduct,
            int productYear,
            int productMonth,
            decimal amountAtMonth,
            DateTime productDate,
            ProductDateType dateType)
        {
            IEnumerable<CalculationDetail> details = from c in calculations
                where
                    c.ProductId == product.ProductId &&
                    c.SourceProductId == sourceProduct.ProductId &&
                    c.CalculationDate.Year == productYear && c.CalculationDate.Month == productMonth
                select c;

            if (details.Count() > 0)
            {
                CalculationDetail calculationDetail = details.Single();
                AddSourceDetailToCalculationDetail(calculationDetail, sourceDetail, amountAtMonth);

                return calculationDetail;
            }

            CalculationDetail calculation = new CalculationDetail
            {
                DetailId = Guid.NewGuid(),
                CalculationDate = new DateTime(productYear, productMonth, 1),
                Product = product.Name,
                Source = sourceProduct.Name,
                ProductCategoryId = product.Category == null ? product.ProductId : product.Category.CategoryId,
                ProductCategory = product.Category == null ? product.Name : product.Category.Name,
                ProductCategoryAbbreviation = product.Category == null
                    ? product.Name
                    : string.IsNullOrEmpty(product.Category.Abbreviation)
                            ? product.Category.Name
                            : product.Category.Abbreviation,
                Amount = 0,
                ProductId = product.ProductId,
                SourceProductId = sourceProduct.ProductId,
                ProductDate = productDate,
                ProductDateType = dateType,
                MappingColumn = (product.UnderlyingFutures == null)
                    ? (product.OfficialProduct == null ?
                        null : product.OfficialProduct.MappingColumn) : product.UnderlyingFutures.MappingColumn,
                OfficialProductId = product.OfficialProduct == null ? 0 : product.OfficialProduct.OfficialProductId,
                PnlFactor = product.PnlFactor,
                PositionFactor = product.PositionFactor
            };

            AddSourceDetailToCalculationDetail(calculation, sourceDetail, amountAtMonth);

            calculations.Add(calculation);
            return calculation;
        }

        private void AddSourceDetailToCalculationDetail(
            CalculationDetail calculation,
            SourceDetail sourceDetail,
            decimal amountAtMonth)
        {
            if (sourceDetail.SourceDetailId > 0)
            {
                if (calculation.SourceDetails == null)
                {
                    calculation.SourceDetails = new List<SourceDetail>();
                }

                if (calculation.SourceDetailAmountsDict == null)
                {
                    calculation.SourceDetailAmountsDict = new ConcurrentDictionary<int, decimal>();
                }

                if (calculation.SourceDetailAmountsDict.ContainsKey(sourceDetail.SourceDetailId))
                {
                    int key = sourceDetail.SourceDetailId;

                    if (calculation.SourceDetailAmountsDict.TryGetValue(key, out decimal sdAmount))
                    {
                        calculation.SourceDetailAmountsDict.TryRemove(key, out decimal ignore);
                        calculation.SourceDetailAmountsDict.TryAdd(key, sdAmount + amountAtMonth);
                    }
                }
                else
                {
                    calculation.SourceDetails.Add(sourceDetail);
                    calculation.SourceDetailAmountsDict.TryAdd(sourceDetail.SourceDetailId, amountAtMonth);
                }
            }
        }

        public int GetBusinessDays(DateTime start, DateTime end)
        {
            DateTime dateCount = start;
            int workingDays = 0;

            while (dateCount.Date <= end.Date)
            {
                switch (dateCount.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                    case DayOfWeek.Sunday:
                        break;

                    default:
                        {
                            workingDays++;
                        }
                        break;
                }
                dateCount = dateCount.AddDays(1);
            }
            return workingDays;
        }

        public List<DateTime> GetBusinessDaysArray(List<DateTime> holidays)
        {
            DateTime today = SystemTime.Today();
            DateTime start = new DateTime(today.Year, today.Month, 1);
            DateTime end = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

            return GetBusinessDaysArray(start, end, holidays);
        }

        public List<DateTime> GetBusinessDaysArray(DateTime start, DateTime end, List<DateTime> holidays)
        {
            List<DateTime> workingDays = new List<DateTime>();
            DateTime dateTime = start;

            while (dateTime.Date <= end.Date)
            {
                switch (dateTime.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                    case DayOfWeek.Sunday:
                        break;

                    default:
                        {
                            if (!holidays.Contains(dateTime))
                                workingDays.Add(dateTime);
                        }
                        break;
                }

                dateTime = dateTime.AddDays(1);
            }

            return workingDays;
        }

        private void ComputeHolidays()
        {
            _holidays = new HashSet<DateTime>();

            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                List<DateTime> holidaysList = context.CalendarHolidays
                    .Select(x => x.HolidayDate)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                foreach (DateTime holiday in holidaysList)
                {
                    if (!_holidays.Contains(holiday))
                    {
                        _holidays.Add(holiday);
                    }
                }
            }
        }

        public bool IsBusinessDay(DateTime day)
        {
            if (_holidays == null)
            {
                ComputeHolidays();
            }

            if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday || _holidays.Contains(day))
            {
                return false;
            }
            return true;
        }

        private ConcurrentDictionary<DateTime, DataTable> _priceDataCache =
            new ConcurrentDictionary<DateTime, DataTable>();

        public DataTable GetPriceDataFromCache(DateTime nearestSnapshot)
        {
            DataTable priceData;

            if (!_priceDataCache.TryGetValue(nearestSnapshot, out priceData))
            {
                priceData = GetPriceData(nearestSnapshot);
                _priceDataCache.TryAdd(nearestSnapshot, priceData);
            }

            return priceData;
        }

        public decimal? CalculateLivePriceWithSettlement(
            IPriceGetter priceGetter,
            Dictionary<string, decimal> settlementPrices,
            decimal? livePrice,
            SourceDetail detail,
            string[] stripParts,
            Tuple<DateTime, ProductDateType> liveTradeDate,
            DateTime? startDate,
            DateTime? endDate,
            string mappingColumn,
            int officialProductId,
            DateTime? transactTime,
            TradeCapture trade,
            decimal pnlConversionFactor)
        {
            decimal? livePriceValue = livePrice;

            int tradeDaysElapsed1 = 0, tradeDaysElapsed2 = 0;
            int tradePricingStartDay1 = 0, tradePricingStartDay2 = 0;

            List<DateTime> businessDays1 = null;
            List<DateTime> businessDays2 = null;

            string mappingColumn1 = null;
            string mappingColumn2 = null;
            int officialProductId1 = 0;
            int officialProductId2 = 0;
            int productId1 = 0;
            int productId2 = 0;
            decimal positionFactor1 = 1M;
            decimal positionFactor2 = 1M;
            decimal legFactor1 = 1M;
            decimal legFactor2 = 1M;

            if (detail != null)
            {
                lock (detail)
                {
                    businessDays1 = detail.BusinessDays1 == null ? null : detail.BusinessDays1.ToList();
                    tradeDaysElapsed1 = detail.BusinessDaysElapsed1;
                    tradePricingStartDay1 = Math.Max(0, detail.PricingStartDay1 - 1);

                    businessDays2 = detail.BusinessDays2 == null ? null : detail.BusinessDays2.ToList();
                    tradeDaysElapsed2 = detail.BusinessDaysElapsed2;
                    tradePricingStartDay2 = Math.Max(0, detail.PricingStartDay2 - 1);

                    mappingColumn1 = detail.MappingColumn1;
                    mappingColumn2 = detail.MappingColumn2;

                    officialProductId1 = detail.OfficialProductId1;
                    officialProductId2 = detail.OfficialProductId2;

                    productId1 = detail.ProductId1;
                    productId2 = detail.ProductId2;

                    positionFactor1 = detail.PositionFactor1;
                    positionFactor2 = detail.PositionFactor2;

                    legFactor1 = detail.LegFactor1;
                    legFactor2 = detail.LegFactor2;
                }
            }
            else if (trade != null)
            {
                lock (trade)
                {
                    businessDays1 = trade.BusinessDays1 == null ? null : trade.BusinessDays1.ToList();
                    tradeDaysElapsed1 = trade.BusinessDaysElapsed1;
                    tradePricingStartDay1 = Math.Max(0, trade.PricingStartDay1 - 1);

                    businessDays2 = trade.BusinessDays2 == null ? null : trade.BusinessDays2.ToList();
                    tradeDaysElapsed2 = trade.BusinessDaysElapsed2;
                    tradePricingStartDay2 = Math.Max(0, trade.PricingStartDay2 - 1);

                    mappingColumn1 = trade.MappingColumn1;
                    mappingColumn2 = trade.MappingColumn2;

                    officialProductId1 = trade.OfficialProductId1;
                    officialProductId2 = trade.OfficialProductId2;

                    productId1 = trade.ProductId1;
                    productId2 = trade.ProductId2;

                    positionFactor1 = trade.PositionFactor1;
                    positionFactor2 = trade.PositionFactor2;

                    legFactor1 = trade.LegFactor1;
                    legFactor2 = trade.LegFactor2;
                }
            }

            if (trade != null
                && (trade.SecurityDefinition.Product.IsProductDaily))
            {
                businessDays2 = null;
            }

            if (businessDays1 != null && settlementPrices != null && tradeDaysElapsed1 > 0)
            {
                decimal leg2price = 0M;
                Tuple<DateTime, ProductDateType> part2parsed = null;

                // if it's a time spread we need to get leg2 live price
                if (stripParts != null && stripParts.Length == 2)
                {
                    try
                    {
                        if (trade != null)
                        {
                            part2parsed = new Tuple<DateTime, ProductDateType>(
                                trade.Strip.Part2.StartDate,
                                trade.Strip.Part2.DateType);
                        }
                        else
                        {
                            part2parsed = StripHelper.ParseStripDate(
                                stripParts[1],
                                startDate ?? DateTime.MinValue,
                                transactTime);
                        }

                        leg2price = priceGetter.GetProductPrice(
                            productId2,
                            part2parsed.Item1,
                            part2parsed.Item2,
                            mappingColumn,
                            startDate,
                            endDate) ?? 0M;
                    }
                    catch
                    {
                    }
                }

                if (businessDays2 != null && businessDays2.Count > 0)
                {
                    int totalBusinessDays1 = businessDays1.Count - tradePricingStartDay1;
                    int businessDaysElapsed1 = Math.Max(0, tradeDaysElapsed1 - tradePricingStartDay1);

                    decimal? leg1St1livePrice =
                        priceGetter.GetProductPrice(productId1, liveTradeDate.Item1, liveTradeDate.Item2,
                                                    mappingColumn1, startDate, endDate) ?? 0M;

                    decimal sumLeg1St1Settlement = GetSumSettlementPrices(
                        settlementPrices,
                        leg1St1livePrice.Value,
                        tradePricingStartDay1,
                        tradeDaysElapsed1,
                        businessDays1,
                        0M,
                        officialProductId1);

                    decimal live1 = sumLeg1St1Settlement / totalBusinessDays1 +
                                    leg1St1livePrice.Value * (totalBusinessDays1 - businessDaysElapsed1)
                                    / totalBusinessDays1;

                    live1 = live1 * legFactor1 / positionFactor1;

                    int totalBusinessDays2 = businessDays2.Count - tradePricingStartDay2;
                    int businessDaysElapsed2 = Math.Max(0, tradeDaysElapsed2 - tradePricingStartDay2);

                    decimal? leg2St1livePrice =
                        priceGetter.GetProductPrice(productId2, liveTradeDate.Item1, liveTradeDate.Item2,
                                                    mappingColumn2, startDate, endDate) ?? 0M;

                    decimal sumLeg2St1Settlement =
                        GetSumSettlementPrices(settlementPrices, leg2St1livePrice.Value, tradePricingStartDay2,
                                               tradeDaysElapsed2, businessDays2, 0M, officialProductId2);

                    decimal live2 = sumLeg2St1Settlement / totalBusinessDays2 +
                                    leg2St1livePrice.Value * (totalBusinessDays2 - businessDaysElapsed2)
                                    / totalBusinessDays2;

                    live2 = live2 * legFactor2 / positionFactor2;

                    livePriceValue = (live1 - live2);

                    if (part2parsed != null)
                    {
                        decimal? leg1St2livePrice = priceGetter.GetProductPrice(
                            productId1,
                            part2parsed.Item1,
                            part2parsed.Item2,
                            mappingColumn1, startDate, endDate) ?? 0M;

                        decimal? leg2St2livePrice = priceGetter.GetProductPrice(
                            productId2,
                            part2parsed.Item1,
                            part2parsed.Item2,
                            mappingColumn2,
                            startDate,
                            endDate) ?? 0M;

                        decimal timespreadLeg2Price = leg1St2livePrice.Value * legFactor1 / positionFactor1 -
                                                      leg2St2livePrice.Value * legFactor2 / positionFactor2;

                        livePriceValue -= timespreadLeg2Price;
                    }

                    if (pnlConversionFactor > 0M)
                    {
                        livePriceValue = livePriceValue / pnlConversionFactor;
                    }
                }
                else
                {
                    int totalBusinessDays = businessDays1.Count - tradePricingStartDay1;
                    int businessDaysElapsed = Math.Max(0, tradeDaysElapsed1 - tradePricingStartDay1);

                    decimal sumSettlementPrices =
                        GetSumSettlementPrices(settlementPrices, livePrice.Value, tradePricingStartDay1,
                                               tradeDaysElapsed1, businessDays1, leg2price, officialProductId);

                    livePriceValue = sumSettlementPrices / totalBusinessDays +
                                     livePrice.Value * (totalBusinessDays - businessDaysElapsed) / totalBusinessDays;
                }
            }

            return livePriceValue;
        }

        private decimal GetSumSettlementPrices(
            Dictionary<string, decimal> settlementPrices,
            decimal livePrice,
            int pricingStartDay,
            int businessDaysElapsed,
            List<DateTime> businessDays,
            decimal leg2price,
            int officialProductId)
        {
            decimal sumSettlementPrices = 0M;

            for (int i = pricingStartDay; i < businessDaysElapsed; i++)
            {
                string key = officialProductId + businessDays[i].ToShortDateString();

                if (settlementPrices.TryGetValue(key, out decimal settlementPrice))
                {
                    // in case it's a time spread we would calculate leg2price, otherwise it's zero
                    settlementPrice -= leg2price;
                }
                else
                {
                    // if we haven't got settlement price for a date we don't subtract leg2price
                    // (cause we use trade live price)
                    settlementPrice = livePrice;
                }

                sumSettlementPrices += settlementPrice;
            }

            return sumSettlementPrices;
        }

        public TimeSpan? GetSnapshotEod(DateTime date)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                DateTime dateTime = date.Date;

                return cxt.PnlReportEods.OrderBy(x => x.EndOfDayDb).Where(x => x.Day == dateTime).ToList()
                          .Select(x => x.EndOfDay).FirstOrDefault();
            }
        }

        public void SetSnapshotTime(DateTime snapshotTime)
        {
            DateTime day = snapshotTime.Date;

            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                PnlReportEod val = cxt.PnlReportEods.FirstOrDefault(x => x.Day == day);

                if (val == null)
                {
                    val = new PnlReportEod();
                    val.Day = day;

                    cxt.PnlReportEods.Add(val);
                }

                val.EndOfDayDb = snapshotTime;

                cxt.SaveChanges();
            }
        }

        public void ResetPriceDataCache()
        {
            _priceDataCache.Clear();
        }

        public List<CalculationDetail> CalculatePositions(
            DateTime sourceDataDate,
            DateTime riskDate,
            SourceDataType dataType,
            string accountNumber,
            out List<CalculationError> calculationErrors,
            string exchangeValue = null,
            bool useCache = true,
            CalculationCache globalCache = null)
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                string commandTimeoutString = ConfigurationManager.AppSettings["SqlServer.CommandTimeoutInSeconds"];

                if (!int.TryParse(commandTimeoutString, out int commandTimeout))
                {
                    commandTimeout = 30 * 60; // 30 min
                }

                context.Database.CommandTimeout = commandTimeout;
                if (dataType == SourceDataType.Seals)
                {
                    //If seals data required. Get sourcedata oject with linked seal details
                    DbQuery<SourceData> sourceDataQuery =
                        (DbQuery<SourceData>)context.SourceDataSet
                            .Include("SealDetails")
                            .Where(s => s.Date == sourceDataDate && s.TypeDb == (int)dataType);

                    //prepare product list for further mapping
                    List<Product> products = context.Products
                        .Include("OfficialProduct").Include("BalmoOnCrudeProduct")
                        .Include("BalmoOnComplexProduct").Include("Category")
                        .Include("ExpiryCalendar").Include("UnderlyingFutures").ToList();

                    SourceData sourceData = sourceDataQuery.FirstOrDefault();
                    //if there are no data of Seals detail imported given day
                    if (sourceData == null)
                    {
                        calculationErrors = null;
                        return null;
                    }

                    CalculationCache cache = null;

                    if (useCache)
                    {
                        if (globalCache != null)
                        {
                            cache = globalCache;
                        }
                        else
                        {
                            cache = new CalculationCache();
                            cache.Initialize(context);
                        }
                    }
                    List<SourceDetail> sourceDetails = new List<SourceDetail>();
                    List<CalculationError> convertErrors = new List<CalculationError>();
                    //T\Convert seal details to source details
                    sourceData.SealDetails.ToList().ForEach(sd =>
                    {
                        try
                        {
                            //try to map product
                            Product product;
                            //Special for balmo
                            if (sd.IsICE && sd.DateType.GetValueOrDefault() == (int)ProductDateType.Day)
                            {
                                int datePart = 0;
                                product = ProductManager.GetProductByBalmoSealCode(
                                    sd.Market_Product,
                                    products,
                                    out datePart,
                                    false);
                                // found by balmo code try to set up balmo date
                                if (product != null)
                                {
                                    if (datePart > 0)
                                    {
                                        try
                                        {
                                            //try to set actual balmo date for given details
                                            sd.MarketExpiry = new DateTime(
                                                sd.MarketExpiry.Year,
                                                sd.MarketExpiry.Month,
                                                datePart);
                                        }
                                        catch
                                        {
                                            //if resulting date index does not fit date(i.e. 31 for Febrary)
                                            product = null;
                                        }

                                    }
                                }
                                //if not found by balmo code try to retrieve by Exchange code
                                if (product == null)
                                {
                                    product = products.FirstOrDefault(p => p.ExchangeContractCode == sd.Market_Product);

                                    if (product != null)
                                    {
                                        //if found mark as not balmo
                                        sd.DateType = (short)ProductDateType.MonthYear;
                                    }
                                }
                            }
                            else
                            {
                                product = products.FirstOrDefault(p => p.ExchangeContractCode == sd.Market_Product);
                            }
                            if (product != null)
                            {
                                sd.Product = product;
                                sourceDetails.Add(sd.ToSourceDetail());
                            }
                            else
                            {
                                //if errors in convert to sourcedetails or product mapping collect these errors
                                convertErrors.Add(
                                new CalculationError()
                                {
                                    ErrorMessage =
                                        string.Format("The alias {0} does not map to a product", sd.Market_Product),
                                    SourceDetail = sd.SafeToSourceDetail()
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            //if errors in convert to sourcedetails or product mapping collect these errors
                            convertErrors.Add(
                                new CalculationError()
                                {
                                    ErrorMessage = ex.Message,
                                    SourceDetail = sd.SafeToSourceDetail()
                                });
                        }
                    });

                    //Call calculation
                    List<CalculationDetail> result = CalculatePositions(
                        cache,
                        context,
                        sourceDetails,
                        riskDate,
                        dataType,
                        out calculationErrors);

                    //Add extra errors if occurs on calculation routine 
                    calculationErrors.AddRange(convertErrors);

                    return result;
                }
                else
                {
                    DbQuery<SourceData> sourceDataQuery = (DbQuery<SourceData>)
                        context.SourceDataSet.Include("SourceDetails").Include("SourceDetails.Product")
                        .Include("SourceDetails.Product.OfficialProduct").Include("SourceDetails.Product.BalmoOnCrudeProduct")
                        .Include("SourceDetails.Product.BalmoOnComplexProduct").Include("SourceDetails.Product.Category")
                        .Include("SourceDetails.Product.ExpiryCalendar").Include("SourceDetails.Product.UnderlyingFutures")
                        .Where(s => s.Date == sourceDataDate && s.TypeDb == (int)dataType);

                    SourceData sourceData = sourceDataQuery.FirstOrDefault();

                    if (sourceData == null)
                    {
                        calculationErrors = null;
                        return null;
                    }

                    CalculationCache cache = null;

                    if (useCache)
                    {
                        if (globalCache != null)
                        {
                            cache = globalCache;
                        }
                        else
                        {
                            cache = new CalculationCache();
                            cache.Initialize(context);
                        }
                    }

                    List<SourceDetail> sourceDetails =
                        sourceData.SourceDetails.Where(
                            s =>
                            (accountNumber == null || s.AccountNumber == accountNumber) &&
                            (exchangeValue == null || s.ExchangeCode == exchangeValue)).ToList();

                    return CalculatePositions(cache, context, sourceDetails, riskDate, dataType, out calculationErrors);
                }
            }
        }

        /// <summary>
        /// Calculate positions for the specified parameters.
        /// </summary>
        /// <param name="cache">Calculation cache (products, expiry dates, holidays) to use,
        /// if null a new one will be created.</param>
        /// <param name="context">Database context to use (in case the calculation is a part of a
        /// larger operation that already exploit context).</param>
        /// <param name="sourceDetails">SourceDetails for which positions will be calculated.</param>
        /// <param name="riskDate">Risk date for the calculation.</param>
        /// <param name="dataType">Data type (open positions, trade activity or seals).</param>
        /// <param name="calculationErrors">An output list of calculation errors.</param>
        /// <returns>Calculated positions as a CalculationDetail list.</returns>
        public List<CalculationDetail> CalculatePositions(CalculationCache cache,
            [NotNull] MandaraEntities context,
            List<SourceDetail> sourceDetails,
            DateTime riskDate,
            SourceDataType dataType,
            out List<CalculationError> calculationErrors,
            bool reportErrors = true)
        {
            calculationErrors = new List<CalculationError>();

            ClassicPositionsCalculator positionsCalculator = new ClassicPositionsCalculator(context, cache);

            return CalculatePositions(
                sourceDetails,
                riskDate,
                positionsCalculator,
                out calculationErrors,
                reportErrors);
        }

        /// <summary>
        /// Calculate positions for the specified parameters.
        /// </summary>
        /// <param name="sourceDetails">SourceDetails for which positions will be calculated.</param>
        /// <param name="riskDate">Risk date for the calculation.</param>
        /// <param name="positionsCalculator">Initialized position calculator used for the calculation.</param>
        /// <param name="calculationErrors">An output list of calculation errors.</param>
        /// <returns>Calculated positions as a CalculationDetail list.</returns>
        public List<CalculationDetail> CalculatePositions(
            List<SourceDetail> sourceDetails,
            DateTime riskDate,
            ClassicPositionsCalculator positionsCalculator,
            out List<CalculationError> calculationErrors,
            bool reportErrors = true)
        {
            //TODO: always empty
            calculationErrors = new List<CalculationError>();

            return positionsCalculator.Calculate(sourceDetails, riskDate, reportErrors);
        }

        /// <summary>
        /// Calculate historical pnl report.
        /// </summary>
        /// <param name="sourceDataDate">Date of the source data.</param>
        /// <param name="riskDate">Risk date for which pnl report should be calculated.</param>
        /// <param name="priceDate">Price snapshot datetime.</param>
        /// <param name="dataType">Data type (open positions, trade activity or seals).</param>
        /// <param name="accountNumber">Account number for the calculation.
        /// Specify NULL value in order to use all source details for the calculation.</param>
        /// <param name="exchangeValue">Exchange code for the calculation.
        /// Specify NULL value in order to use all source details for the calculation.</param>
        /// <param name="externalPrices">Obsolete. Do not use that parameter.</param>
        /// <returns>Calculated historical pnl as a PnlCalculationDetail list.</returns>
        public List<PnlCalculationDetail> CalculateHistoricalPnl(
            DateTime sourceDataDate,
            DateTime riskDate,
            DateTime priceDate,
            SourceDataType dataType,
            string accountNumber,
            string exchangeValue = null,
            List<ProductPriceDetail> externalPrices = null)
        {

            /*
             * 
             * Calculates historical pnl for the specified parameters.
             * 
             * Method acts as follows:
             *     - we read from the price database data for the date specified in priceDate parameter.
             *       That's a products price from a snapshot.
             *     - we calculate historical positions for the specified parameters.
             *     - for each position we calculate corresponding PnlCalculationDetail:
             *         * we loop through position source details
             *         * for each source detail we calculate pnl using product snapshot price received above.
             * 
             */

            bool useExternalPrices = externalPrices != null && externalPrices.Count > 0;

            int priceTimestamp = EpochConverter.ToEpochTime(priceDate);

            DataTable priceData = null;
            if (!useExternalPrices)
                priceData = GetPriceData(priceTimestamp);

            List<CalculationError> positionCalculationErrors;
            List<CalculationDetail> positions = CalculatePositions(
                sourceDataDate,
                riskDate,
                dataType,
                accountNumber,
                out positionCalculationErrors,
                exchangeValue,
                true);

            if (positions == null)
            {
                return null;
            }

            List<PnlCalculationDetail> result = new List<PnlCalculationDetail>();

            SnapshotPriceGetter snapshotPriceGetter = new SnapshotPriceGetter(priceData, priceDate);
            Dictionary<string, decimal> settlementPrices = GetSettlementPrices();

            List<SourceDetail> sourceDetails =
                positions.Where(p => p != null && p.SourceDetails != null)
                         .SelectMany(p => p.SourceDetails)
                         .Where(sd => sd != null)
                         .ToList();

            List<int> usedIDs = new List<int>();

            foreach (SourceDetail detail in sourceDetails)
            {
                if (usedIDs.Contains(detail.SourceDetailId))
                {
                    continue;
                }

                usedIDs.Add(detail.SourceDetailId);

                decimal? livePrice = snapshotPriceGetter.GetProductPrice(
                    detail.Product.ProductId,
                    detail.ProductDate,
                    detail.DateType,
                    detail.Product.OfficialProduct.MappingColumn) ?? 0M;

                PnlCalculationDetail calculation = GetPnlCalculationDetail(result, detail);
                decimal pnlConversionFactor = detail.Product.PnlFactor.HasValue ? detail.Product.PnlFactor.Value : 1M;

                livePrice = CalculateLivePriceWithSettlement(
                    snapshotPriceGetter,
                    settlementPrices,
                    livePrice,
                    detail,
                    null,
                    new Tuple<DateTime, ProductDateType>(detail.ProductDate, detail.DateType),
                    detail.ProductDate,
                    detail.TradeEndDate,
                    detail.Product.OfficialProduct.MappingColumn,
                    detail.Product.OfficialProduct.OfficialProductId,
                    detail.TransactTime,
                    null,
                    pnlConversionFactor);

                decimal baseAmount =
                    detail.Quantity.Value
                    * (livePrice.Value - detail.TradePrice)
                    * pnlConversionFactor
                    * detail.Product.ContractSize;
                decimal amount =
                    ContractSizeCalculator.ApplyContractSizeMultiplier(
                        baseAmount,
                        detail.Product.ContractSizeMultiplier,
                        detail.ProductDate);

                calculation.Amount += amount;
            }

            return result;
        }
    }
}
