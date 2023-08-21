using System.Linq;
using Mandara.Business.Bus.Commands.Base;

namespace Mandara.Business.Bus.Commands
{
    public class BusClientRestartCommand : BusCommandBase
    {
        public override void Execute()
        {
            // no data messages, we need to reconnect (restart BusClient)
            IoC.Get<BusClient>()
               .RestartBusClient(
                   InformaticaHelper.ServerPrefixes.Any() ? InformaticaHelper.ServerPrefixes[0] : string.Empty);
        }
    }
}