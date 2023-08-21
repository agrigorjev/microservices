using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;

namespace Mandara.Business.Bus.Handlers
{
    public class ClientReconnectHandler : MessageHandler<ClientReconnectMessage>
    {
        protected override void Handle(ClientReconnectMessage message)
        {
            if (message == null)
                return;

            BusClient.Instance.TriggerServerReconnection(message.Prefix);
        }
    }
}