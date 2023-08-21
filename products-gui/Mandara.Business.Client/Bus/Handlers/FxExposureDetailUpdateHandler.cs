using com.latencybusters.lbm;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages.Trades;

namespace Mandara.Business.Bus.Handlers
{
    public class FxExposureDetailUpdateHandler : MessageHandler<FxExposureDetailUpdateMessage>
    {
        protected override void Handle(FxExposureDetailUpdateMessage message)
        {
            BusClient.Instance.PendingFxExposureDetailUpdates.Enqueue(message);
        }
    }
}
