using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.TradeApiService.Data
{
    [Table("units")]
    public partial class Unit
    {
        private const int InvalidId = -1;
        private const string InvalidName = "Invalid";
        public static readonly Unit Default = new Unit() { UnitId = InvalidId, Name = InvalidName };

        [ReadOnly(true)]        
        public bool IsDefault()
        {
            return InvalidId == UnitId || InvalidName == Name;
        }

        [Column("unit_id")]
        [Key]
        public int UnitId { get; set; }

        [Column("name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column("default_position_factor")]
        public decimal? DefaultPositionFactor { get; set; }

        [Column("only_month_contract_size")]
        public bool AllowOnlyMonthlyContractSize { get; set; }

        [NotMapped]
        public Unit Instance => this;

        public override bool Equals(object obj)
        {
            return obj is Unit entity && UnitId == entity.UnitId;
        }

        public override int GetHashCode()
        {
            return UnitId;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
