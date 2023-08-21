using com.latencybusters.lbm;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Handlers.Base
{
    public class BusResponder : IResponder
    {
        public void Respond(InformaticaResponder busMessage, MessageBase message)
        {
            InformaticaHelper.RespondToMessage(busMessage, message);
        }
    }
}