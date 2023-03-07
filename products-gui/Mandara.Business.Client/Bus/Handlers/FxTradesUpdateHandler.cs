using com.latencybusters.lbm;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages.Trades;

namespace Mandara.Business.Bus.Handlers
{
    public class FxTradesUpdateHandler : MessageHandler<FxTradesUpdateMessage>
    {
        protected override void Handle(FxTradesUpdateMessage message)
        {
            BusClient.Instance.PendingFxTradesUpdates.Enqueue(message);
        }
    }
}