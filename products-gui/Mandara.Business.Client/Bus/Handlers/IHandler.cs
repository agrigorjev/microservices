using com.latencybusters.lbm;

namespace Mandara.Business.Bus.Handlers.Base
{
    public interface IHandler
    {
        void Handle(string topicName, LBMMessage lbmMessage, long receivedEpoch);
    }
}