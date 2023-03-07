using com.latencybusters.lbm;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Handlers.Base
{
    public interface IResponder
    {
        void Respond(InformaticaResponder busMessage , MessageBase message);
    }
}