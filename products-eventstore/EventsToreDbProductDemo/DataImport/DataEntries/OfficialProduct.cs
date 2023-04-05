using EventStore.Client;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataImport.DataEntries
{
    [Table("official_products")]
    public class OfficialProduct
    {

        public const string DefaultName = "DefaultOffProd";
        public const int DefaultId = 0;

        private Guid? _id;

        [NotMapped]
        public Guid Id { get; set; }
            
        public static readonly OfficialProduct Default = new OfficialProduct()
        {
            OfficialProductId = DefaultId,
            Name = DefaultName,
            DisplayName = DefaultName,
            Id=Guid.NewGuid(),
        };

        public bool IsDefault()
        {
            return DefaultName == Name
                   && DefaultName == DisplayName;
        }

        public OfficialProduct()
        {
            Id= Guid.NewGuid();
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
        public string? DisplayName { get; set; }

        [Column("price_mapping_column")]
        [StringLength(50)]
        public string? MappingColumn { get; set; }

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
        public string? VoiceName { get; set; }

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
        public string? NameOnUms { get; set; }

        [Column("price_unit")]
        public int PriceUnitId { get; set; }

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

        [Column("currency_id")]
        public int CurrencyId { get; set; }

        [NotMapped]
        public Guid CurrencyGuId { get; set; }

        [NotMapped]
        public Guid? RegionGuId { get; set; }

        [NotMapped]
        public Guid UnitGuid { get; set; }

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
