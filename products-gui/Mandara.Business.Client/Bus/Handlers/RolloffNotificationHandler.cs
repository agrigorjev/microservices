using System.Linq;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;
using com.latencybusters.lbm;

namespace Mandara.Business.Bus.Handlers
{
    public class RolloffNotificationHandler : MessageHandler<RolloffNotificationMessage>
    {
        protected override void Handle(RolloffNotificationMessage message)
        {
            if (HasRollOffDetails())
            {
                BusClient.Instance.OnRolloff(message.RolloffDetails);
            }

            bool HasRollOffDetails()
            {
                return message?.RolloffDetails != null && message.RolloffDetails.Count > 0;
            }
        }
    }
}