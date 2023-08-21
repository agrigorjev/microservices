using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.PricingReport
{
    public class TasCheckSkipResponseMessage : MessageBase
    {
        public string Response { get; set; }
    }
}