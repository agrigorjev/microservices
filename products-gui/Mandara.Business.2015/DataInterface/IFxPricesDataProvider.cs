using System;
using System.Collections.Generic;

namespace Mandara.Business.DataInterface
{
    public interface IFxPricesDataProvider
    {
        ICurrencyProvider CurrencyProvider { get; }
        Dictionary<string, decimal> GetLivePrices();
        Dictionary<string, decimal> GetSnapshotPrices(DateTime snapshotDateTime);
    }
}