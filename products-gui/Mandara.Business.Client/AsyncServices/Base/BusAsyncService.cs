using Mandara.Business.Bus.Commands.Base;
using Ninject.Extensions.Logging;

namespace Mandara.Business.AsyncServices.Base
{
    public abstract class BusAsyncService : AsyncService
    {
        private readonly CommandManager _commandManager;
        private CommandManager CommandManager { get { return _commandManager; } }

        protected BusAsyncService(CommandManager commandManager, ILogger log)
            : base(log)
        {
            _commandManager = commandManager;
        }

        protected void RunCommand(ICommand command)
        {
            CommandManager.AddCommand(command);
        }
    }
}

