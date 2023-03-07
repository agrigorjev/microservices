using Mandara.Business.AsyncServices.Base;
using Mandara.Business.Bus;
using Mandara.Business.Bus.Commands.Base;
using Ninject.Extensions.Logging;

namespace Mandara.Business.AsyncServices
{
    public abstract class ClientAsyncService : BusAsyncService
    {
        public BusClient BusClient { get; private set; }

        public ClientAsyncService(CommandManager commandManager, ILogger log) : base(commandManager, log)
        {
        }

        public ClientAsyncService(CommandManager commandManager, BusClient busClient, ILogger log)
            : base(commandManager, log)
        {
            BusClient = busClient;
        }

        protected override void DoWork()
        {
        }
    }
}