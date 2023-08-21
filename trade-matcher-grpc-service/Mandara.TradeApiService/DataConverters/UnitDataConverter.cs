using Google.Protobuf.WellKnownTypes;
using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService.Data;

namespace Mandara.TradeApiService.DataConverters;

public class UnitDataConverter : IDataConverter<Unit, UnitGrpc>
{
    public UnitGrpc Convert(Unit data)
    {
        UnitGrpc converted = new();
        converted.UnitId = data.UnitId;
        converted.OnlyMonthContractSize = data.AllowOnlyMonthlyContractSize;
        converted.Name = data.Name;
        converted.DefaultPositionFactor = data.DefaultPositionFactor;
        return converted;
    }
}
