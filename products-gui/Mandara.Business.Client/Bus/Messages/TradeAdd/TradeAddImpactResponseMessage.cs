using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.TradeAdd
{
    public class TradeAddImpactResponseMessage : MessageBase
    {
        public TradeAddImpact TradesImpact { get; set; }
        public TradeAddImpact TransferTradesImpact { get; set; }
        public string ErrorMessage { get; set; }

        // TODO: The TradeAddImpactResponseMessage should not have null content by default.
        //public TradeAddImpactResponseMessage()
        //{
        //    TradesImpact = new TradeAddImpact();
        //    TransferTradesImpact = new TradeAddImpact();
        //    ErrorMessage = String.Empty;
        //}
    }
}