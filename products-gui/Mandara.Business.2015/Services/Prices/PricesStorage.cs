using Mandara.Business.Contracts;
using Mandara.Business.DataInterface;
using Mandara.Database.Sql;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Extensions.Option;
using Mandara.IRM.Server.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Services.Prices
{
    public abstract class PricesStorage
    {
        private static readonly ILogger Logger = new NLogLoggerFactory().GetCurrentClassLogger();
        public List<TimeSpan> TodayTimestamps { get; protected set; }
        protected readonly IDailyPricesTable _dailyPricesTable;
        protected readonly IPricesTimestampsRepository _pricesTimestampsRepository;
        protected readonly IProductsStorage _productsStorage;
        protected readonly SqlConnectionStringBuilder pricesDbConnectionString;
        private static readonly Dictionary<string, DateTime> KnownZeroPrices = new Dictionary<string, DateTime>();
        private static readonly TimeSpan IgnoreZeroPriceFor = TimeSpan.FromMinutes(10);

        protected static readonly ConcurrentDictionary<string, DateTime> _errorsByMappingColumn =
            new ConcurrentDictionary<string, DateTime>();

        public PricesStorage(
            IDailyPricesTable dailyPricesTable,
            IPricesTimestampsRepository pricesTimestampsRepository,
            IProductsStorage productsStorage,
            List<TimeSpan> todayTimestamps)
        {
            _dailyPricesTable = dailyPricesTable;
            _pricesTimestampsRepository = pricesTimestampsRepository;
            _productsStorage = productsStorage;
            TodayTimestamps = todayTimestamps;
            pricesDbConnectionString = ConnectionString.GetConnectionStringBuild("PriceDatabase");
        }

        public TryGetResult<Money>[] GetProductPricesByMonth(
            int productId,
            DateTime productDate,
            ProductDateType priceDateType,
            string mappingColumn,
            int officialProductId,
            IPricesTable livePricesTable,
            DateTime riskDate,
            DateRange tradeStartEndRange)
        {
            if (livePricesTable == null || string.IsNullOrEmpty(mappingColumn))
            {
                return new TryGetResult<Money>[0];
            }

            if (priceDateType != ProductDateType.Quarter && priceDateType != ProductDateType.Year
                && priceDateType != ProductDateType.Custom)
            {
                return new TryGetResult<Money>[0];
            }

            DateRange riskProductRange = new DateRange(riskDate, productDate);
            int productMonth = riskProductRange.TotalMonths + 72;

            TryGetResult<decimal>[] prices = new TryGetResult<decimal>[0];

            try
            {
                switch (priceDateType)
                {
                    case ProductDateType.Quarter:
                    prices = livePricesTable.GetPricesByMonth(
                        productMonth,
                        productMonth + 2,
                        mappingColumn,
                        productId,
                        productDate,
                        _productsStorage);
                    break;

                    case ProductDateType.Year:
                    prices = livePricesTable.GetPricesByMonth(
                        productMonth,
                        productMonth + 11,
                        mappingColumn,
                        productId,
                        productDate,
                        _productsStorage);
                    break;

                    case ProductDateType.Custom:
                    if (tradeStartEndRange.HasStartDate() && tradeStartEndRange.HasEndDate())
                    {
                        int numAddonMonths = tradeStartEndRange.TotalMonths;

                        DateRange riskTradeStartRange = new DateRange(riskDate, tradeStartEndRange.Start);
                        productMonth = riskTradeStartRange.TotalMonths + 72;

                        prices = livePricesTable.GetPricesByMonth(
                            productMonth,
                            productMonth + numAddonMonths,
                            mappingColumn,
                            productId,
                            productDate,
                            _productsStorage);
                    }
                    break;
                }
            }
            catch (Exception)
            {
                ReportMappingError(mappingColumn);
            }

            if (prices.Length > 0)
            {
                string currencyName = _productsStorage.TryGetOfficialProduct(officialProductId).Value.Currency.IsoName;
                return prices.Select(
                        p => p.HasValue
                            ? new TryGetVal<Money>(new Money(p.Value, currencyName))
                            : new TryGetVal<Money>()).ToArray();
            }
            else
            {
                return new TryGetResult<Money>[0];
            }
        }

        protected void ReportMappingError(string mappingColumn)
        {
            bool reportError = false;
            DateTime errorTime;

            if (_errorsByMappingColumn.TryGetValue(mappingColumn, out errorTime))
            {
                if (SystemTime.Now() - errorTime > TimeSpan.FromMinutes(1))
                {
                    reportError = true;

                    DateTime _;
                    _errorsByMappingColumn.TryRemove(mappingColumn, out _);
                    _errorsByMappingColumn.TryAdd(mappingColumn, SystemTime.Now());
                }
            }
            else
            {
                reportError = true;
                _errorsByMappingColumn.TryAdd(mappingColumn, SystemTime.Now());
            }

            if (reportError)
            {
                //TODO: error reporter
            }
        }

        protected TryGetResult<Money> GetProductPrice(
            int productId,
            int officialProductId,
            DateTime productDate,
            ProductDateType priceDateType,
            DateRange tradeStartEndRange,
            string mappingColumn,
            IPricesTable pricesTable,
            DateTime riskDate)
        {
            if (pricesTable == null || string.IsNullOrEmpty(mappingColumn))
            {
                return new TryGetVal<Money>();
            }

            decimal? price = 0;
            DateRange riskProductRange = new DateRange(riskDate, productDate);
            int productMonth = riskProductRange.TotalMonths + 72;
            try
            {
                switch (priceDateType)
                {
                    case ProductDateType.Daily:
                    {
                        price = GetDailyPrice(
                            productId,
                            productDate,
                            tradeStartEndRange,
                            mappingColumn,
                            riskProductRange);
                    }
                    break;

                    case ProductDateType.Day:
                    case ProductDateType.MonthYear:
                    {
                        price = GetMonthlyPrice(mappingColumn, pricesTable, productMonth);
                    }
                    break;

                    case ProductDateType.Quarter:
                    {
                        price = pricesTable.GetAveragePrice(
                            productMonth,
                            productMonth + 2,
                            mappingColumn,
                            productId,
                            productDate,
                            _productsStorage);
                    }
                    break;

                    case ProductDateType.Year:
                    {
                        price = pricesTable.GetAveragePrice(
                            productMonth,
                            productMonth + 11,
                            mappingColumn,
                            productId,
                            productDate,
                            _productsStorage);
                    }
                    break;

                    case ProductDateType.Custom:
                    {
                        price = GetCustomPeriodPrice(
                            productId,
                            productDate,
                            tradeStartEndRange,
                            mappingColumn,
                            pricesTable,
                            riskDate);
                    }
                    break;
                }
            }
            catch (Exception)
            {
                ReportMappingError(mappingColumn);
            }

            return price == null
                ? new TryGetVal<Money>()
                : new TryGetVal<Money>(new Money(
                    price.Value,
                    _productsStorage.TryGetOfficialProduct(officialProductId).Value.Currency.IsoName));
        }

        private decimal? GetDailyPrice(
            int productId,
            DateTime productDate,
            DateRange tradeStartEndRange,
            string mappingColumn,
            DateRange riskProductRange)
        {
            return (tradeStartEndRange.HasStartDate() && tradeStartEndRange.HasEndDate())
                ? CalculateAverageDailyPrice()
                : 0;

            decimal? CalculateAverageDailyPrice()
            {
                int diffDays = tradeStartEndRange.TotalDays;
                int startDay = riskProductRange.TotalDays + 72;
                int endDay = startDay + diffDays;

                return _dailyPricesTable.GetAveragePrice(startDay, endDay, mappingColumn, productId, productDate, null);
            }
        }

        private static decimal? GetMonthlyPrice(string mappingColumn, IPricesTable pricesTable, int productMonth)
        {
            decimal? price = pricesTable.GetPrice(productMonth, mappingColumn);

            if (price == 0M 
                && (!KnownZeroPrices.TryGetValue(mappingColumn, out DateTime lastZeroLogged) 
                    || SystemTime.Now().Subtract(lastZeroLogged) > IgnoreZeroPriceFor))
            {
                KnownZeroPrices[mappingColumn] = SystemTime.Now();
                Logger.Debug(
                    "PriceStorage - price for mapping col {0}, month {1} is 0",
                    mappingColumn,
                    productMonth);
            }

            return price;
        }

        private decimal? GetCustomPeriodPrice(
            int productId,
            DateTime productDate,
            DateRange tradeStartEndRange,
            string mappingColumn,
            IPricesTable pricesTable,
            DateTime riskDate)
        {
            return (tradeStartEndRange.HasStartDate() && tradeStartEndRange.HasEndDate())
                ? CalculateAvergeCustomPrice()
                : 0;

            decimal? CalculateAvergeCustomPrice()
            {
                int numAddOnMonths = tradeStartEndRange.TotalMonths;
                DateRange riskTradeStartRange = new DateRange(riskDate, tradeStartEndRange.Start);
                int productMonth = riskTradeStartRange.TotalMonths + 72;

                return pricesTable.GetAveragePrice(
                    productMonth,
                    productMonth + numAddOnMonths,
                    mappingColumn,
                    productId,
                    productDate,
                    _productsStorage);
            }
        }
    }
}
