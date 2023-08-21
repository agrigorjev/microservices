using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;

namespace Mandara.Business.Bus.Handlers
{
    public class HeartbeatHandler : MessageHandler<HeartbeatMessage>
    {
        protected override void Handle(HeartbeatMessage heartbeatMessage)
        {
            BusClient.Instance.LastHeartbeat = new ServerHeartbeat(heartbeatMessage);
        }
    }
}