using System.Collections.Generic;
using Mandara.Entities;
using Mandara.Entities.Dto;

namespace Mandara.Business.Bus.Messages.EditTrades
{
    public class TradesEditMessageDto : TradesCancelMessage
    {
        public int? EditTradeId { get; set; }
        public int? Portfolio1 { get; set; }
        public int? Portfolio2 { get; set; }
        public List<TradeCaptureDto> NewTradeCaptures { get; set; }
    }
}