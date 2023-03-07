using Mandara.Business.Contracts;
using Mandara.Database.Query;
using Mandara.Database.Sql;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using Mandara.Extensions.Collections;

namespace Mandara.IRM.Server.Services
{
    public class SettlementPricesStorage : ISettlementPricesStorage
    {
        private readonly IProductsStorage _productsStorage;
        private ConcurrentDictionary<string, Money> _settlementPrices = new ConcurrentDictionary<string, Money>();
        private readonly MemoryCache _nonCurrentSettlements = new MemoryCache("NonCurrentSettles");
        private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(15);
        private readonly SqlConnectionStringBuilder _priceDbConnectionString;
        private readonly ILogger _log;

        private static readonly string SettlementLoadTemplate =
            $@"SELECT official_product_id, settlement_date, price, start_date 
                    FROM dbo.settlement_prices 
                    where settlement_date >= '{{{LoadDateField(0)}}}' 
                        and settlement_date <= '{{{LoadDateField(1)}}}'";

        private static string LoadDateField(int formatArgNum)
        {
            return $"{formatArgNum}:{Formats.DashSeparatedShortDate} 00:00:00";
        }

        public SettlementPricesStorage(IProductsStorage productsStorage, ILogger log)
        {
            _productsStorage = productsStorage;
            _priceDbConnectionString = ConnectionString.GetConnectionStringBuild("PriceDatabase");
            _log = log;
        }

        public bool TryGetPrice(
            int officialProductId,
            DateTime settlementDate,
            DateTime stripMonth,
            TradeCapture trade,
            out Money price)
        {
            return TryGetPrice(
                officialProductId,
                settlementDate,
                GetStartDateFromStrip(stripMonth, trade),
                out price);
        }

        private static DateTime GetStartDateFromStrip(DateTime stripMonth, TradeCapture trade)
        {
            DateTime startOfStripMonth = stripMonth.FirstDayOfMonth();
            PrecalcDetail precalcDetailForStripMonth =
                trade.PrecalcPositions.FirstOrDefault(precalc => precalc.Month == startOfStripMonth);

            return (AvailablePrecalcDetails(precalcDetailForStripMonth)
                ? precalcDetailForStripMonth.DaysPositions.Keys.Min()
                : startOfStripMonth).FirstDayOfMonth();
        }

        private static bool AvailablePrecalcDetails(PrecalcDetail precalcDetail)
        {
            return precalcDetail?.DaysPositions != null && precalcDetail.DaysPositions.Any();
        }

        public bool TryGetPrice(int officialProductId, DateTime settlementDate, DateTime startDate, out Money price)
        {
            TryGetResult<Money> settle;

            if (settlementDate >= SystemTime.Now().FirstDayOfMonth())
            {
                settle = TryGetPrice(
                    officialProductId,
                    settlementDate,
                    startDate,
                    _settlementPrices);

                price = settle.HasValue ? settle.Value : default(Money);
                return settle.HasValue;
            }

            string settlesKey = GetCacheKey(settlementDate);
            ConcurrentDictionary<string, Money> cachedSettles =
                (ConcurrentDictionary<string, Money>)_nonCurrentSettlements.Get(settlesKey);

            if (null == cachedSettles)
            {
                cachedSettles =
                    new ConcurrentDictionary<string, Money>(Update(GetSettlementDateRange(settlementDate)));

                if (cachedSettles.Any())
                {
                    _nonCurrentSettlements.Add(
                        settlesKey,
                        cachedSettles,
                        new CacheItemPolicy() { AbsoluteExpiration = SystemTime.Now().Add(CacheLifetime) });
                }
            }

            settle = TryGetPrice(officialProductId, settlementDate, startDate, cachedSettles);
            price = settle.HasValue ? settle.Value : default(Money);
            return settle.HasValue;
        }

        private static string GetCacheKey(DateTime settlementDate)
        {
            return settlementDate.FirstDayOfMonth().ToShortDateString();
        }

        private static TryGetResult<Money> TryGetPrice(
            int officialProductId,
            DateTime settleDate,
            DateTime startDate,
            ConcurrentDictionary<string, Money> settles)
        {
            string keyWithStartDate =
                officialProductId + settleDate.ToShortDateString() + startDate.ToShortDateString();
            Money price;
            bool found = settles.TryGetValue(keyWithStartDate, out price);

            if (!found)
            {
                string keyWithoutStartDate = officialProductId + settleDate.ToShortDateString();

                found = settles.TryGetValue(keyWithoutStartDate, out price);
            }

            return new TryGetVal<Money>(price, (val) => !found);
        }

        public void Update()
        {
            _settlementPrices =
                new ConcurrentDictionary<string, Money>(Update(GetSettlementDateRange(SystemTime.Now())));
        }

        private DateRange GetSettlementDateRange(DateTime baseDate)
        {
            DateTime firstDayOfMonth = baseDate.FirstDayOfMonth();

            return new DateRange(firstDayOfMonth, firstDayOfMonth.AddMonths(1));
        }

        private Dictionary<string, Money> Update(DateRange settleDates)
        {
            try
            {
                Dictionary<string, Money> settlementPrices = SqlServerCommandExecution.ExecuteReaderQuery(
                   _priceDbConnectionString,
                   (conn) => CreateUpdateSettlePricesQuery(settleDates, conn),
                   ReadSettlementPrices);

                UpdateStoredSettlePrices(settlementPrices);
                return settlementPrices;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error updating Settlement Prices");
                return new Dictionary<string, Money>();
            }
        }

        private static SqlCommand CreateUpdateSettlePricesQuery(DateRange settleDates, SqlConnection conn)
        {
            string sql = String.Format(SettlementLoadTemplate, settleDates.Start, settleDates.End);

            return new SqlCommand(sql, conn);
        }

        private Dictionary<string, Money> ReadSettlementPrices(SqlDataReader reader)
        {
            Dictionary<string, Money> settlePrices = new Dictionary<string, Money>();

            while (reader.Read())
            {
                try
                {
                    settlePrices = ConstructSettlementEntry(settlePrices, reader);
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Error loading settlement price, Official Product Id: {0}", reader.GetInt32(0));
                }
            }

            return settlePrices;
        }

        private Dictionary<string, Money> ConstructSettlementEntry(
            Dictionary<string, Money> settlementPrices,
            SqlDataReader dbReader)
        {
            int officialProductId = dbReader.GetInt32(0);
            string settlePriceKey = GetSettlementPriceKey(dbReader, officialProductId);
            decimal price = dbReader.GetDecimal(2);
            string currencyIsoName = _productsStorage.GetOfficialProductCurrency(officialProductId).IsoName;

            settlementPrices.Add(settlePriceKey, new Money(price, currencyIsoName));
            return settlementPrices;
        }

        private static string GetSettlementPriceKey(SqlDataReader reader, int officialProductId)
        {
            DateTime settlementDate = reader.GetDateTime(1);
            string startDateString = GetStartDate(reader);

            return $"{officialProductId}{settlementDate.ToShortDateString()}{startDateString}";
        }

        private static string GetStartDate(SqlDataReader reader)
        {
            return reader.IsDBNull(3) ? String.Empty : reader.GetDateTime(3).ToShortDateString();
        }

        private void UpdateStoredSettlePrices(Dictionary<string, Money> settlementPrices)
        {
            settlementPrices.Keys.ForEach(
                offProdSettleDate =>
                {
                    Money settlePrice = settlementPrices[offProdSettleDate];

                    _settlementPrices.AddOrUpdate(
                        offProdSettleDate,
                        settlePrice,
                        (settleDate, settlement) => settlePrice);
                });
        }
    }
}