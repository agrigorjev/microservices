using Google.Protobuf.WellKnownTypes;
using Mandara.ProductConfiguration.Contracts;
using Mandara.ProductConfiguration.Data;
using Mandara.ProductConfiguration.GrpcDefinitions;

namespace Mandara.ProductConfiguration.DataConverters;

public class ProductMappingsConverter : IDataConverter<OfficialProduct, ProductMappingsGrpc>
{
    public ProductMappingsGrpc Convert(OfficialProduct data)
    {

        ProductMappingsGrpc convertedDate = new()
        {
            Priority = data.Priority,
            Abbreviation = data.Abbreviation,
            ConversionFactor = data.ConversionFactor,
            CategoryId = data.CategoryId,
            DisplayName = data.DisplayName,
            HolidayCalendarId = data.HolidayCalendarId,
            OfficialName = data.Name,
            OfficialProductId = data.OfficialProductId,
            OldPriceMappingColumn = data.OldPriceMappingColumn,
            PriceMappingColumn = data.PriceMappingColumn,
            IsDaily = data.IsDaily,

        };
        convertedDate.DisplayName = data.DisplayName;
        return convertedDate;
    }
}


public class CategoryConverter : IDataConverter<ProductCategory, CategoryGrpc>
{
    public CategoryGrpc Convert(ProductCategory data)
    {

        CategoryGrpc convertedDate = new()
        {
            Daily = data.Daily,
            ExpiryCalendarId = data.ExpiryCalendarId,
            Id = data.CategoryId,
            PnlFactor = data.PnlFactor,
            SelectMonths = data.SelectMonths,
            Value = data.Name

        };
        return convertedDate;
    }
}

