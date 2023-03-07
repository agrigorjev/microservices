using Mandara.Business.Calculators;
using Mandara.Business.Contracts;
using Mandara.Database.Query;
using Mandara.Entities;
using Mandara.Extensions.Option;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Mandara.Business.Services.Prices
{
    [Serializable]
    public class PricesTable : IPricesTable
    {
        private readonly IProductsStorage _productsStorage;
        protected readonly ILogger Log;

        public Dictionary<string, int> PriceColHeaders { get; protected set; }
        public SortedList<int, decimal?[]> PricesByMonth { get; protected set; }

        public PricesTable(
            IProductsStorage productsStorage,
            ILogger log,
            Dictionary<string, int> headersMap = null,
            SortedList<int, decimal?[]> dataMap = null)
        {
            _productsStorage = productsStorage;
            Log = log;
            PriceColHeaders = headersMap ?? new Dictionary<string, int>();
            PricesByMonth = dataMap ?? new SortedList<int, decimal?[]>();
        }

        [Inject]
        public PricesTable(ILogger log)
        {
            Log = log;

            PriceColHeaders = new Dictionary<string, int>();
            PricesByMonth = new SortedList<int, decimal?[]>();
        }

        public virtual decimal? GetPrice(int period, string column)
        {
            column = column.ToLowerInvariant().Trim();

            if (!PriceColHeaders.TryGetValue(column, out int idx))
            {
                return null;
            }

            return !PricesByMonth.TryGetValue(period, out decimal?[] row) ? null : row[idx];
        }

        public TryGetResult<decimal>[] GetPricesByMonth(int periodStart,
            int periodEnd,
            string column,
            int productId,
            DateTime startMonth,
            IProductsStorage productsStorage)
        {
            if (null == productsStorage)
            {
                return new TryGetResult<decimal>[0];
            }

            column = column.ToLowerInvariant().Trim();

            if (!PriceColHeaders.TryGetValue(column, out int idx))
            {
                return new TryGetResult<decimal>[0];
            }

            bool allIsNull = true;
            DateTime month = startMonth;
            Product product;
            TryGetResult<Product> getProductResult = productsStorage.TryGetProduct(productId);

            if (!getProductResult.HasValue)
            {
                return new TryGetResult<decimal>[0];
            }

            product = getProductResult.Value;

            TryGetResult<decimal>[] result = new TryGetResult<decimal>[periodEnd - periodStart + 1];

            for (int i = periodStart; i <= periodEnd; i++, month = month.AddMonths(1))
            {
                decimal?[] row;
                if (!PricesByMonth.TryGetValue(i, out row) || row[idx] == null)
                {
                    result[i - periodStart] = new TryGetVal<decimal>();
                    continue;
                }

                allIsNull = false;
                result[i - periodStart] = row[idx].HasValue ? new TryGetVal<decimal>(row[idx].Value)
                    : new TryGetVal<decimal>();
            }

            if (allIsNull)
            {
                return new TryGetResult<decimal>[0];
            }

            return result;
        }

        public virtual decimal? GetAveragePrice(
            int periodStart,
            int periodEnd,
            string column,
            int productId,
            DateTime startMonth,
            IProductsStorage productsStorage)
        {
            // TODO: Why would this ever be called with a null reference?  It'd be better to throw a null reference 
            // exception and fix the actual problem than have a null check.
            if (null == productsStorage)
            {
                return null;
            }

            column = column.ToLowerInvariant().Trim();

            int idx;

            if (!PriceColHeaders.TryGetValue(column, out idx))
            {
                return null;
            }

            decimal sum = 0M;
            bool allIsNull = true;
            int numDataPoints = 0;
            DateTime month = startMonth;
            Product product;
            TryGetResult<Product> getProductResult = productsStorage.TryGetProduct(productId);

            if (!getProductResult.HasValue)
            {
                return null;
            }

            product = getProductResult.Value;

            for (int i = periodStart; i <= periodEnd; i++, month = month.AddMonths(1))
            {
                decimal?[] row;
                if (!PricesByMonth.TryGetValue(i, out row))
                {
                    // we don't have prices for specified period, check next then
                    continue;
                }

                // check if all the prices are null
                if (row[idx] != null)
                    allIsNull = false;

                decimal baseAppliedContractSizeMultiplier =
                    ContractSizeCalculator.ApplyContractSizeMultiplier(
                        1.0M,
                        product.ContractSizeMultiplier,
                        month);

                // increase period's counter
                int monthDataPoints = (int)baseAppliedContractSizeMultiplier;

                numDataPoints += monthDataPoints;

                // sum prices
                decimal monthPriceBase = (row[idx] ?? 0M);
                decimal monthPrice = monthPriceBase * baseAppliedContractSizeMultiplier;

                sum += monthPrice;
            }

            // if all the prices are null, return null (keeping the old )
            if (allIsNull)
                return null;

            // return avg price
            return sum / (decimal)numDataPoints;
        }

        public virtual void Update(SqlConnectionStringBuilder pricesDbConnectionString, Func<SqlCommand> createCommand)
        {
            try
            {
                (PriceColHeaders, PricesByMonth) = SqlServerCommandExecution.ExecuteReaderQuery(
                    pricesDbConnectionString,
                    (conn) => GetPriceUpdateCommand(createCommand, conn),
                    ReadPrices);
            }
            catch (Exception ex)
            {
                Log.ErrorException("Error updating prices", ex);
            }
        }

        private static (Dictionary<string, int>, SortedList<int, decimal?[]>) ReadPrices(SqlDataReader reader)
        {
            Dictionary<string, int> headers = new Dictionary<string, int>();
            SortedList<int, decimal?[]> pricesByMonth = new SortedList<int, decimal?[]>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string headerName = reader.GetName(i).ToLowerInvariant().Trim();

                if (!headers.ContainsKey(headerName))
                {
                    headers.Add(headerName, i);
                }
            }

            while (reader.Read())
            {
                decimal?[] dataArray = new decimal?[reader.FieldCount];

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object value = reader.GetValue(i);

                    if (value != null && value != DBNull.Value)
                    {
                        dataArray[i] = Convert.ToDecimal(value);
                    }
                }

                int month = (int)reader["rdate"];

                if (!pricesByMonth.ContainsKey(month))
                {
                    pricesByMonth.Add(month, dataArray);
                }
            }

            return (headers, pricesByMonth);
        }

        private static SqlCommand GetPriceUpdateCommand(Func<SqlCommand> createCommand, SqlConnection conn)
        {
            SqlCommand comm = createCommand();
            comm.Connection = conn;
            return comm;
        }

        public virtual IPricesTable ShallowCopy()
        {
            return new PricesTable(_productsStorage, Log, new Dictionary<string, int>(PriceColHeaders), new SortedList<int, decimal?[]>(PricesByMonth));
        }
    }
}