using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.TradeAdd;

namespace Mandara.Business.Bus.Messages.TradeAdd
{
    public class TradeAddImpactRequestMessageDto : MessageBase
    {
        public TradeAddDetailsDto TradeAddDetails { get; set; }
        public bool IsMasterToolMode { get; set; }
    }
}