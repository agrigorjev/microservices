using Mandara.Business.Contracts;
using Mandara.Business.DataInterface;
using Mandara.Date;
using Mandara.Entities;
using Mandara.Extensions.Option;
using Mandara.IRM.Server.Services;
using System;
using System.Data.SqlClient;

namespace Mandara.Business.Services.Prices
{
    public class SnapshotPricesStorage : PricesStorage, ISnapshotPricesStorage
    {
        public DateTime? PriceTimestamp
        {
            get
            {
                return _priceTimestamp;
            }
            set
            {
                _priceTimestamp = value;
                _isSnapshotInitialized = false;
            }
        }

        private readonly IPricesTable _snapshotPricesTable;

        private bool _isSnapshotInitialized;
        private DateTime? _priceTimestamp;

        public SnapshotPricesStorage(
            IPricesTable snapshotPricesTable,
            IDailyPricesTable dailyPricesTable,
            IPricesTimestampsRepository pricesTimestampsRepository,
            IProductsStorage productsStorage)
                : base(dailyPricesTable, pricesTimestampsRepository, productsStorage, null)
        {
            _snapshotPricesTable = snapshotPricesTable;
            _isSnapshotInitialized = false;
        }

        private void InitStorage()
        {
            if (!_isSnapshotInitialized)
            {
                Update();
                _isSnapshotInitialized = true;
            }
            if (_priceTimestamp.HasValue)
            {
                _dailyPricesTable.RiskDate = _priceTimestamp.Value;
            }
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
            InitStorage();
            if (_priceTimestamp == null)
            {
                return new TryGetVal<Money>();
            }

            return GetProductPrice(
                productId,
                officialProductId,
                productDate,
                priceDateType,
                new DateRange(tradeStartDate, tradeEndDate),
                mappingColumn,
                _snapshotPricesTable,
                _priceTimestamp.Value);
        }

        public virtual void Update()
        {
            if (PriceTimestamp == null)
            {
                return;
            }

            int unixTimestamp = EpochConverter.ToEpochTime(PriceTimestamp.Value);
            _snapshotPricesTable.Update(
                pricesDbConnectionString,
                () =>
                {
                    SqlCommand comm = new SqlCommand("SELECT * FROM dbo.pnl_PriceView WHERE sdate = @timestamp");
                    comm.Parameters.Add(new SqlParameter("@timestamp", unixTimestamp));

                    return comm;
                });
            _dailyPricesTable.Update(_snapshotPricesTable.PriceColHeaders, _snapshotPricesTable.PricesByMonth);

            TodayTimestamps = _pricesTimestampsRepository.GetPriceTimestamps(PriceTimestamp.Value.Date);
        }

        public TryGetResult<Money>[] GetProductPricesByMonth(
            int productId,
            DateTime productDate,
            ProductDateType priceDateType,
            string mappingColumn,
            int officialProductId,
            DateTime? tradeStartDate = default(DateTime?),
            DateTime? tradeEndDate = default(DateTime?))
        {
            if (_priceTimestamp == null)
            {
                return new TryGetResult<Money>[0];
            }
            InitStorage();

            return GetProductPricesByMonth(
                productId,
                productDate,
                priceDateType,
                mappingColumn,
                officialProductId,
                _snapshotPricesTable,
                _priceTimestamp.Value,
                new DateRange(tradeStartDate, tradeEndDate));
        }

        public void Dispose()
        {
        }
    }
}