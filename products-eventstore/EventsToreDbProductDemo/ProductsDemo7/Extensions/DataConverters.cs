using MandaraDemoDTO;
using MandaraDemo.GrpcDefinitions;
using Region = MandaraDemoDTO.Region;
using CustomTypes;
using ProductsDemo7.Extensions;

namespace ProductsDemo7.Extensions
{
    public class OfficialProductConverter : IDataConverter< ProductGrpc,OfficialProduct>
    {

        public OfficialProduct? Convert(ProductGrpc data)
        {
            if (data == null) return null;
            else
            {
                return new OfficialProduct()
                {
                    Id = Guid.Parse(data.Id),
                    Name = data.Name,
                    DisplayName = data.DisplayName,
                    MappingColumn = data.MappingColumn,
                    ApplySignVerification = data.ApplySignVerification,
                    ApplyFractionPartVerification = data.ApplyFractionPartVerification,
                    Epsilon = data.Epsilon,
                    ApplyMissingPointVerification = data.ApplyMissingPointVerification,
                    MissingPointAccuracy = data.MissingPointAccuracy,
                    PublishToUms = data.PublishToUms,
                    NameOnUms = data.NameOnUms,
                    UnitToBarrelConversionFactor = data.UnitToBarrelConversionFactor,
                    price_expiration_period = data.PriceExpirationPeriod.DateTimeNullable(),
                    spread_price_expiration_period = data.SpreadPriceExpirationPeriod.DateTimeNullable(),
                    desk_id = data.DeskId,
                    SettlementProductId = data.SettlementProductId,
                    IsAllowedForManualTradesDb = data.IsAllowedForManualTradesDb,
                    CurrencyGuId = Guid.Parse(data.CurrencyGuId),
                    RegionGuId = data.RegionGuId.toGuidNullable(),
                    UnitGuid = Guid.Parse(data.UnitGuid)

                };
            }
        }
    }

    public class CurrencyDataConverter : IDataConverter< CurrencyGrpc, Currency>
    {
        public Currency? Convert(CurrencyGrpc? data)
        {
            if (data == null) return null;
            else
            {
                return new Currency()
                {
                    Id = Guid.Parse(data.Id),
                    IsoName = data.IsoName,
                };
            }
        }
    }

    public class PriceUnitDataConverter : IDataConverter<PriceUnitGrpc,Unit>
    {
        public Unit? Convert(PriceUnitGrpc? data)
        {
            if (data == null) return null;
            else
            {
                return new Unit()
                {
                    Id = Guid.Parse(data.Id),
                    AllowOnlyMonthlyContractSize = data.AllowOnlyMonthlyContractSize,
                    DefaultPositionFactor = data.DefaultPositionFactor,
                    Name = data.Name,

                };
            }
        }
    }
    public class RegionDataConverter : IDataConverter<RegiontGrpc,Region>
    {
        public Region? Convert(RegiontGrpc? data)
        {
            if (data == null) return null;
            else
            {
                return new Region()
                {
                    Id = Guid.Parse(data.Id),
                    Name = data.Name,
                };
            }
        }
    }
}

