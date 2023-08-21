using Google.Protobuf.WellKnownTypes;
using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService.Data;

namespace Mandara.TradeApiService.DataConverters;

public class CompanyAliasDataConverter : IDataConverter<CompanyAlias, CompanyAliasGrpc>
{
    public CompanyAliasGrpc Convert(CompanyAlias data)
    {
        CompanyAliasGrpc converted = new();
        converted.AliasId = data.AliasId;
        converted.AliasName = data.AliasName;
        converted.CompanyId = data.CompanyId;

        return converted;
    }
}
