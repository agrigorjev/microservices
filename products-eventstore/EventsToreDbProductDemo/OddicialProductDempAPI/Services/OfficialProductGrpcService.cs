

using Grpc.Core;
using MandaraDemo.GrpcDefinitions;
using MandaraDemoDTO;
using OfficialProductDemoAPI.Services.Contracts;
using Optional;
using static MandaraDemo.GrpcDefinitions.ProductAPIService;

namespace OfficialProductDemoAPI.Services
{
    public class OfficialProductGrpcService : ProductAPIServiceBase
    {
        private IDataService<OfficialProduct> _productsCache;
        private OfficialProductConverter _converter=new OfficialProductConverter();
        public OfficialProductGrpcService(IDataService<OfficialProduct> productsCache)
        {
            _productsCache=productsCache;
        }

        public override Task<ProductsGrpcMessage> GetAllProducts(GetAllRequestMessage request, ServerCallContext context)
        {

            ProductsGrpcMessage productsGrpcMessage = new ProductsGrpcMessage();
            _productsCache.GetList().ForEach(p => productsGrpcMessage.Products.Add(_converter.Convert(p)));
            return Task.FromResult(productsGrpcMessage);
        }

        public override Task<ProductGrpcResponse> GetProduct(GetByIdRequestMessage request, ServerCallContext context)
        {
            ProductGrpcResponse productGrpcResponse = new ProductGrpcResponse();
            Option<OfficialProduct> option = _productsCache.GetSingle(request.Id);
            if(option.HasValue) productGrpcResponse.Product =_converter.Convert(option.ValueOr(()=>null));
            return Task.FromResult(productGrpcResponse);
        }
    }
}
