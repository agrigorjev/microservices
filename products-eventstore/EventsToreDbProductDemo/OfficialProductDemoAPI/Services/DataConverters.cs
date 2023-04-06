using MandaraDemoDTO;
using OfficialProductDemoAPI.Extensions;
using MandaraDemo.GrpcDefinitions;

namespace OfficialProductDemoAPI.Services
{
    public class OfficialProductConverter : IDataConverter<OfficialProduct, ProductGrpc>
    {

        private PriceUnitDataConverter _priceUnitDataConverter=new PriceUnitDataConverter();
        private CurrencyDataConverter  _currencyDataConverter = new CurrencyDataConverter();
        private RegionDataConverter _regionDataConverter = new RegionDataConverter();

        public ProductGrpc? Convert(OfficialProduct data)
        {
            if (data==null) return null;
            else
            {
                return new ProductGrpc()
                {
                   Id = data.Id.ToString(),
                   Name=data.Name,
                   DisplayName = data.DisplayName,
                   MappingColumn = data.MappingColumn,
                   ApplySignVerification = data.ApplySignVerification,
                   ApplyFractionPartVerification = data.ApplyFractionPartVerification,
                   Epsilon = data.Epsilon,
                   ApplyMissingPointVerification = data.ApplyMissingPointVerification,
                   MissingPointAccuracy = data.MissingPointAccuracy,
                   PublishToUms = data.PublishToUms,
                   NameOnUms = data.NameOnUms,
                   UnitToBarrelConversionFactor=data.UnitToBarrelConversionFactor,
                   PriceExpirationPeriod = data.price_expiration_period.toProtoTimestamp(),
                   SpreadPriceExpirationPeriod= data.spread_price_expiration_period.toProtoTimestamp(),
                   DeskId = data.desk_id,
                   SettlementProductId = data.SettlementProductId,
                   IsAllowedForManualTradesDb = data.IsAllowedForManualTradesDb,
                   CurrencyGuId = data.CurrencyGuId.ToString(),
                   RegionGuId = data.RegionGuId.ToString(),
                   UnitGuid = data.UnitGuid.ToString(),
                   Currency=_currencyDataConverter.Convert(data.Currency),
                    PriceUnit=_priceUnitDataConverter.Convert(data.PriceUnit),
                    Region=_regionDataConverter.Convert(data.Region)

                };
            }
        }
    }

    public class CurrencyDataConverter : IDataConverter<Currency, CurrencyGrpc>
    {
        public CurrencyGrpc? Convert(Currency? data)
        {
            if (data==null) return null;
            else
            {
                return new CurrencyGrpc()
                {
                    Id=data.Id.ToString(),
                    IsoName=data.IsoName,
                };
            }
        }
    }

    public class PriceUnitDataConverter : IDataConverter<Unit, PriceUnitGrpc>
    {
        public PriceUnitGrpc? Convert(Unit? data)
        {
            if (data == null) return null;
            else
            {
                return new PriceUnitGrpc()
                {
                    Id = data.Id.ToString(),
                   AllowOnlyMonthlyContractSize=data.AllowOnlyMonthlyContractSize,
                   DefaultPositionFactor=data.DefaultPositionFactor,
                   Name=data.Name,

                };
            }
        }
    }
    public class RegionDataConverter : IDataConverter<Region, RegiontGrpc>
    {
        public RegiontGrpc? Convert(Region? data)
        {
            if (data == null) return null;
            else
            {
                return new RegiontGrpc()
                {
                    Id = data.Id.ToString(),
                    Name = data.Name,
                };
            }
        }
    }
}
