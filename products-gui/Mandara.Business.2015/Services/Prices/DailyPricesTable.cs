using Mandara.Business.Contracts;
using Mandara.Business.Services.Prices;
using Mandara.Date.Time;
using Mandara.Entities;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Mandara.IRM.Server.Services
{
    [Serializable]
    public class DailyPricesTable : PricesTable, IDailyPricesTable
    {
        public DateTime? RiskDate { get; set; }

        private readonly IProductsStorage _productsStorage;

        [Inject]
        public DailyPricesTable(IProductsStorage productsStorage, ILogger log)
            : base(log)
        {
            _productsStorage = productsStorage;
        }

        public DailyPricesTable(IProductsStorage productsStorage, ILogger log, Dictionary<string, int> headersMap = null,
            SortedList<int, decimal?[]> dataMap = null)
            : base(productsStorage, log, headersMap, dataMap)
        {
            _productsStorage = productsStorage;
        }

        public void Update(Dictionary<string, int> headersMap, SortedList<int, decimal?[]> dataMap)
        {
            PriceColHeaders = new Dictionary<string, int>(headersMap);
            PricesByMonth = new SortedList<int, decimal?[]>(dataMap);
        }

        public override IPricesTable ShallowCopy()
        {
            return new DailyPricesTable(_productsStorage, Log, new Dictionary<string, int>(PriceColHeaders), new SortedList<int, decimal?[]>(PricesByMonth));
        }

        public override decimal? GetAveragePrice(int periodStart, int periodEnd, string column, int productId, DateTime startMonth, IProductsStorage productsStorage)
        {
            column = column.ToLowerInvariant().Trim();

            if (periodEnd < 72)
                return null;

            if (periodStart < 72)
                periodStart = 72;

            if (periodEnd < periodStart)
                return null;

            DateTime riskDate = RiskDate ?? SystemTime.Today();

            int diffStartDays = periodStart - 72;
            DateTime startDay = riskDate.AddDays(diffStartDays);

            int adjustedStart = periodStart;

            if (periodStart > 72)
            {
                adjustedStart = 72;
                for (DateTime d = riskDate; d < startDay; d = d.AddDays(1))
                {
                    if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                    {
                        continue;
                    }

                    adjustedStart++;
                }
            }

            int diffEndDays = periodEnd - 72;
            DateTime endDay = riskDate.AddDays(diffEndDays);

            int adjustedEnd = 72;
            for (DateTime d = riskDate; d < endDay; d = d.AddDays(1))
            {
                if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }

                adjustedEnd++;
            }

            int idx;
            if (!PriceColHeaders.TryGetValue(column, out idx))
                return null;

            decimal sum = 0M;
            bool allIsNull = true;
            int numDataPoints = 0;
            int skippedDays = 0;

            DateTime day = startDay;
            for (int i = adjustedStart; i <= adjustedEnd; i++, day = day.AddDays(1))
            {
                if (IsHoliday(productId, day))
                {
                    skippedDays++;
                    continue;
                }

                decimal?[] row;
                if (!PricesByMonth.TryGetValue(i - skippedDays, out row))
                {
                    // we don't have prices for specified period, check next then
                    continue;
                }

                // check if all the prices are null
                if (row[idx] != null)
                    allIsNull = false;

                // increase period's counter
                numDataPoints++;

                // sum prices
                sum += (row[idx] ?? 0M);
            }

            // if all the prices are null, return null (keeping the old )
            if (allIsNull)
                return null;

            // return avg price
            return sum / (decimal)numDataPoints;
        }

        private bool IsHoliday(int productId, DateTime day)
        {
            Product product = _productsStorage.TryGetProduct(productId).Value;

            return _productsStorage.HasHoliday(product.calendar_id, day.Date);
        }
    }
}