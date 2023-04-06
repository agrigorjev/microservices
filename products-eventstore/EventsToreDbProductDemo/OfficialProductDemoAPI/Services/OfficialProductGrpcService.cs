

using Grpc.Core;
using MandaraDemo.GrpcDefinitions;
using MandaraDemoDTO;
using OfficialProductDemoAPI.Services.Cache;
using OfficialProductDemoAPI.Services.Contracts;
using Optional;
using static MandaraDemo.GrpcDefinitions.ProductAPIService;

namespace OfficialProductDemoAPI.Services
{
    public class OfficialProductGrpcService : ProductAPIServiceBase
    {
        private CacheService<OfficialProduct> _productsCache;
        private CacheService<Currency> _currencyCache;
        private CacheService<Unit> _unitCache;
        private CacheService<Region> _regionsCache;
        private OfficialProductConverter _converter=new OfficialProductConverter();
        private PriceUnitDataConverter _priceUnitDataConverter = new PriceUnitDataConverter();
        private CurrencyDataConverter _currencyDataConverter = new CurrencyDataConverter();
        private RegionDataConverter _regionDataConverter = new RegionDataConverter();
        public OfficialProductGrpcService(CacheService<OfficialProduct> productsCache,
                           CacheService<Currency> currencyCache,
                            CacheService<Unit> unitCache,
                           CacheService<Region> regionsCache)
        {
            _productsCache=productsCache;
            _currencyCache = currencyCache;
            _unitCache=unitCache;
            _regionsCache=regionsCache;
        }

        public override Task<ProductsGrpcMessage> GetAllProducts(GetAllRequestMessage request, ServerCallContext context)
        {

            ProductsGrpcMessage productsGrpcMessage = new ProductsGrpcMessage();
            _productsCache.GetList().ForEach(p => productsGrpcMessage.Products.Add(_converter.Convert(p)));
            return Task.FromResult(productsGrpcMessage);
        }

        public override Task<CurrencyGrpcMessage> GetCurrencyReference(GetAllRequestMessage request, ServerCallContext context)
        {
            CurrencyGrpcMessage currencyGrpcMessage = new CurrencyGrpcMessage();
           _currencyCache.GetList().ForEach(p => currencyGrpcMessage.Reference.Add(_currencyDataConverter.Convert(p)));
            return Task.FromResult(currencyGrpcMessage);

        }

        public override Task<PriceUnitGrpcMessage> GetPriceUnitReference(GetAllRequestMessage request, ServerCallContext context)
        {
            PriceUnitGrpcMessage priceUnitGrpcMessage = new PriceUnitGrpcMessage();
            _unitCache.GetList().ForEach(p => priceUnitGrpcMessage.Reference.Add(_priceUnitDataConverter.Convert(p)));
            return Task.FromResult(priceUnitGrpcMessage);
        }

        

        public override Task<RegionGrpcMessage> GetRegionReference(GetAllRequestMessage request, ServerCallContext context)
        {
            RegionGrpcMessage regionGrpcMessage = new RegionGrpcMessage();
            _regionsCache.GetList().ForEach(p => regionGrpcMessage.Reference.Add(_regionDataConverter.Convert(p)));
            return Task.FromResult(regionGrpcMessage);
        }


        public override Task<ProductGrpcResponse> GetProduct(GetByIdRequestMessage request, ServerCallContext context)
        {
            ProductGrpcResponse productGrpcResponse = new ProductGrpcResponse();
            Option<OfficialProduct> option = _productsCache.GetSingle(request.Id);
            if (option.HasValue) productGrpcResponse.Product = _converter.Convert(option.ValueOr(() => null));
            return Task.FromResult(productGrpcResponse);
        }
    }
}
