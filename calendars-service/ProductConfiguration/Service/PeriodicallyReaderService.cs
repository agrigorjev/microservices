using Mandara.ProductConfiguration.Contracts;

namespace Mandara.ProductConfiguration.Services;

public class PeriodicallyReaderService : BackgroundService
{
    private readonly IDataStorage _storage;
    public PeriodicallyReaderService(
        IDataStorage storage)
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
