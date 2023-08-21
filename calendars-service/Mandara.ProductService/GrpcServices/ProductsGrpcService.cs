

using Grpc.Core;
using Mandara.ProductService.Data.Entities;
using Mandara.ProductService.GrpcDefinitions;
using Mandara.ProductService.Services;
using Optional;
using static Mandara.ProductService.GrpcDefinitions.ProductAPIService;

namespace Mandara.ProductService.GrpcServices;

public class ProductsGrpcService : ProductAPIServiceBase
{
    private readonly IProductStorage _cache;
    private readonly DataConverters.ProductDataConverter productDataConverter = new DataConverters.ProductDataConverter();
    private readonly DataConverters.SecurityDefinitionDataConverter securityDefinitionDataConverter = new DataConverters.SecurityDefinitionDataConverter();


    public ProductsGrpcService(IProductStorage cache)
    {
        _cache = cache;
    }

    public override Task<ProductsGrpcMessage> GetAllProducts(GetAllRequestMessage request, ServerCallContext context)
    {
        ProductsGrpcMessage productsGrpcMessage = new ProductsGrpcMessage();
        _cache.GetProducts().ForEach(p => productsGrpcMessage.Products.Add(productDataConverter.Convert(p)));
        return Task.FromResult(productsGrpcMessage);
    }

    public override Task<SecurtyDefinitionsGrpcMessage> GetAllSecurityDefinitions(GetAllRequestMessage request, ServerCallContext context)
    {
        SecurtyDefinitionsGrpcMessage securtyDefinitionsGrpcMessage = new SecurtyDefinitionsGrpcMessage();
        _cache.GetSecurityDefinitions().ForEach(sd => securtyDefinitionsGrpcMessage.SecurityDefinitions.Add(securityDefinitionDataConverter.Convert(sd)));
        return Task.FromResult(securtyDefinitionsGrpcMessage);
    }


    public override Task<ProductGrpcResponse> GetProduct(GetByIdRequestMessage request, ServerCallContext context)
    {
        ProductGrpcResponse response = new();
        Option<Product> option = _cache.GetProduct(request.Id);
        response.Product = productDataConverter.Convert(option.ValueOr(Product.Default));
        return Task.FromResult(response);
    }

    public override Task<SecurityDefinitionGrpcResponse> GetSecurrityDefinition(GetByIdRequestMessage request, ServerCallContext context)
    {
        SecurityDefinitionGrpcResponse response = new();
        Option<SecurityDefinition> option = _cache.GetSecurityDefinition(request.Id);
        response.SecurityDefinition = securityDefinitionDataConverter.Convert(option.ValueOr(new SecurityDefinition()));
        return Task.FromResult(response);
    }



    //public override Task<StockCalendarsGrpcMessage> GetAllStockCalendars(GetAllRequestMessage request, ServerCallContext context)
    //{
    //    StockCalendarsGrpcMessage responseMessage = new();
    //    _cache.GetStockCalendars().ForEach(calendar => responseMessage.StockCalendars.Add(stockCalendarConverter.Convert(calendar)));
    //    return Task.FromResult(responseMessage);
    //}

    //public override Task<StockCalendarGrpcResponse> GetStockCalendar(GetByIdRequestMessage request, ServerCallContext context)
    //{
    //    StockCalendarGrpcResponse responseMessage = new();

    //    var calendar = _cache.GetStockCalendar(request.Id);

    //    responseMessage.StockCalendarData = stockCalendarConverter.Convert(calendar.ValueOr(StockCalendar.Default));

    //    return Task.FromResult(responseMessage);
    //}

    //public override Task<HolidaysGrpcMessage> GetAllHolidays(GetAllRequestMessage request, ServerCallContext context)
    //{
    //    HolidaysGrpcMessage responseMessage = new();
    //    _cache.GetCalendarHolidays().ForEach(calendar => responseMessage.Holidays.Add(calendarHolidayConverter.Convert(calendar)));
    //    return Task.FromResult(responseMessage);
    //}

    //public override Task<HolidaysGrpcMessage> GetHolidays(GetByIdRequestMessage request, ServerCallContext context)
    //{
    //    HolidaysGrpcMessage responseMessage = new();
    //    _cache.GetCalendarHolidays(request.Id).ForEach(calendar => responseMessage.Holidays.Add(calendarHolidayConverter.Convert(calendar)));
    //    return Task.FromResult(responseMessage);
    //}

    //public override Task<ExpiryDatesGrpcMessage> GetAllExpiryDates(GetAllRequestMessage request, ServerCallContext context)
    //{
    //    ExpiryDatesGrpcMessage responseMessage = new();
    //    _cache.GetCalendarExpiryDates().ForEach(calendar => responseMessage.ExpiryDates.Add(calendarExpiryDateConverter.Convert(calendar)));
    //    return Task.FromResult(responseMessage);
    //}

    //public override Task<ExpiryDatesGrpcMessage> GetExpiryDates(GetByIdRequestMessage request, ServerCallContext context)
    //{
    //    ExpiryDatesGrpcMessage responseMessage = new();
    //    _cache.GetCalendarExpiryDates(request.Id).ForEach(calendar => responseMessage.ExpiryDates.Add(calendarExpiryDateConverter.Convert(calendar)));
    //    return Task.FromResult(responseMessage);
    //}
}