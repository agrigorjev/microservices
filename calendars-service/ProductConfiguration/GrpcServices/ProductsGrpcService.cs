

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mandara.ProductConfiguration.Contracts;
using Mandara.ProductConfiguration.DataConverters;
using Mandara.ProductConfiguration.GrpcDefinitions;
using Optional;
using Optional.Unsafe;
using static Mandara.ProductConfiguration.GrpcDefinitions.ProductConfigurationService;

namespace Mandara.ProductConfiguration.GrpcServices;

public class ProductsGrpcService: ProductConfigurationServiceBase
{
    private readonly IDataStorage _cache;

    private readonly CategoryConverter _categoryConverter = new();
    private readonly ProductMappingsConverter _productMappingsConverter = new();


    public ProductsGrpcService(IDataStorage cache)
    {
        _cache = cache;
    }

    public override Task<CategoryGrpcResponse> GeCategoryById(GetByIdRequestMessage request, ServerCallContext context)
    {
        CategoryGrpcResponse categoryGrpcResponse = new();
        var proposed = _cache.GetCategories(request.Id);
        if (proposed.HasValue)
        {
            categoryGrpcResponse.Category = _categoryConverter.Convert(proposed.ValueOrDefault());
        }
        return Task.FromResult(categoryGrpcResponse);
    }

    public override Task<CategoryGrpcMessage> GetAllCategories(Empty request, ServerCallContext context)
    {
        CategoryGrpcMessage categoryGrpcMessage = new();
        categoryGrpcMessage.Categories.AddRange(_cache.GetCategories().Select(c => _categoryConverter.Convert(c)));
        return Task.FromResult(categoryGrpcMessage);
    }

    public override Task<ProductMappingsGrpcMessage> GetAllProductMappings(Empty request, ServerCallContext context)
    {
        ProductMappingsGrpcMessage productMappingsGrpcMessage = new();
        productMappingsGrpcMessage.ProductMappings.AddRange(_cache.GetMappings().Select(c => _productMappingsConverter.Convert(c)));
        return Task.FromResult(productMappingsGrpcMessage);
    }

    public override Task<ProductMappingsGrpcResponse> GetProductMappingById(GetByIdRequestMessage request, ServerCallContext context)
    {
        ProductMappingsGrpcResponse productMappingsGrpcResponse = new();
        var proposed=_cache.GetMappings(request.Id);
        if (proposed.HasValue)
        {
            productMappingsGrpcResponse.ProductMappings =_productMappingsConverter.Convert(proposed.ValueOrDefault());
        }
        return Task.FromResult(productMappingsGrpcResponse);
    }

}