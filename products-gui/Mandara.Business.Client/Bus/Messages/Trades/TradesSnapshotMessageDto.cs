using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;
using Mandara.Entities.Dto;

namespace Mandara.Business.Bus.Messages.Trades
{
    public class TradesSnapshotMessageDto : SnapshotMessageBase
    {
        // message parameters
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SpreadGroupTradeId { get; set; }
        public List<int> TradeIds { get; set; }
        public List<int> GroupIds { get; set; }

        // message data
        public List<TradeCaptureDto> TradeCaptures { get; set; }
    }
}