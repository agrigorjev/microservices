using Ninject.Extensions.Logging;

namespace Mandara.Business.Bus.Handlers.Base
{
    using System;
    using com.latencybusters.lbm;
    using Mandara.Business.Bus;
    using Mandara.Business.Bus.Messages.Base;

    public abstract class MessageHandler<T> : IHandler where T : MessageBase
    {
        protected abstract void Handle(T message);

        public void Handle(string topicName, LBMMessage lbmMessage, long receivedEpoch)
        {

            switch (lbmMessage.type())
            {
                case LBM.MSG_UNRECOVERABLE_LOSS:
                case LBM.MSG_UNRECOVERABLE_LOSS_BURST:
                    var log = new NLogLoggerFactory().GetCurrentClassLogger();
                    log.Warn("Unrecoverable loss on topic {0}", topicName);
                    break;
            }

            if (lbmMessage.type() != LBM.MSG_DATA)
                return;

            T message = JsonHelper.Deserialize<T>(lbmMessage.data());
            if (message != null)
            {
                message.ReceivedAt = receivedEpoch;
                Handle(message);
            }

        }
    }
}
