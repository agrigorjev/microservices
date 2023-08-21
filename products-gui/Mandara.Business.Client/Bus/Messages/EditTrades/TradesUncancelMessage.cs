using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.EditTrades
{
    public class TradesUncancelMessage : MessageBase
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int UncancelTradeId { get; set; }
        public string UncancelReason { get; set; }
    }
}