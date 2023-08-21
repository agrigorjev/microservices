namespace Mandara.Business.Bus.Handlers.Base
{
    using com.latencybusters.lbm;
    using JetBrains.Annotations;
    using Mandara.Business.Bus.Messages.Base;

    public abstract class RequestHandler<T> : IHandler where T : MessageBase
    {
        protected IResponder _responder;
        protected RequestHandler([NotNull]IResponder responder)
        {
            _responder = responder;
        }

        public abstract void Handle(T message);

        private LBMMessage _request;
        private InformaticaResponder _respond;

        public void Handle(string topicName, LBMMessage req, long receivedEpoch)
        {
            _request = req;
            _respond = InformaticaHelper.BuildResponder(req);

            using (req)
            {
                if (req.type() != LBM.MSG_REQUEST)
                {
                    return;
                }

                T message = JsonHelper.Deserialize<T>(req.data());
                if (message != null)
                {
                    message.ReceivedAt = receivedEpoch;
                    _request = req;
                    Handle(message);
                }
            }
        }

        public virtual void Respond(MessageBase message)
        {
            //_responder.Respond(_lbmMessage, message);
            _responder.Respond(_respond, message);
        }
    }
}
