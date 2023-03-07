using System.Collections.Generic;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.EditTrades
{
    public class TradesEditMessage : TradesCancelMessage
    {
        public int? EditTradeId { get; set; }
        public int? Portfolio1 { get; set; }
        public int? Portfolio2 { get; set; }
        public List<TradeCapture> NewTradeCaptures { get; set; }
    }
}