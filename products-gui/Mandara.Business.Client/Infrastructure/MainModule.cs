using com.latencybusters.lbm;
using Mandara.Business.AsyncServices.Base;
using Mandara.Business.Bus;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Handlers.Base;
using Ninject.Modules;

namespace Mandara.Business.Client.Infrastructure
{
    public class MainModule : NoCommsModule
    {
        public override void Load()
        {
            IoC.Initialize(Kernel);

            Bind<LBMContext>().ToSelf().InSingletonScope();
            base.Load();
        }
    }

    public class NoCommsModule : NinjectModule
    {
        public override void Load()
        {
            IoC.Initialize(Kernel);

            Bind<CommandManager>().ToSelf().InSingletonScope();
            Bind<AsyncServiceManager>().ToSelf().InSingletonScope();
            Bind<HandlerManager>().ToSelf().InSingletonScope();
            Bind<BusClient>().ToSelf().InSingletonScope();
        }
    }
}
