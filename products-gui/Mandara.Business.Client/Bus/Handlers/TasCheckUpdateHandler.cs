using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;
using Mandara.Business.Bus.Messages.PricingReport;
using com.latencybusters.lbm;

namespace Mandara.Business.Bus.Handlers
{
    public class TasCheckUpdateHandler : MessageHandler<TasCheckDetailResponseMessage>
    {
        protected override void Handle(TasCheckDetailResponseMessage message)
        {
            if (null != message?.TasCheckDetails)
            {
                BusClient.Instance.OnTasCheckUpdate(message.TasCheckDetails);
            }
        }
    }
}