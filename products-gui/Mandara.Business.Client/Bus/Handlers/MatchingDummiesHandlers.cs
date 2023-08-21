using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Bus.Messages;

namespace Mandara.Business.Bus.Handlers
{ 
    public class MatchingDummiesUpdateHandler : MessageHandler<MatchingDummiesSnapshotMessage>
    {
        protected override void Handle(MatchingDummiesSnapshotMessage message)
        {
            BusClient.Instance.UpdateMatchingDummies(message.MatchingDummiesObjectCollection);
        }
    }
}