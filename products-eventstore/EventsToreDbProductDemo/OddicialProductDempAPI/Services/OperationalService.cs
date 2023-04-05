using MandaraDemoDTO;
using OfficialProductDemoAPI.Services.Contracts;

namespace OfficialProductDemoAPI.Services
{
    public class OperationalService : IHostedService
    {
        IDataService<OfficialProduct> _cachesService;
        public OperationalService(IDataService<OfficialProduct> cachesService)
        {
            _cachesService = cachesService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
           await _cachesService.InitialLoad();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.Run(()=> _cachesService.Dispose());

        }
    }
}
