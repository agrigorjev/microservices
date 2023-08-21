using Google.Protobuf.WellKnownTypes;
using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService.Contracts;
using Mandara.TradeApiService.Data;
using Mandara.TradeApiService.GrpcDefinitions;

namespace Mandara.TradeApiService.DataConverters
{
    public class ProductDataConverter : IDataConverter<Product, ProductGrpc>
    {
        public ProductGrpc? Convert(Product data)
        {
            if (data.IsDefault) return null;
            else
            {
                return new ProductGrpc()
                {
                    ProductId = data.ProductId,
                    CalendarId = data.calendar_id,
                    HolidaysCalendarId = data.holidays_calendar_id,
                    Name = data.Name,
                    ProductTypeDb = data.ProductTypeDb,
                    PositionFactor = data.PositionFactor,
                    PnlFactor = data.PnlFactor,
                    CategoryId = data.CategoryId,
                    OfficialProductId = data.OfficialProductId,
                    ContractSize = data.ContractSize,
                    BalmoOnCrudeProductId = data.BalmoOnCrudeProductId,
                    BalmoOnComplexProductId = data.BalmoOnComplexProductId,
                    ValidFrom = data.ValidFrom.toProtoTimestamp(),
                    ValidTo = data.ValidTo.toProtoTimestamp(),
                    UnderlyingFuturesProductId = data.UnderlyingFuturesProductId,
                    UnderlyingFuturesOverrideId = data.UnderlyingFuturesOverrideId,
                    ExchangeContractCode = data.ExchangeContractCode,
                    FeeExchange = data.FeeExchange,
                    FeeNfa = data.FeeNfa,
                    FeeCommission = data.FeeCommission,
                    FeeClearing = data.FeeClearing,
                    TimezoneId = data.TimezoneId,
                    RolloffTime = data.RolloffTime.toProtoTimestamp(),
                    UseRolloffSettings = data.UseRolloffSettings,
                    FeeConversionFactor = data.FeeConversionFactor,
                    FeeCash = data.FeeCash,
                    DefinitionLink = data.DefinitionLink,
                    BalmoContractCode1 = data.BalmoContractCode1,
                    BalmoContractCode2 = data.BalmoContractCode2,
                    BalmoContractCode3 = data.BalmoContractCode3,
                    BalmoCodeFirstLetter = data.BalmoCodeFirstLetter,
                    ExpirationTypeDb = data.ExpirationTypeDb,
                    RollingMethodDb = data.RollingMethodDb,
                    GivenDate = data.GivenDate.toProtoTimestamp(),
                    ExpirationMonth = data.ExpirationMonth,
                    NumberOfDays = data.NumberOfDays,
                    ExchangeId = data.ExchangeId,
                    FeeBlockTrade = data.FeeBlockTrade,
                    FeePlattsTrade = data.FeePlattsTrade,
                    FuturesExpireTime = data.FuturesExpireTime.toProtoTimestamp(),
                    UseExpiryCalendar = data.UseExpiryCalendar,
                    IsPhysicallySettledDb = data.IsPhysicallySettledDb,
                    IsInternalTransferProductDb = data.IsInternalTransferProductDb,
                    PriceConversionFactorDb = data.PriceConversionFactorDb,
                    IsTasDb = data.IsTasDb,
                    PricingEndTime = data.PricingEndTime.toProtoTimestamp(),
                    TreatTimespreadStripAsLegsDb = data.TreatTimespreadStripAsLegsDb,
                    CalculatePnlFromLegs = data.CalculatePnlFromLegs,
                    IsMopsDb = data.IsMopsDb,
                    IsMmDb = data.IsMmDb,
                    CategoryOverrideAt = data.CategoryOverrideAt.toProtoTimestamp(),
                    CategoryOverrideId = data.CategoryOverrideId,
                    TasOfficialProductId = data.TasOfficialProductId,
                    IsChanged = data.IsChanged,
                    MonthlyOfficialProductId = data.MonthlyOfficialProductId,
                    DailyDiffMonthShiftDb = data.DailyDiffMonthShiftDb,
                    PhysicalCode = data.PhysicalCode,
                    UnitId = data.UnitId,
                    IsAllowedForManualTradesDb = data.IsAllowedForManualTradesDb,
                    Currency1Id = data.Currency1Id,
                    Currency2Id = data.Currency2Id,
                    FeeClearingCurrencyId = data.FeeClearingCurrencyId,
                    FeeCommissionCurrencyId = data.FeeCommissionCurrencyId,
                    FeeExchangeCurrencyId = data.FeeExchangeCurrencyId,
                    FeeNfaCurrencyId = data.FeeNfaCurrencyId,
                    FeeCashCurrencyId = data.FeeCashCurrencyId,
                    FeeBlockCurrencyId = data.FeeBlockCurrencyId,
                    FeePlattsCurrencyId = data.FeePlattsCurrencyId,
                    IsEnabledRiskDecompositionDb = data.IsEnabledRiskDecompositionDb,
                    IsCalendarDaySwap = data.IsCalendarDaySwap,
                    ContractSizeMultiplierDb = data.ContractSizeMultiplierDb,
                    IceEquivalentProductDb = data.IceEquivalentProductDb,
                    IceEquivalentUnderlyingProduct = data.IceEquivalentUnderlyingProduct,
                };
            }
        }
    }

    public class OfficialProductDataConverter : IDataConverter<OfficialProduct, OfficalProductGrpc>
    {
        public OfficalProductGrpc Convert(OfficialProduct data)
        {
            return new OfficalProductGrpc()
            {
                OfficialProductId = data.OfficialProductId,
                OfficialName = data.Name,
                DisplayName = data.DisplayName,
                MappingColumn = data.PriceMappingColumn,
            };
        }
    }

}
