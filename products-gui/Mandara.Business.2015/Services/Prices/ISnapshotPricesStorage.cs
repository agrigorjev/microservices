using System;
using System.Data.SqlTypes;

namespace Mandara.IRM.Server.Services
{
    public interface ISnapshotPricesStorage : IPricesStorage
    {
        DateTime? PriceTimestamp { get; set; }
    }
}