using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Trades
{
    public class AmendBrokerageRequestMessage : MessageBase
    {
        public int TradeCaptureId { get; set; }
        public decimal Brokerage { get; set; }
    }
}