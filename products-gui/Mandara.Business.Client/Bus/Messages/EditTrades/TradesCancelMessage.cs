using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.EditTrades
{
    public class TradesCancelMessage : MessageBase
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public List<int> CancelTradeIds { get; set; }
        public string EditCancelReason { get; set; }
    }
}