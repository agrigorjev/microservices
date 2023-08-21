using System.Collections.Generic;
using Mandara.Business.Model;

namespace Mandara.Business.Contracts
{
    public class TradesQuantityData
    {
        public List<SdQuantityModel> SecurityDefintionBasedQuantities { get; private set; }
        public List<SdQuantityModel> SecurityDefintionBasedLegsQuantities { get; private set; }
        public List<TradeModel> CustomPeriodTrades { get; private set; }
        public int LatestTradeId { get; private set; }
        public int LatestSecurityDefinitionId { get; private set; }
        public int LatestTradeChangeId { get; private set; }

        public TradesQuantityData(
            List<SdQuantityModel> sdQuantities,
            List<SdQuantityModel> sdLegsQuantities,
            List<TradeModel> customPeriodTrades,
            int latestTradeId,
            int latestSecDefId,
            int latestTradeChangeId)
        {
            SecurityDefintionBasedQuantities = sdQuantities ?? new List<SdQuantityModel>();
            SecurityDefintionBasedLegsQuantities = sdLegsQuantities ?? new List<SdQuantityModel>();
            CustomPeriodTrades = customPeriodTrades ?? new List<TradeModel>();
            LatestTradeId = latestTradeId;
            LatestTradeChangeId = latestTradeChangeId;
            LatestSecurityDefinitionId = latestSecDefId;
        }
    }
}