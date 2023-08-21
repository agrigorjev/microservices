using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Mandara.TradeApiService.Data;

[Table("products")]
public class Product
{
    public const int DefaultId = -1;
    public const string DefaultName = "DefaultProduct";

    public static readonly Product Default = new Product()
    {
        ProductId = DefaultId,
        //Category = ProductCategory.Default,
        //OfficialProduct = OfficialProduct.Default,
        //Exchange = Exchange.Default,
        //Currency1 = Currency.Default,
        //Currency2 = Currency.Default,
        //FeeBlockCurrency = Currency.Default,
        //FeeCashCurrency = Currency.Default,
        //FeeClearingCurrency = Currency.Default,
        //FeeCommissionCurrency = Currency.Default,
        //FeeExchangeCurrency = Currency.Default,
        //FeeNfaCurrency = Currency.Default,
        //FeePlattsCurrency = Currency.Default,
    };

    [NotMapped]
    public bool IsDefault => DefaultId == ProductId;


    [Column("product_id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }

    [Column("calendar_id")]
    public int calendar_id { get; set; }

    [Column("holidays_calendar_id")]
    public int? holidays_calendar_id { get; set; }

    [Column("product_name")]
    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Column("product_type")]
    public short ProductTypeDb { get; set; }

    [Column("position_convertion")]
    [Precision(18, 8)]
    public decimal? PositionFactor { get; set; }

    [Column("pnl_conversion")]
    [Precision(18, 8)]
    public decimal? PnlFactor { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("official_product_id")]
    public int OfficialProductId { get; set; }

    [Column("contract_size")]
    [Precision(18, 2)]
    public decimal ContractSize { get; set; }

    [Column("balmo_on_crude_product_id")]
    public int? BalmoOnCrudeProductId { get; set; }

    [Column("balmo_on_complex_product_id")]
    public int? BalmoOnComplexProductId { get; set; }

    [Column("valid_from")]
    public DateTime? ValidFrom { get; set; }

    [Column("valid_to")]
    public DateTime? ValidTo { get; set; }

    [Column("underlying_futures_product_id")]
    public int? UnderlyingFuturesProductId { get; set; }

    [Column("underlying_futures_override_id")]
    public int? UnderlyingFuturesOverrideId { get; set; }

    [Column("exchange_contract_code")]
    [StringLength(10)]
    public string? ExchangeContractCode { get; set; }

    [Column("fee_exchange")]
    [Precision(18, 4)]
    public decimal? FeeExchange { get; set; }

    [Column("fee_nfa")]
    [Precision(18, 4)]
    public decimal? FeeNfa { get; set; }

    [Column("fee_commission")]
    [Precision(18, 4)]
    public decimal? FeeCommission { get; set; }

    [Column("fee_clearing")]
    [Precision(18, 4)]
    public decimal? FeeClearing { get; set; }

    [Column("timezone_id")]
    [StringLength(255)]
    public string? TimezoneId { get; set; } = "GMT Standard Time";

    [Column("rolloff_time")]
    public DateTime? RolloffTime { get; set; }

    [Column("use_rolloff_settings")]
    public bool? UseRolloffSettings { get; set; }

    [Column("fee_conversion_factor")]
    [Precision(18, 4)]
    public decimal? FeeConversionFactor { get; set; }

    [Column("fee_cash")]
    [Precision(18, 4)]
    public decimal? FeeCash { get; set; }

    [Column("definition_link")]
    [StringLength(255)]
    public string? DefinitionLink { get; set; }

    [Column("balmo_contract_code_1")]
    [StringLength(2)]
    public string? BalmoContractCode1 { get; set; }

    [Column("balmo_contract_code_2")]
    [StringLength(2)]
    public string? BalmoContractCode2 { get; set; }

    [Column("balmo_contract_code_3")]
    [StringLength(2)]
    public string? BalmoContractCode3 { get; set; }

    [Column("balmo_contract_code_1_f_letter")]
    [StringLength(1)]
    public string? BalmoCodeFirstLetter { get; set; }

    [Column("expiration_type_db")]
    public short? ExpirationTypeDb { get; set; }

    [Column("rolling_method_db")]
    public short? RollingMethodDb { get; set; }

    [Column("given_date")]
    public DateTime? GivenDate { get; set; }

    [Column("expiration_month")]
    public short? ExpirationMonth { get; set; }

    [Column("number_of_days")]
    public short? NumberOfDays { get; set; }

    [Column("exchange_id")]
    public int? ExchangeId { get; set; }

    [Column("fee_block_trade")]
    [Precision(18, 4)]
    public decimal? FeeBlockTrade { get; set; }

    [Column("fee_platts_trade")]
    [Precision(18, 4)]
    public decimal? FeePlattsTrade { get; set; }

    [Column("expire_time")]
    public DateTime? FuturesExpireTime { get; set; }

    [Column("use_expiry_calendar")]
    public bool? UseExpiryCalendar { get; set; }

    [Column("is_physically_settled")]
    public bool? IsPhysicallySettledDb { get; set; }

    [Column("is_internal_transfer_product ")]
    public bool? IsInternalTransferProductDb { get; set; }

    [Column("price_conversion_factor")]
    [Precision(18, 8)]
    public decimal? PriceConversionFactorDb { get; set; }

    [Column("is_tas")]
    public bool? IsTasDb { get; set; }

    [Column("pricing_end_time")]
    public DateTime? PricingEndTime { get; set; }

    [Column("treat_timespread_strips_as_legs")]
    public bool? TreatTimespreadStripAsLegsDb { get; set; }

    [Column("calculate_pnl_from_legs")]
    public bool CalculatePnlFromLegs { get; set; }

    [Column("is_mops")]
    public bool? IsMopsDb { get; set; }

    [Column("is_mm")]
    public bool? IsMmDb { get; set; }

    [Column("category_override_at")]
    public DateTime? CategoryOverrideAt { get; set; }

    [Column("category_override_id")]
    public int? CategoryOverrideId { get; set; }

    [Column("tas_official_product_id")]
    public int? TasOfficialProductId { get; set; }

    [Column("is_changed")]
    public bool IsChanged { get; set; }

    [Column("monthly_official_product_id")]
    public int? MonthlyOfficialProductId { get; set; }

    [Column("daily_diff_month_shift")]
    public int? DailyDiffMonthShiftDb { get; set; }


    [Column("physical_code")]
    public string? PhysicalCode { get; set; }

    [Column("unit_id")]
    public int? UnitId { get; set; }

    [Column("is_allowed_for_manual_trades")]
    public bool? IsAllowedForManualTradesDb { get; set; }

    [Column("currency1_id")]
    public int? Currency1Id { get; set; }

    [Column("currency2_id")]
    public int? Currency2Id { get; set; }

    [Column("fee_clearing_currency")]
    public int? FeeClearingCurrencyId { get; set; }

    [Column("fee_commission_currency")]
    public int? FeeCommissionCurrencyId { get; set; }

    [Column("fee_exchange_currency")]
    public int? FeeExchangeCurrencyId { get; set; }

    [Column("fee_nfa_currency")]
    public int? FeeNfaCurrencyId { get; set; }

    [Column("fee_cash_currency")]
    public int? FeeCashCurrencyId { get; set; }

    [Column("fee_block_trade_currency")]
    public int? FeeBlockCurrencyId { get; set; }

    [Column("fee_platts_trade_currency")]
    public int? FeePlattsCurrencyId { get; set; }

    [Column("is_enabled_risk_decomposition")]
    public bool? IsEnabledRiskDecompositionDb { get; set; }

    [Column("is_calendar_day_swap")]
    public bool IsCalendarDaySwap { get; set; }

    [Column("contract_size_multiplier")]
    public int? ContractSizeMultiplierDb { get; set; }

    [Column("ice_equivalent_product")]
    public int? IceEquivalentProductDb { get; set; }

    [Column("ice_equivalent_underlying_product")]
    public int? IceEquivalentUnderlyingProduct { get; set; }

    [NotMapped]
    public ProductType Type
    {
        get => (ProductType)ProductTypeDb;
        set => ProductTypeDb = (short)value;
    }

    private OfficialProduct _officialProduct;

    [ForeignKey("OfficialProductId")]
    public virtual OfficialProduct OfficialProduct
    {
        get => _officialProduct;
        set
        {
            _officialProduct = value;
            OfficialProductId = _officialProduct?.OfficialProductId ?? 0;
        }
    }

    private Exchange _exchange;

    [ForeignKey("ExchangeId")]
    public virtual Exchange Exchange
    {
        get => _exchange;
        set
        {
            _exchange = value ?? throw new ArgumentNullException(nameof(value), "Exchange could not be null");
            ExchangeId = _exchange.ExchangeId;
        }
    }

    private ComplexProduct _complexProduct;

    [ForeignKey("ProductId")]
    public virtual ComplexProduct ComplexProduct
    {
        get => _complexProduct;
        set
        {
            _complexProduct = value;

            if (ProductId == default(int))
            {
                ProductId = _complexProduct?.ProductId ?? 0;
            }
        }
    }

    private ProductCategory _category;

    [ForeignKey("CategoryId")]
    public virtual ProductCategory Category
    {
        get => _category;
        set
        {
            _category = value;
            CategoryId = _category?.CategoryId;
        }
    }

    private Currency _currency1;
    private Currency _currency2;

    [ForeignKey("Currency1Id")]
    public virtual Currency Currency1
    {
        get => _currency1;
        set
        {
            _currency1 = value;
            Currency1Id = _currency1?.CurrencyId;
        }
    }

    [ForeignKey("Currency2Id")]
    public virtual Currency Currency2
    {
        get => _currency2;
        set
        {
            _currency2 = value;
            Currency2Id = _currency2?.CurrencyId;
        }
    }

    private Unit _unit;

    [ForeignKey("UnitId")]
    public virtual Unit Unit
    {
        get => _unit;
        set
        {
            _unit = value;
            UnitId = _unit?.UnitId ?? 0;
        }
    }


    [NotMapped]
    public bool IsAllowedForManualTrades
    {
        get => IsAllowedForManualTradesDb ?? true;
        set => IsAllowedForManualTradesDb = value;
    }

    [NotMapped]
    private bool IsTas
    {
        get => IsTasDb ?? false;
        set => IsTasDb = value;
    }

    [NotMapped]
    public TasType TasType
    {
        get
        {
            if (IsTas)
            {
                return TasType.Tas;
            }

            if (IsMops)
            {
                return TasType.Mops;
            }

            if (IsMm)
            {
                return TasType.Mm;
            }

            return TasType.NotTas;
        }

        set
        {
            IsTas = IsMops = IsMm = false;

            switch (value)
            {
                case TasType.Tas:
                    {
                        IsTas = true;
                    }
                    break;
                case TasType.Mops:
                    {
                        IsMops = true;
                    }
                    break;
                case TasType.Mm:
                    {
                        IsMm = true;
                    }
                    break;
            }
        }
    }

    [NotMapped]
    private bool IsMops
    {
        get => IsMopsDb ?? false;
        set => IsMopsDb = value;
    }

    [NotMapped]
    private bool IsMm
    {
        get => IsMmDb ?? false;
        set => IsMmDb = value;
    }

    [NotMapped]
    public bool IsInternalTransferProduct
    {
        get => IsInternalTransferProductDb ?? false;
        set => IsInternalTransferProductDb = value;
    }

    [NotMapped]
    public bool IsProductDaily => Type.IsDaily();
}
