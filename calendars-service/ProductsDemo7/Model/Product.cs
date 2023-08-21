using System;
using CustomTypes;
using ProductsDemo.GrpcDefinitions;
#nullable enable
namespace ProductsDemo.Model
{
    public class Product
    {
        public const int NoDailyDiffMonthShift = 0;
        public const int DefaultId = -1;
        public const string? DefaultName = "DefaultProduct";

        public Product(ProductGrpc data)
        {
            ProductId = data.ProductId;
            calendar_id = data.CalendarId;
            holidays_calendar_id = data.HolidaysCalendarId;
            Name = data.Name;
            ProductTypeDb = (short)data.ProductTypeDb;
            PositionFactor = data.PositionFactor;
            PnlFactor = data.PnlFactor;
            CategoryId = data.CategoryId;
            OfficialProductId = data.OfficialProductId;
            ContractSize = data.ContractSize;
            BalmoOnCrudeProductId = data.BalmoOnCrudeProductId;
            BalmoOnComplexProductId = data.BalmoOnComplexProductId;
            ValidFrom = data.ValidFrom.DateTimeNullable();
            ValidTo = data.ValidTo.DateTimeNullable();
            UnderlyingFuturesProductId = data.UnderlyingFuturesProductId;
            UnderlyingFuturesOverrideId = data.UnderlyingFuturesOverrideId;
            ExchangeContractCode = data.ExchangeContractCode;
            FeeExchange = data.FeeExchange;
            FeeNfa = data.FeeNfa;
            FeeCommission = data.FeeCommission;
            FeeClearing = data.FeeClearing;
            TimezoneId = data.TimezoneId;
            RolloffTime = data.RolloffTime.DateTimeNullable();
            UseRolloffSettings = data.UseRolloffSettings;
            FeeConversionFactor = data.FeeConversionFactor;
            FeeCash = data.FeeCash;
            DefinitionLink = data.DefinitionLink;
            BalmoContractCode1 = data.BalmoContractCode1;
            BalmoContractCode2 = data.BalmoContractCode2;
            BalmoContractCode3 = data.BalmoContractCode3;
            BalmoCodeFirstLetter = data.BalmoCodeFirstLetter;
            ExpirationTypeDb = (short?)data.ExpirationTypeDb;
            RollingMethodDb = (short?)data.RollingMethodDb;
            GivenDate = data.GivenDate.DateTimeNullable();
            ExpirationMonth = (short?)data.ExpirationMonth;
            NumberOfDays = (short?)data.NumberOfDays;
            ExchangeId = data.ExchangeId;
            FeeBlockTrade = data.FeeBlockTrade;
            FeePlattsTrade = data.FeePlattsTrade;
            FuturesExpireTime = data.FuturesExpireTime.DateTimeNullable();
            UseExpiryCalendar = data.UseExpiryCalendar;
            IsPhysicallySettledDb = data.IsPhysicallySettledDb;
            IsInternalTransferProductDb = data.IsInternalTransferProductDb;
            PriceConversionFactorDb = data.PriceConversionFactorDb;
            IsTasDb = data.IsTasDb;
            PricingEndTime = data.PricingEndTime.DateTimeNullable();
            TreatTimespreadStripAsLegsDb = data.TreatTimespreadStripAsLegsDb;
            CalculatePnlFromLegs = data.CalculatePnlFromLegs;
            IsMopsDb = data.IsMopsDb;
            IsMmDb = data.IsMmDb;
            CategoryOverrideAt = data.CategoryOverrideAt.DateTimeNullable();
            CategoryOverrideId = data.CategoryOverrideId;
            TasOfficialProductId = data.TasOfficialProductId;
            IsChanged = data.IsChanged;
            MonthlyOfficialProductId = data.MonthlyOfficialProductId;
            DailyDiffMonthShiftDb = data.DailyDiffMonthShiftDb;
            PhysicalCode = data.PhysicalCode;
            UnitId = data.UnitId;
            IsAllowedForManualTradesDb = data.IsAllowedForManualTradesDb;
            Currency1Id = data.Currency1Id;
            Currency2Id = data.Currency2Id;
            FeeClearingCurrencyId = data.FeeClearingCurrencyId;
            FeeCommissionCurrencyId = data.FeeCommissionCurrencyId;
            FeeExchangeCurrencyId = data.FeeExchangeCurrencyId;
            FeeNfaCurrencyId = data.FeeNfaCurrencyId;
            FeeCashCurrencyId = data.FeeCashCurrencyId;
            FeeBlockCurrencyId = data.FeeBlockCurrencyId;
            FeePlattsCurrencyId = data.FeePlattsCurrencyId;
            IsEnabledRiskDecompositionDb = data.IsEnabledRiskDecompositionDb;
            IsCalendarDaySwap = data.IsCalendarDaySwap;
            ContractSizeMultiplierDb = data.ContractSizeMultiplierDb;
            IceEquivalentProductDb = data.IceEquivalentProductDb;
            IceEquivalentUnderlyingProduct = data.IceEquivalentUnderlyingProduct;
        }

