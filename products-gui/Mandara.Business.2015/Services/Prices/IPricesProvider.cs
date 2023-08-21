using Mandara.Entities;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Services.Prices
{
    public interface IPricesProvider
    {
        IEnumerable<TimeSpan> TodayTimestamps { get; }

        TryGetResult<decimal> TryGetFxPrice(DateTime valueDate, Currency currency1, Currency currency2);

        TryGetResult<Money> TryGetProductPrice(
            int productId,
            DateTime productDate,
            ProductDateType priceDateType,
            string mappingColumn,
            int officialProductId,
            DateTime? tradeStartDate = null,
            DateTime? tradeEndDate = null);

        TryGetResult<Money>[] GetProductPricesByMonth(
            int productId,
            DateTime productDate,
            ProductDateType priceDateType,
            string mappingColumn,
            int officialProductId,
            DateTime? tradeStartDate = null,
            DateTime? tradeEndDate = null);
    }
}
