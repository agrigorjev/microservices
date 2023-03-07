using Mandara.Business.Contracts;
using Mandara.Business.DataInterface;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Extensions.Option;
using Mandara.IRM.Server.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Mandara.Business.Services.Prices
{
    public class LivePricesStorage : PricesStorage, ILivePricesStorage
    {
        private readonly IPricesTable _livePricesTable;

        public LivePricesStorage(
            IPricesTable livePricesTable,
            IDailyPricesTable dailyPricesTable,
            IPricesTimestampsRepository pricesTimestampsRepository,
            IProductsStorage productsStorage,
            List<TimeSpan> todayTimestamps)
                : base(dailyPricesTable, pricesTimestampsRepository, productsStorage, todayTimestamps)
        {
            _livePricesTable = livePricesTable;
        }

        public virtual TryGetResult<Money> GetProductPrice(
            int productId,
            DateTime productDate,
            ProductDateType priceDateType,
            string mappingColumn,
            int officialProductId,
            DateTime? tradeStartDate = null,
            DateTime? tradeEndDate = null)
        {
            return GetProductPrice(
                productId,
                officialProductId,
                productDate,
                priceDateType,
                new DateRange(tradeStartDate, tradeEndDate),
                mappingColumn,
                _livePricesTable,
                SystemTime.Now());
        }

        public virtual void Update()
        {
            _livePricesTable.Update(
                pricesDbConnectionString,
                () => new SqlCommand("SELECT * FROM dbo.pnl_LivePriceView"));
            _dailyPricesTable.Update(_livePricesTable.PriceColHeaders, _livePricesTable.PricesByMonth);

            TodayTimestamps = _pricesTimestampsRepository.GetPriceTimestamps(SystemTime.Today());
        }

        public ILivePricesStorage GetFixedLivePrices()
        {
            Update();
            return new LivePricesStorage(
                _livePricesTable.ShallowCopy(),
                (IDailyPricesTable)_dailyPricesTable.ShallowCopy(),
                _pricesTimestampsRepository,
                _productsStorage,
                TodayTimestamps.ToList());
        }

        public TryGetResult<Money>[] GetProductPricesByMonth(
           int productId,
           DateTime productDate,
           ProductDateType priceDateType,
           string mappingColumn,
           int officialProductId,
           DateTime? tradeStartDate = null,
           DateTime? tradeEndDate = null)
        {
            return GetProductPricesByMonth(
                productId,
                productDate,
                priceDateType,
                mappingColumn,
                officialProductId,
                _livePricesTable,
                SystemTime.Now(),
                new DateRange(tradeStartDate, tradeEndDate));
        }

        public void Dispose()
        {
        }

        public void Stop()
        {
        }
    }
}