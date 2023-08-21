using System;

namespace Mandara.Business.Services.Prices
{
    public interface ISnapshotPricesProvider : IPricesProvider
    {
        DateTime? PriceTimestamp { get; }
        void UpdatePrices(DateTime snapshotDatetime);
    }
}