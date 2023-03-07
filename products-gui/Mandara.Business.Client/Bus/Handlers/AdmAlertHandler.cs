using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages.AdmAlerts;
using Mandara.Business.PublishSubscribe;

namespace Mandara.Business.Bus.Handlers
{
    public class AdmAlertHandler : MessageHandler<AdmAlertMessage>
    {
        protected override void Handle(AdmAlertMessage message)
        {
            if (message != null && message.AdmAlert != null)
            {
                if (BusClient.IsCurrentUserCanSeeAdmAlert(message.UserName))
                    PubSub.SendMessage(message.AdmAlert);
            }
        }
    }
}