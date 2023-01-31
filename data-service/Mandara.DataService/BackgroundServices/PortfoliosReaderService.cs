using Mandara.DataService.Services;

namespace Mandara.DataService.BackgroundServices
{
    public class PortfoliosReaderService : BackgroundService
    {
        private readonly IPortfoliosStorage _portfoliosStorage;
        public PortfoliosReaderService(
            IPortfoliosStorage portfolioStorage)
        {
            _portfoliosStorage = portfolioStorage;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _portfoliosStorage.Update();

                try
                {
                    await Task.Delay(1000*60*5, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
    }
}
