using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.PricingReport
{
    public class TasCheckSkipRequestMessage : MessageBase
    {
        public string KeyToSkip { get; set; }
    }
}