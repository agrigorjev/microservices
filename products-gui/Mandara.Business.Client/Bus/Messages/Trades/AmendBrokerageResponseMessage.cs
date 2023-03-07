using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Trades
{
    public class AmendBrokerageResponseMessage : MessageBase
    {
        public string ErrorMessage { get; set; }
    }
}