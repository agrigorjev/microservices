using Mandara.TradeApiService.Contracts;
using Mandara.TradeApiService.Configuration;
using Microsoft.Extensions.Options;

namespace Mandara.TradeApiService.BackgroundServices;

public class PeriodicallyReaderService : BackgroundService
{
    private readonly ServiceSettings _serviceSettings;
    private readonly IDataStorage _storage;
    public PeriodicallyReaderService(IOptions<ServiceSettings> serviceSettings,
        IDataStorage storage)
    {
        _serviceSettings = serviceSettings.Value;
        _storage = storage;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
           _storage.UpdatePortfolios();
            _storage.UpdateOfficialProducts();

            try
            {
                await Task.Delay(_serviceSettings.PollingIntervalMilliseconds, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
