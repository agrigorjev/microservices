using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages.AdmAlerts;
using Mandara.Business.PublishSubscribe;

namespace Mandara.Business.Bus.Handlers
{
    public class AdmAlertAcknowledgeDoneHandler : MessageHandler<AcknowledgeDoneMessage>
    {
        protected override void Handle(AcknowledgeDoneMessage message)
        {
            if (message != null)
                PubSub.SendMessage(message);
        }
    }
}