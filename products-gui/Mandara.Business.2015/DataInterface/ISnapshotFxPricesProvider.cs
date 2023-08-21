using System;

namespace Mandara.Business.DataInterface
{
    public interface ISnapshotFxPricesProvider : IFxPricesProvider
    {
        void UpdatePrices(DateTime snapshotDateTime);
    }
}