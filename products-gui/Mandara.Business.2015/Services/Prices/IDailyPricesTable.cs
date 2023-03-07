using Mandara.Business.Services.Prices;
using System;
using System.Collections.Generic;

namespace Mandara.IRM.Server.Services
{
    public interface IDailyPricesTable : IPricesTable
    {
        DateTime? RiskDate { get; set; }
        void Update(Dictionary<string, int> headersMap, SortedList<int, decimal?[]> dataMap);
    }
}