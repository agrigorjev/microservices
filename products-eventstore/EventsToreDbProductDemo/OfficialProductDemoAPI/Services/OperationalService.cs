using MandaraDemoDTO;
using OfficialProductDemoAPI.Services.Cache;
using OfficialProductDemoAPI.Services.Contracts;

namespace OfficialProductDemoAPI.Services
{
    public class OperationalService : IHostedService
    {
        private CacheService<OfficialProduct> _productsCache;
        private CacheService<Currency> _currencyCache;
        private CacheService<Unit> _unitCache;
        private CacheService<Region> _regionsCache;
        public OperationalService(CacheService<OfficialProduct> productsCache,
                           CacheService<Currency> currencyCache,
                            CacheService<Unit> unitCache,
                           CacheService<Region> regionsCache)
        {
            _productsCache = productsCache;
            _currencyCache = currencyCache;
            _unitCache = unitCache;
            _regionsCache = regionsCache;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _currencyCache.InitialLoad();
            await _unitCache.InitialLoad();
            await _regionsCache.InitialLoad();
            await _productsCache.InitialLoad();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.Run(()=>
            {
                _productsCache.Dispose();
                _currencyCache.Dispose();
                _unitCache.Dispose();
                _regionsCache.Dispose();
            }
            );

        }
    }
}
