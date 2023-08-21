using AutoMapper;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages.Positions;
using com.latencybusters.lbm;

namespace Mandara.Business.Bus.Handlers
{
    public class PositionsUpdateHandler : IHandler
    {
        public void Handle(string topicName, LBMMessage lbmMessage, long receivedEpoch)
        {
            if (LBM.MSG_DATA != lbmMessage.type())
            {
                return;
            }

            PositionsUpdateMessage message = GetPositionsUpdate();

            if (message == null)
            {
                return;
            }

            BusClient.Instance.PendingPositionsUpdates.Enqueue(message);

            PositionsUpdateMessage GetPositionsUpdate()
            {
                return Mapper.Map<PositionsUpdateMessageDto, PositionsUpdateMessage>(
                    JsonHelper.Deserialize<PositionsUpdateMessageDto>(lbmMessage.data()));
            }
        }
    }
}