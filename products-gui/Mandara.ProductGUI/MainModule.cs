using com.latencybusters.lbm;
using Mandara.Business.AsyncServices.Base;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Handlers.Base;
using Mandara.Business.Contracts;
using Mandara.Business.Data;
using Mandara.Business.DataInterface;
using Mandara.Business.Services;
using Mandara.ProductGUI.Desks;
using Ninject.Modules;

namespace Mandara.ProductGUI
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Bind<LBMContext>().ToSelf().InSingletonScope();
            Bind<CommandManager>().ToSelf().InSingletonScope();
            Bind<AsyncServiceManager>().ToSelf().InSingletonScope();
            Bind<HandlerManager>().ToSelf().InSingletonScope();

            Bind<ISequenceNumbersCache>().To<SequenceNumbersInMemoryCache>().InSingletonScope();

            Bind<IPortfoliosStorage>().To<PortfoliosStorage>().InSingletonScope();
            Bind<ICurrencyProvider>().To<CurrencyProvider>().InTransientScope();
            Bind<IProductsStorage>().To<ProductsStorage>().InSingletonScope();
            Bind<ISecurityDefinitionsStorage>().To<SecurityDefinitionsStorage>().InSingletonScope();

            Bind<IPositionCalculationService>().To<PositionCalculationService>();

            Bind<ITradesRepository>().To<TradesRepository>();

            Bind<ICsvImportService>().To<CsvImportService>().InSingletonScope();
        }
    }
}
