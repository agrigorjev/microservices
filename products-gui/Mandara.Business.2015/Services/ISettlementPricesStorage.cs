using Mandara.Entities;
using System;

namespace Mandara.IRM.Server.Services
{
    public interface ISettlementPricesStorage
    {
        bool TryGetPrice(int officialProductId, DateTime settlementDate, DateTime startDate, out Money price);
        void Update();
    }
}