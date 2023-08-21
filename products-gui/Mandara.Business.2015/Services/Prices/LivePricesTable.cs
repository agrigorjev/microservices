using Mandara.Business.Calculators;
using Mandara.Business.Contracts;
using Mandara.Entities;
using Mandara.Extensions.Option;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Mandara.Extensions.Collections;

namespace Mandara.Business.Services.Prices
{
    [Serializable]
    public class LivePricesTable : IPricesTable
    {
        protected readonly ILogger logger;

        public Dictionary<string, int> PriceColHeaders { get; protected set; }
        public SortedList<int, decimal?[]> PricesByMonth { get; protected set; }

        public LivePricesTable(
            ILogger log,
            Dictionary<string, int> headersMap = null,
            SortedList<int, decimal?[]> dataMap = null)
        {
            logger = log;

            PriceColHeaders = headersMap ?? new Dictionary<string, int>();
            PricesByMonth = dataMap ?? new SortedList<int, decimal?[]>();
        }

        [Inject]
        public LivePricesTable(ILogger log)
        {
            logger = log;

            PriceColHeaders = new Dictionary<string, int>();
            PricesByMonth = new SortedList<int, decimal?[]>();
        }

        public virtual decimal? GetPrice(int period, string column)
        {
            if (!PriceColHeaders.TryGetValue(column.ToLowerInvariant().Trim(), out int idx))
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

            DateTime month = startMonth;
            TryGetResult<Product> getProductResult = productsStorage.TryGetProduct(productId);

            if (!getProductResult.HasValue)
            {
                return new TryGetResult<decimal>[0];
            }

            TryGetResult<decimal>[] result = new TryGetResult<decimal>[periodEnd - periodStart + 1];

            for (int i = periodStart; i <= periodEnd; i++, month = month.AddMonths(1))
            {
                if (!PricesByMonth.TryGetValue(i, out decimal?[] row) || row[idx] == null)
                {
                    result[i - periodStart] = new TryGetVal<decimal>();
                    continue;
                }

                result[i - periodStart] = row[idx].HasValue ? new TryGetVal<decimal>(row[idx].Value)
                    : new TryGetVal<decimal>();
            }

            return result;
        }

        public void Update(SqlConnectionStringBuilder pricesDbConnectionString, Func<SqlCommand> createCommand)
        {
            throw new NotImplementedException();
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

            if (!PriceColHeaders.TryGetValue(column, out int idx))
            {
                return null;
            }

            decimal? sum = null;
            int numDataPoints = 0;
            DateTime month = startMonth;
            TryGetResult<Product> getProductResult = productsStorage.TryGetProduct(productId);

            if (!getProductResult.HasValue)
            {
                return null;
            }

            Product product = getProductResult.Value;

            for (int monthIndex = periodStart; monthIndex <= periodEnd; monthIndex++, month = month.AddMonths(1))
            {
                if (!PricesByMonth.TryGetValue(monthIndex, out decimal?[] row))
                {
                    continue;
                }

                decimal baseAppliedContractSizeMultiplier =
                    ContractSizeCalculator.ApplyContractSizeMultiplier(
                        1.0M,
                        product.ContractSizeMultiplier,
                        month);

                numDataPoints += (int)baseAppliedContractSizeMultiplier;

                decimal monthPriceBase = (row[idx] ?? 0M);
                decimal monthPrice = monthPriceBase * baseAppliedContractSizeMultiplier;

                sum = sum.HasValue ? sum + monthPrice : monthPrice;
            }

            return sum / numDataPoints;
        }

        internal virtual void Update(LivePriceSnapshot allPrices)
        {
            Dictionary<string, int> headersMap = new Dictionary<string, int>();
            SortedList<int, decimal?[]> dataMap = new SortedList<int, decimal?[]>();

            try
            {
                int numberOfProducts = allPrices.Prices.Count;

                allPrices.Prices.ForEachWithIndex(
                    (productIdx, productPrices) =>
                    {
                        string colName = productPrices.Column.ToLowerInvariant().Trim();

                        if (!headersMap.ContainsKey(colName))
                        {
                            headersMap.Add(colName, productIdx);
                        }

                        productPrices.Prices.ForEachWithIndex(
                            (monthIdx, price) =>
                            {
                                if (!dataMap.ContainsKey(monthIdx + 72))
                                {
                                    dataMap.Add(monthIdx + 72, new decimal?[numberOfProducts]);
                                }

                                dataMap[monthIdx + 72][productIdx] =
                                    Double.IsNaN(price) ? default(decimal?) : (decimal)price;
                            });
                    });

                PriceColHeaders = headersMap;
                PricesByMonth = dataMap;
            }
            catch (Exception ex)
            {
                logger.ErrorException("Error updating prices", ex);
            }
        }

        public virtual IPricesTable ShallowCopy()
        {
            return new LivePricesTable(
                logger,
                new Dictionary<string, int>(PriceColHeaders),
                new SortedList<int, decimal?[]>(PricesByMonth));
        }
    }
}