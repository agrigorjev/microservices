using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.Trades
{
    public class TradesSnapshotMessage : SnapshotMessageBase
    {
        // message parameters
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SpreadGroupTradeId { get; set; }
        public List<int> TradeIds { get; set; }
        public List<int> GroupIds { get; set; }

        // message data
        public List<TradeCapture> TradeCaptures { get; set; }

        public static TradesSnapshotMessage ForDateRange(DateTime? from, DateTime? to)
        {
            return new TradesSnapshotMessage()
            {
                StartDate = from,
                EndDate = to
            };
        }

        public static TradesSnapshotMessage ForSpreadGroup(int spreadGroupId)
        {
            return new TradesSnapshotMessage()
            {
                SpreadGroupTradeId = spreadGroupId
            };
        }

        public static TradesSnapshotMessage ForTrades(IEnumerable<int> tradeIds)
        {
            return new TradesSnapshotMessage()
            {
                TradeIds = new List<int>(tradeIds)
            };
        }

        public static TradesSnapshotMessage ForTradeGroups(IEnumerable<int> tradeGroupIds)
        {
            return new TradesSnapshotMessage()
            {
                GroupIds = new List<int>(tradeGroupIds)
            };
        }
    }
}