using Mandara.Business.Contracts;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Mandara.Business.Services.Prices
{
    public interface IPricesTable
    {
        /// <summary>
        /// Get price for a specified period/column pair
        /// </summary>
        decimal? GetPrice(int period, string column);

        /// <summary>
        /// Get average price for a period, for a specified column
        /// </summary>
        decimal? GetAveragePrice(
            int periodStart,
            int periodEnd,
            string column,
            int productId,
            DateTime startMonth,
            IProductsStorage productsStorage);

        TryGetResult<decimal>[] GetPricesByMonth(
            int periodStart,
            int periodEnd,
            string column,
            int productId,
            DateTime startMonth,
            IProductsStorage productsStorage);

        void Update(SqlConnectionStringBuilder pricesDbConnectionString, Func<SqlCommand> createCommand);

        IPricesTable ShallowCopy();

        Dictionary<string, int> PriceColHeaders { get; }
        SortedList<int, decimal?[]> PricesByMonth { get; }
    }
}