using Mandara.Entities;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;

namespace Mandara.IRM.Server.Services
{
    public interface IPricesStorage : IDisposable
    {
        List<TimeSpan> TodayTimestamps { get; }
        TryGetResult<Money> GetProductPrice(
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
        void Update();
    }
}