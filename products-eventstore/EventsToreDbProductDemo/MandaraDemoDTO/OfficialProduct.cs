

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MandaraDemoDTO
{
    public partial class OfficialProduct
    {

        public Guid Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? DisplayName { get; set; }

        [StringLength(50)]
        public string? MappingColumn { get; set; }

        public bool? ApplySignVerification { get; set; }

        public bool? ApplyFractionPartVerification { get; set; }

        public decimal? Epsilon { get; set; }

        public bool? ApplyMissingPointVerification { get; set; }

        public decimal? MissingPointAccuracy { get; set; }

        [StringLength(255)]
        public string? VoiceName { get; set; }

        private bool _publishToUms;
        public bool? PublishToUms
        {
            get => _publishToUms;
            set => _publishToUms = value ?? false;
        }

        [StringLength(255)]
        public string? NameOnUms { get; set; }
        public decimal UnitToBarrelConversionFactor { get; set; }
        public DateTime? price_expiration_period { get; set; }

        public DateTime? spread_price_expiration_period { get; set; }

        public int? desk_id { get; set; }

        public int? SettlementProductId { get; set; }

        public bool? IsAllowedForManualTradesDb { get; set; }

        public Guid CurrencyGuId { get; set; }

        public Guid? RegionGuId { get; set; }
        public Guid UnitGuid { get; set; }

        public Currency? Currency { get;set;}

        public Region? Region { get; set; }

        public Unit? PriceUnit { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is OfficialProduct offProd))
            {
                return false;
            }

            return Id == offProd.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }



    }
}
