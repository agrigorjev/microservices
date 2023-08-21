using AutoMapper;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages.Trades;
using com.latencybusters.lbm;

namespace Mandara.Business.Bus.Handlers
{
    public class TradesUpdateHandler : IHandler
    {
        public void Handle(string topicName, LBMMessage lbmMessage, long receivedEpoch)
        {
            if (LBM.MSG_DATA != lbmMessage.type())
            {
                return;
            }

            TradesUpdateMessage message = GetTradesUpdate();

            if (message == null)
            {
                return;
            }

            message.ReceivedAt = receivedEpoch;
            BusClient.Instance.PendingTradesUpdates.Enqueue(message);

            TradesUpdateMessage GetTradesUpdate()
            {
                return Mapper.Map<TradesUpdateMessageDto, TradesUpdateMessage>(
                    JsonHelper.Deserialize<TradesUpdateMessageDto>(lbmMessage.data()));
            }
        }
    }
}