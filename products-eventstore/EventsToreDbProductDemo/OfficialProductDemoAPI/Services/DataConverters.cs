﻿using MandaraDemoDTO;
using OfficialProductDemoAPI.Extensions;
using MandaraDemo.GrpcDefinitions;
using MandaraDemoDTO.Contracts;

namespace OfficialProductDemoAPI.Services
{
    public class OfficialProductConverter : IDataConverter<OfficialProduct, ProductGrpc>
    {

        

        public ProductGrpc? Convert(OfficialProduct data)
        {
            if (data==null) return null;
            else
            {
                return new ProductGrpc()
                {
                    Id = data.Id.ToString(),
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
                    PriceExpirationPeriod = data.price_expiration_period.toProtoTimestamp(),
                    SpreadPriceExpirationPeriod = data.spread_price_expiration_period.toProtoTimestamp(),
                    DeskId = data.desk_id,
                    SettlementProductId = data.SettlementProductId,
                    IsAllowedForManualTradesDb = data.IsAllowedForManualTradesDb,
                    CurrencyGuId = data.CurrencyGuId.ToString(),
                    RegionGuId = data.RegionGuId.ToString(),
                    UnitGuid = data.UnitGuid.ToString(),
                    Status = data.Status.ToString()
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
                    Status = data.Status.ToString()
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
                    Status = data.Status.ToString()
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
                    Status = data.Status.ToString()
                };
            }
        }
    }
}
