using Mandara.CalendarsService.Services;

namespace Mandara.CalendarsService.BackgroundServices
{
    public class CalendarsReaderService : BackgroundService
    {
        private readonly ICalendarsStorage _portfoliosStorage;
        public CalendarsReaderService(
            ICalendarsStorage portfolioStorage)
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
