using System.Collections.ObjectModel;
using AutoMapper;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities.Enums;
using Mandara.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Mandara.Entities
{
    [Table("products")]
    public partial class Product : INewable
    {
        [IgnoreMap]
        [NotMapped]
        public const int NoDailyDiffMonthShift = 0;
        public const int DefaultId = -1;
        public const string DefaultName = "DefaultProduct";

        public Product()
        {
            ComplexParentProducts1 = new HashSet<ComplexProduct>();
            ComplexParentProducts2 = new HashSet<ComplexProduct>();
            ComplexProductBalmos = new HashSet<Product>();
            CrudeSwapBalmos = new HashSet<Product>();
            Aliases = new HashSet<ProductAlias>();
            InitialiseSecurityDefinitionsReference();
            product_categories = new HashSet<ProductCategory>();
            product_categories1 = new HashSet<ProductCategory>();
            CompaniesBrokerages = new HashSet<ProductBrokerage>();
            product_categories1_1 = new HashSet<ProductCategory>();
            IceProductsMappings = new HashSet<IceProductMapping>();
            ABNMappings = new HashSet<ABNMappings>();
            trade_scenarios = new HashSet<TradeScenario>();
            GmiBalmoCodes = new HashSet<GmiBalmoCode>();
            IceBalmoMappings = new HashSet<IceBalmoMapping>();
            adm_alerts = new HashSet<AdministrativeAlert>();
            product_categories3 = new HashSet<ProductCategory>();
            swap_cross_per_product = new HashSet<SwapCrossPerProduct>();
            swap_cross_per_product1 = new HashSet<SwapCrossPerProduct>();
        }

        public static readonly Product Default = new Product()
        {
            ProductId = DefaultId,
            Name = DefaultName,
            Category = ProductCategory.Default,
            OfficialProduct = OfficialProduct.Default,
            Exchange = Exchange.Default,
            Currency1 = Currency.Default,
            Currency2 = Currency.Default,
            FeeBlockCurrency = Currency.Default,
            FeeCashCurrency = Currency.Default,
            FeeClearingCurrency = Currency.Default,
            FeeCommissionCurrency = Currency.Default,
            FeeExchangeCurrency = Currency.Default,
            FeeNfaCurrency = Currency.Default,
            FeePlattsCurrency = Currency.Default,
        };

        [NotMapped]
        public bool IsDefault => DefaultId == ProductId &&
                                 DefaultName == Name &&
                                 ProductCategory.Default.Equals(Category) &&
                                 OfficialProduct.Default.Equals(OfficialProduct) &&
                                 Currency.Default.Equals(Currency1) &&
                                 Currency.Default.Equals(Currency2);

        private void InitialiseSecurityDefinitionsReference()
        {
            security_definitions = new HashSet<SecurityDefinition>();
        }

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
        public decimal? PositionFactor { get; set; }

        [Column("pnl_conversion")]
        public decimal? PnlFactor { get; set; }

        [Column("category_id")]
        public int? CategoryId { get; set; }

        [Column("official_product_id")]
        public int OfficialProductId { get; set; }

        [Column("contract_size")]
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
        public string ExchangeContractCode { get; set; }

        [Column("fee_exchange")]
        public decimal? FeeExchange { get; set; }

        [Column("fee_nfa")]
        public decimal? FeeNfa { get; set; }

        [Column("fee_commission")]
        public decimal? FeeCommission { get; set; }

        [Column("fee_clearing")]
        public decimal? FeeClearing { get; set; }

        [Column("timezone_id")]
        [StringLength(255)]
        public string TimezoneId { get; set; } = "GMT Standard Time";

        [Column("rolloff_time")]
        public DateTime? RolloffTime { get; set; }

        [Column("use_rolloff_settings")]
        public bool? UseRolloffSettings { get; set; }

        [NotMapped]
        public bool UsesRollOffSettings
        {
            get => UseRolloffSettings.HasValue && UseRolloffSettings.Value;
            set => UseRolloffSettings = value;
        }

        [Column("fee_conversion_factor")]
        public decimal? FeeConversionFactor { get; set; }

        [Column("fee_cash")]
        public decimal? FeeCash { get; set; }

        [Column("definition_link")]
        [StringLength(255)]
        public string DefinitionLink { get; set; }

        [Column("balmo_contract_code_1")]
        [StringLength(2)]
        public string BalmoContractCode1 { get; set; }

        [Column("balmo_contract_code_2")]
        [StringLength(2)]
        public string BalmoContractCode2 { get; set; }

        [Column("balmo_contract_code_3")]
        [StringLength(2)]
        public string BalmoContractCode3 { get; set; }

        [Column("balmo_contract_code_1_f_letter")]
        [StringLength(1)]
        public string BalmoCodeFirstLetter { get; set; }

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
        public int ExchangeId { get; set; }

        [Column("fee_block_trade")]
        public decimal? FeeBlockTrade { get; set; }

        [Column("fee_platts_trade")]
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

        [IgnoreMap]
        [NotMapped]
        public int DailyDiffMonthShift
        {
            get => DailyDiffMonthShiftDb ?? NoDailyDiffMonthShift;
            set => DailyDiffMonthShiftDb = NoDailyDiffMonthShift == value ? default(int?) : value;
        }

        [Column("physical_code")]
        public string PhysicalCode { get; set; }

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
        public ContractSizeMultiplier ContractSizeMultiplier
        {
            get => ContractSizeMultiplierDb.HasValue
                    ? (ContractSizeMultiplier)ContractSizeMultiplierDb
                    : ContractSizeMultiplier.Monthly;

            set
            {
                int intValue = (int)value;

                if (Enum.IsDefined(typeof(ContractSizeMultiplier), intValue))
                {
                    ContractSizeMultiplierDb = intValue;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"{value} is not a valid value for ContractSizeMutlipler");
                }
            }
        }

        [NotMapped]
        public bool IsAllowedForManualTrades
        {
            get => IsAllowedForManualTradesDb ?? true;
            set => IsAllowedForManualTradesDb = value;
        }

        public const int NoCalendar = 0;

        [ForeignKey("CalendarId")]
        public virtual StockCalendar ExpiryCalendar
        {
            get => _expiryCalendar;
            set
            {
                _expiryCalendar = value;
                calendar_id = _expiryCalendar?.CalendarId ?? NoCalendar;
            }
        }

        [ForeignKey("holidays_calendar_id")]
        public virtual StockCalendar HolidaysCalendar
        {
            get => _holidaysCalendar;
            set
            {
                _holidaysCalendar = value;
                holidays_calendar_id = _holidaysCalendar?.CalendarId ?? NoCalendar;
            }
        }

        public bool IsNymex()
        {
            if (null == Exchange)
            {
                return false;
            }

            return "nymex".Equals(
                Exchange.Name,
                StringComparison.InvariantCultureIgnoreCase);
        }

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

        [ForeignKey("OfficialProductId")]
        public virtual OfficialProduct OfficialProduct
        {
            get => _officialProduct;
            set
            {
                _officialProduct = value;
                OfficialProductId = _officialProduct?.OfficialProductId ?? 0;

                LoggerHelper.LogOffProduct(_officialProduct);
            }
        }

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

        [ForeignKey("FeeClearingCurrencyId")]
        public virtual Currency FeeClearingCurrency
        {
            get => _feeClearingCurrency;
            set
            {
                _feeClearingCurrency = value;
                FeeClearingCurrencyId = _feeClearingCurrency?.CurrencyId;
            }
        }

        [ForeignKey("FeeCommissionCurrencyId")]
        public virtual Currency FeeCommissionCurrency
        {
            get => _feeCommissionCurrency;
            set
            {
                _feeCommissionCurrency = value;
                FeeCommissionCurrencyId = _feeCommissionCurrency?.CurrencyId;
            }
        }

        [ForeignKey("FeeExchangeCurrencyId")]
        public virtual Currency FeeExchangeCurrency
        {
            get => _feeExchangeCurrency;
            set
            {
                _feeExchangeCurrency = value;
                FeeExchangeCurrencyId = _feeExchangeCurrency?.CurrencyId;
            }
        }

        [ForeignKey("FeeNfaCurrencyId")]
        public virtual Currency FeeNfaCurrency
        {
            get => _feeNfaCurrency;
            set
            {
                _feeNfaCurrency = value;
                FeeNfaCurrencyId = _feeNfaCurrency?.CurrencyId;
            }
        }

        [ForeignKey("FeeCashCurrencyId")]
        public virtual Currency FeeCashCurrency
        {
            get => _feeCashCurrency;
            set
            {
                _feeCashCurrency = value;
                FeeCashCurrencyId = _feeCashCurrency?.CurrencyId;
            }
        }

        [ForeignKey("FeeBlockCurrencyId")]
        public virtual Currency FeeBlockCurrency
        {
            get => _feeBlockTradeCurrency;
            set
            {
                _feeBlockTradeCurrency = value;
                FeeBlockCurrencyId = _feeBlockTradeCurrency?.CurrencyId;
            }
        }

        [ForeignKey("FeePlattsCurrencyId")]
        public virtual Currency FeePlattsCurrency
        {
            get => _feePlattsTradeCurrency;
            set
            {
                _feePlattsTradeCurrency = value;
                FeePlattsCurrencyId = _feePlattsTradeCurrency?.CurrencyId;
            }
        }

        [ForeignKey("BalmoOnCrudeProductId")]
        public virtual Product BalmoOnCrudeProduct
        {
            get => _balmoOnCrudeProduct;
            set
            {
                _balmoOnCrudeProduct = value;
                BalmoOnCrudeProductId = _balmoOnCrudeProduct?.ProductId;
            }
        }

        [ForeignKey("BalmoOnComplexProductId")]
        public virtual Product BalmoOnComplexProduct
        {
            get => _balmoOnComplexProduct;
            set
            {
                _balmoOnComplexProduct = value;
                BalmoOnComplexProductId = _balmoOnComplexProduct?.ProductId;
            }
        }

        [ForeignKey("UnderlyingFuturesProductId")]
        public virtual OfficialProduct UnderlyingFutures
        {
            get => _underlyingFutures;
            set
            {
                _underlyingFutures = value;
                UnderlyingFuturesProductId = _underlyingFutures?.OfficialProductId;
            }
        }

        [ForeignKey("UnderlyingFuturesOverrideId")]
        public virtual OfficialProduct UnderlyingFuturesOverride
        {
            get => _underlyingFuturesOverride;
            set
            {
                _underlyingFuturesOverride = value;
                UnderlyingFuturesOverrideId = _underlyingFuturesOverride?.OfficialProductId;
            }
        }

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

        [ForeignKey("TasOfficialProductId")]
        public virtual OfficialProduct TasOfficialProduct
        {
            get => _tasOfficialProduct;
            set
            {
                _tasOfficialProduct = value;
                TasOfficialProductId = _tasOfficialProduct?.OfficialProductId;
            }
        }

        [ForeignKey("CategoryOverrideId")]
        public virtual ProductCategory CategoryOverride
        {
            get => _categoryOverride;
            set
            {
                _categoryOverride = value;
                CategoryOverrideId = _categoryOverride?.CategoryId;
            }
        }

        [ForeignKey("MonthlyOfficialProductId")]
        public virtual OfficialProduct MonthlyOfficialProduct
        {
            get => _monthlyOfficialProduct;
            set
            {
                _monthlyOfficialProduct = value;
                MonthlyOfficialProductId = _monthlyOfficialProduct?.OfficialProductId ?? 0;
            }
        }

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

        public virtual ICollection<ComplexProduct> ComplexParentProducts1 { get; set; }
        public virtual ICollection<ComplexProduct> ComplexParentProducts2 { get; set; }

        public virtual ICollection<Product> ComplexProductBalmos { get; set; }
        public virtual ICollection<Product> CrudeSwapBalmos { get; set; }
        public virtual ICollection<ProductAlias> Aliases { get; set; }
        public virtual ICollection<SecurityDefinition> security_definitions { get; set; }
        public virtual ICollection<ProductCategory> product_categories { get; set; }
        public virtual ICollection<ProductCategory> product_categories1 { get; set; }
        public virtual ICollection<ProductBrokerage> CompaniesBrokerages { get; set; }
        public virtual ICollection<ProductCategory> product_categories1_1 { get; set; }
        public virtual ICollection<IceProductMapping> IceProductsMappings { get; set; }
        public virtual ICollection<ABNMappings> ABNMappings { get; set; }
        public virtual ICollection<TradeScenario> trade_scenarios { get; set; }
        public virtual ICollection<GmiBalmoCode> GmiBalmoCodes { get; set; }
        public virtual ICollection<IceBalmoMapping> IceBalmoMappings { get; set; }
        public virtual ICollection<AdministrativeAlert> adm_alerts { get; set; }
        public virtual ICollection<ProductCategory> product_categories3 { get; set; }
        public virtual ICollection<SwapCrossPerProduct> swap_cross_per_product { get; set; }
        public virtual ICollection<SwapCrossPerProduct> swap_cross_per_product1 { get; set; }
        public static string BfoeCategoryName = "bfoe";

        [NotMapped]
        public Product Instance => this;

        [NotMapped]
        public ProductType Type
        {
            get => (ProductType)ProductTypeDb;
            set => ProductTypeDb = (short)value;
        }

        [NotMapped]
        public bool IsProductDaily => Type.IsDaily();

        [NotMapped]
        public bool IsDailyDiff => Type.IsDailyOrWeeklyDiff();

        [NotMapped]
        public string ProductTypeDisplay => Type.Name();

        [NotMapped]
        public TimeZoneInfo Timezone
        {
            get => string.IsNullOrEmpty(TimezoneId) ? null : TimeZoneInfo.FindSystemTimeZoneById(TimezoneId);
            set => TimezoneId = value?.Id;
        }

        [NotMapped]
        public DateTime? RolloffTimeToday
        {
            get
            {
                if (RolloffTime == null)
                {
                    return null;
                }

                return SystemTime.Today().Add(RolloffTime.Value.TimeOfDay);
            }
        }

        private DateTime? _localRolloffTime = null;
        private StockCalendar _expiryCalendar;
        private ComplexProduct _complexProduct;
        private ProductCategory _category;
        private OfficialProduct _officialProduct;
        private Product _balmoOnCrudeProduct;
        private Product _balmoOnComplexProduct;
        private OfficialProduct _underlyingFutures;
        private OfficialProduct _underlyingFuturesOverride;
        private Exchange _exchange;
        private OfficialProduct _tasOfficialProduct;
        private ProductCategory _categoryOverride;
        private OfficialProduct _monthlyOfficialProduct;
        private Unit _unit;
        private StockCalendar _holidaysCalendar;
        private Currency _currency1;
        private Currency _currency2;
        private Currency _feeClearingCurrency;
        private Currency _feeCommissionCurrency;
        private Currency _feeExchangeCurrency;
        private Currency _feeNfaCurrency;
        private Currency _feeCashCurrency;
        private Currency _feeBlockTradeCurrency;
        private Currency _feePlattsTradeCurrency;

        [NotMapped]
        public DateTime? FuturesExpireTimeToday
        {
            get
            {
                if (FuturesExpireTime == null)
                {
                    return null;
                }

                return SystemTime.Today().Add(FuturesExpireTime.Value.TimeOfDay);
            }
        }

        [NotMapped]
        public DateTime? LocalRolloffTime
        {
            get
            {
                if (Type == ProductType.Futures && FuturesExpireTimeToday.HasValue)
                {
                    return FuturesExpireTimeToday.Value;
                }

                if (string.IsNullOrEmpty(TimezoneId) || !RolloffTimeToday.HasValue)
                {
                    return null;
                }

                if (!(ProductType.Swap == Type && TasType.NotTas != TasType)
                       && (!UseRolloffSettings.HasValue || !UseRolloffSettings.Value))
                {
                    return null;
                }

                _localRolloffTime = GetLocalRollOffTime();
                return _localRolloffTime;
            }
        }

        private DateTime GetLocalRollOffTime()
        {
            return new DateTime(
                RolloffTimeToday.Value.Year,
                RolloffTimeToday.Value.Month,
                RolloffTimeToday.Value.Day,
                RolloffTimeToday.Value.Hour,
                RolloffTimeToday.Value.Minute,
                RolloffTimeToday.Value.Second,
                DateTimeKind.Unspecified).ToLocalTime(TimezoneId);
        }

        [NotMapped]
        public DateTime? RolloffTimeWithTimezone
        {
            get
            {
                if (RolloffTimeToday == null)
                {
                    return null;
                }

                _localRolloffTime = GetLocalRollOffTime();
                return _localRolloffTime;
            }
        }

        [NotMapped]
        public string LocalRolloffTimeString => LocalRolloffTime?.ToShortTimeString();

        [NotMapped]
        public TimeSpan RolloffTimeOffset
        {
            get
            {
                DateTime? localRolloffTime = LocalRolloffTime;

                if (localRolloffTime.HasValue && RolloffTimeToday.HasValue)
                {
                    return RolloffTimeToday.Value - localRolloffTime.Value;
                }

                return TimeSpan.Zero;
            }
        }

        [NotMapped]
        public ExpirationType ExpirationType
        {
            get => (ExpirationType)(ExpirationTypeDb ?? 0);
            set => ExpirationTypeDb = (short)value;
        }

        [NotMapped]
        public RollingMethod RollingMethod
        {
            get => (RollingMethod)(RollingMethodDb ?? 0);
            set => RollingMethodDb = (short)value;
        }

        [NotMapped]
        public bool IsPhysicallySettled
        {
            get => IsPhysicallySettledDb ?? false;
            set => IsPhysicallySettledDb = value;
        }

        [NotMapped]
        public bool IsInternalTransferProduct
        {
            get => IsInternalTransferProductDb ?? false;
            set => IsInternalTransferProductDb = value;
        }

        [NotMapped]
        public bool IsEnabledRiskDecomposition
        {
            get => IsEnabledRiskDecompositionDb ?? true;
            set => IsEnabledRiskDecompositionDb = value;
        }

        [NotMapped]
        public decimal PriceConversionFactor
        {
            get => PriceConversionFactorDb ?? 1M;
            set => PriceConversionFactorDb = value;
        }

        [NotMapped]
        private bool IsTas
        {
            get => IsTasDb ?? false;
            set => IsTasDb = value;
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
        public bool TreatTimespreadStripAsLegs
        {
            get => TreatTimespreadStripAsLegsDb ?? false;
            set => TreatTimespreadStripAsLegsDb = value;
        }


        public override bool Equals(object obj)
        {
            Product p = obj as Product;

            return ProductId == p?.ProductId;
        }

        public override int GetHashCode()
        {
            return ProductId;
        }

        public bool IsNew()
        {
            return 0 == ProductId;
        }

        public override string ToString()
        {
            return Name;
        }

        public void ClearComplexProductReference()
        {
            _complexProduct = null;
        }

        public void SetComplexProductReference(ComplexProduct complexProduct = null)
        {
            if (_complexProduct == null)
            {
                _complexProduct = complexProduct ?? new ComplexProduct { ProductId = ProductId };
            }
        }

        public bool IsComplexProduct()
        {
            if (ComplexProduct != null &&
                ComplexProduct.ChildProduct1 != null &&
                ComplexProduct.ChildProduct2 != null)
            {
                return true;
            }

            return false;
        }

        public bool IsBalmoOnComplexProduct()
        {
            if (BalmoOnComplexProduct != null &&
                BalmoOnComplexProduct.ComplexProduct != null &&
                BalmoOnComplexProduct.ComplexProduct.ChildProduct1 != null &&
                BalmoOnComplexProduct.ComplexProduct.ChildProduct2 != null)
            {
                return true;
            }

            return false;
        }

        public Collection<Product> GetProducts()
        {
            Collection<Product> relatedProducts = new Collection<Product>() { this };

            if (IsComplexProduct())
            {
                relatedProducts.Add(ComplexProduct.ChildProduct1);
                relatedProducts.Add(ComplexProduct.ChildProduct2);
            }
            else if (IsBalmoOnComplexProduct())
            {
                relatedProducts.Add(BalmoOnComplexProduct);
                relatedProducts.Add(BalmoOnComplexProduct.ComplexProduct.ChildProduct1);
                relatedProducts.Add(BalmoOnComplexProduct.ComplexProduct.ChildProduct2);
            }
            else if (IsBalmoOnCrudeProduct())
            {
                relatedProducts.Add(BalmoOnCrudeProduct);
            }

            return relatedProducts;
        }

        public bool IsBalmoOnCrudeProduct()
        {
            return BalmoOnCrudeProduct != null;
        }

        [Obsolete("This is a fairly horrible hack. String comparison must be replaced.")]
        public bool IsBfoe()
        {
            string categoryName = Category?.Name ?? Name;

            return IsBfoe(categoryName);
        }

        [Obsolete("This is a horrible hack. String comparison must be replaced.")]
        private bool IsBfoe(string productOrCategoryName)
        {
            return productOrCategoryName.IndexOf(BfoeCategoryName, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public bool IsWeeklyDiffOrDailySwapProduct()
        {
            return Type == ProductType.DayVsMonthFullWeek || IsDailySwapProduct();
        }

        public bool IsDailySwapProduct()
        {
            return Type == ProductType.DailySwap && !Name.ToLower().Contains("dummy");
        }

        public void SanitiseCircularReferences()
        {
            if (null != security_definitions)
            {
                ICollection<SecurityDefinition> productSecDefs = security_definitions;

                InitialiseSecurityDefinitionsReference();
                productSecDefs.ForEach(secDef => secDef.SanitiseCircularReferences());
                productSecDefs.Clear();
            }
        }

        public bool TradedAtSettlement()
        {
            return TasType != TasType.NotTas;
        }

        private HashSet<string> _currencyNames = new HashSet<string>();

        public HashSet<string> CurrencyNames()
        {
            return _currencyNames.Any() ? _currencyNames : FillCurrencyNames();
        }

        private HashSet<string> FillCurrencyNames()
        {
            _currencyNames = new Currency[]
            {
                FeeClearingCurrency ?? Currency.Default,
                FeeBlockCurrency ?? Currency.Default,
                FeeCashCurrency ?? Currency.Default,
                FeeCommissionCurrency ?? Currency.Default,
                FeeExchangeCurrency ?? Currency.Default,
                FeeNfaCurrency ?? Currency.Default,
                FeePlattsCurrency ?? Currency.Default
            }.Where(currency => !currency.IsDefault()).Select(currency => currency.IsoName).ToHashSet();

            return _currencyNames;
        }

        public bool IsFutures()
        {
            return Type == ProductType.Futures;
        }

        public bool IsTasType()
        {
            return (IsTasDb ?? false) || (IsMmDb ?? false);
        }
    }
}
