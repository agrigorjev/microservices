using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataEntries
{
    [Table("official_products")]
    public class OfficialProduct
    {
        public bool IsValid =>
            (null != PriceUnit && !PriceUnit.IsDefault()) && (null != Currency && !Currency.IsDefault());

        private Region _region;
        private Currency _currency;
        private Unit _priceUnit;

        public const string DefaultName = "DefaultOffProd";
        public const int DefaultId = 0;
        
        public static readonly OfficialProduct Default = new OfficialProduct()
        {
            OfficialProductId = DefaultId,
            Currency = Currency.Default,
            Name = DefaultName,
            DisplayName = DefaultName,
            PriceUnit = Unit.Default,
        };

        public bool IsDefault()
        {
            return DefaultName == Name
                   && DefaultName == DisplayName
                   && Currency.Default == Currency
                   && Unit.Default == PriceUnit;
        }

        public OfficialProduct()
        {
            official_products1 = new HashSet<OfficialProduct>();
            IsAllowedForManualTrades = true;
        }

        [Column("official_product_id")]
        [Key]
        public int OfficialProductId { get; set; }

        [Column("official_name")]
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("display_name")]
        [StringLength(255)]
        public string DisplayName { get; set; }

        [Column("price_mapping_column")]
        [StringLength(50)]
        public string MappingColumn { get; set; }

        [Column("apply_sign_verification")]
        public bool? ApplySignVerification { get; set; }

        [Column("apply_fraction_part_verification")]
        public bool? ApplyFractionPartVerification { get; set; }

        [Column("epsilon")]
        public decimal? Epsilon { get; set; }

        [Column("apply_missing_point_verification")]
        public bool? ApplyMissingPointVerification { get; set; }

        [Column("missing_point_accuracy")]
        public decimal? MissingPointAccuracy { get; set; }

        [Column("voice_name")]
        [StringLength(255)]
        public string VoiceName { get; set; }

        [Column("region_id")]
        public int? RegionId { get; set; }

        private bool _publishToUms;

        [Column("publish_to_ums")]
        public bool? PublishToUms
        {
            get => _publishToUms;
            set => _publishToUms = value ?? false;
        }

        [Column("name_on_ums")]
        [StringLength(255)]
        public string NameOnUms { get; set; }

        [Column("price_unit")]
        public int PriceUnitId { get; set; }

        [ForeignKey("PriceUnitId")]
        public Unit PriceUnit
        {
            get => _priceUnit;
            set
            {
                _priceUnit = value ?? Unit.Default;
                PriceUnitId = _priceUnit.UnitId;
            }
        }

        [Column("bbl_conversion")]
        public decimal UnitToBarrelConversionFactor { get; set; }

        [Column("price_expiration_period")]
        public DateTime? price_expiration_period { get; set; }

        [Column("spread_price_expiration_period")]
        public DateTime? spread_price_expiration_period { get; set; }

        [Column("desk_id")]
        public int? desk_id { get; set; }

        [Column("settlement_product_id")]
        public int? SettlementProductId { get; set; }

        [Column("is_allowed_for_manual_trades")]
        public bool? IsAllowedForManualTradesDb { get; set; }

        [ForeignKey("RegionId")]
        public virtual Region Region
        {
            get => _region;
            set
            {
                _region = value;
                RegionId = _region?.RegionId;
            }
        }

        [Column("currency_id")]
        public int CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public Currency Currency
        {
            get => _currency;
            set
            {
                _currency = value ?? Currency.Default;
                CurrencyId = _currency.CurrencyId;
            }
        }

        [NotMapped]
        public bool IsAllowedForManualTrades
        {
            get => IsAllowedForManualTradesDb ?? true;
            set => IsAllowedForManualTradesDb = value;
        }

        public virtual OfficialProduct official_products2 { get; set; }

        public virtual ICollection<OfficialProduct> official_products1 { get; set; }


        [NotMapped]
        public OfficialProduct Instance => this;

        public override bool Equals(object obj)
        {
            if (!(obj is OfficialProduct offProd))
            {
                return false;
            }

            return OfficialProductId == offProd.OfficialProductId;
        }

        public override int GetHashCode()
        {
            return OfficialProductId;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