        public Product()
        {

        }

        public static readonly Product Default = new Product()
        {
            ProductId = DefaultId,
            Name = DefaultName,

        };
        private string? timezoneId = "GMT Standard Time";

        public bool IsDefault => DefaultId == ProductId &&
                                 DefaultName == Name;

        public int ProductId { get; set; }

        public int calendar_id { get; set; }



        public int? holidays_calendar_id { get; set; }

        public string? Name { get; set; }


        public short ProductTypeDb { get; set; }
        public decimal? PositionFactor { get; set; }

        public decimal? PnlFactor { get; set; }

        public int? CategoryId { get; set; }

        public int OfficialProductId { get; set; }

        public decimal? ContractSize { get; set; }

        public int? BalmoOnCrudeProductId { get; set; }

        public int? BalmoOnComplexProductId { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public int? UnderlyingFuturesProductId { get; set; }

        public int? UnderlyingFuturesOverrideId { get; set; }

        public string? ExchangeContractCode { get; set; }

        public decimal? FeeExchange { get; set; }

        public decimal? FeeNfa { get; set; }

        public decimal? FeeCommission { get; set; }

        public decimal? FeeClearing { get; set; }

        public string? TimezoneId { get => timezoneId; set => timezoneId = value; }
        public DateTime? RolloffTime { get; set; }

        public bool? UseRolloffSettings { get; set; }

        public decimal? FeeConversionFactor { get; set; }

        public decimal? FeeCash { get; set; }

        public string? DefinitionLink { get; set; }

        public string? BalmoContractCode1 { get; set; }

        public string? BalmoContractCode2 { get; set; }

        public string? BalmoContractCode3 { get; set; }

        public string? BalmoCodeFirstLetter { get; set; }

        public short? ExpirationTypeDb { get; set; }

        public short? RollingMethodDb { get; set; }

        public DateTime? GivenDate { get; set; }

        public short? ExpirationMonth { get; set; }

        public short? NumberOfDays { get; set; }

        public int? ExchangeId { get; set; }

        public decimal? FeeBlockTrade { get; set; }
        public decimal? FeePlattsTrade { get; set; }

        public DateTime? FuturesExpireTime { get; set; }

        public bool? UseExpiryCalendar { get; set; }

        public bool? IsPhysicallySettledDb { get; set; }

        public bool? IsInternalTransferProductDb { get; set; }

        public decimal? PriceConversionFactorDb { get; set; }

        public bool? IsTasDb { get; set; }

        public DateTime? PricingEndTime { get; set; }

        public bool? TreatTimespreadStripAsLegsDb { get; set; }

        public bool CalculatePnlFromLegs { get; set; }


        public bool? IsMopsDb { get; set; }

        public bool? IsMmDb { get; set; }


        public DateTime? CategoryOverrideAt { get; set; }


        public int? CategoryOverrideId { get; set; }

        public int? TasOfficialProductId { get; set; }


        public bool IsChanged { get; set; }


        public int? MonthlyOfficialProductId { get; set; }


        public int? DailyDiffMonthShiftDb { get; set; }

        public int DailyDiffMonthShift
        {
            get => DailyDiffMonthShiftDb ?? NoDailyDiffMonthShift;
            set => DailyDiffMonthShiftDb = NoDailyDiffMonthShift == value ? default(int?) : value;
        }

        public string? PhysicalCode { get; set; }


        public int? UnitId { get; set; }


        public bool? IsAllowedForManualTradesDb { get; set; }


        public int? Currency1Id { get; set; }


        public int? Currency2Id { get; set; }


        public int? FeeClearingCurrencyId { get; set; }

        public int? FeeCommissionCurrencyId { get; set; }


        public int? FeeExchangeCurrencyId { get; set; }


        public int? FeeNfaCurrencyId { get; set; }


        public int? FeeCashCurrencyId { get; set; }


        public int? FeeBlockCurrencyId { get; set; }


        public int? FeePlattsCurrencyId { get; set; }


        public bool? IsEnabledRiskDecompositionDb { get; set; }


        public bool IsCalendarDaySwap { get; set; }


        public int? ContractSizeMultiplierDb { get; set; }

        public int? IceEquivalentProductDb { get; set; }


        public int? IceEquivalentUnderlyingProduct { get; set; }



        public override bool Equals(object obj)
        {
            Product? p = obj as Product;
            return ProductId == p?.ProductId;
        }

        public override int GetHashCode()
        {
            return ProductId;
        }


    }
}
