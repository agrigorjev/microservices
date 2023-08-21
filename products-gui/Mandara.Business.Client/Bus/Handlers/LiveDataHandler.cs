using com.latencybusters.lbm;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;
using System.Linq;

namespace Mandara.Business.Bus.Handlers
{
    public class LiveDataHandler : MessageHandler<LiveDataMessage>
    {
        protected override void Handle(LiveDataMessage message)
        {
            
            if (!IsValidLiveData())
            {
                return;
            }

            BusClient.Instance.LiveData = message.LiveDataCollection.ToList();

            bool IsValidLiveData()
            {
                return null != message?.LiveDataCollection;
            }
        }
    }
}