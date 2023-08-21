using Mandara.CalendarsService.Services;

namespace Mandara.CalendarsService.BackgroundServices
{
    public class CalendarReaderService : BackgroundService
    {
        private readonly ICalendarsStorage _storage;
        public CalendarReaderService(
            ICalendarsStorage storage)
        {
            _storage = storage;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
               _storage.Update();

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
