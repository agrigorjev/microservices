using Mandara.ProductService.Services;

namespace Mandara.ProductService.BackgroundServices
{
    public class ProductReaderService : BackgroundService
    {
        private readonly IProductStorage _storage;
        public ProductReaderService(
            IProductStorage storage)
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
