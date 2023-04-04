using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataEntries
{
    [Table("currencies")]
    public partial class Currency
    {
        private const int InvalidId = -1;
        private const string InvalidIso = "___";
        public static readonly Currency Default = new Currency() { CurrencyId = InvalidId, IsoName = InvalidIso };

        [ReadOnly(true)]        
        public bool IsDefault()
        {
            return InvalidId == CurrencyId || InvalidIso == IsoName;
        }

        [Column("currency_id")]
        [Key]
        public int CurrencyId { get; set; }

        private string _isoName;

        [Column("iso_name")]
        [Required]
        [StringLength(3)]
        public string IsoName
        {
            get => _isoName;
            set
            {
                if (value.Trim().Length != 3)
                {
                    throw new ArgumentOutOfRangeException(
                        "ISO name",
                        "A currency ISO name is exactly 3 uppercase characters.");
                }

                _isoName = value.ToUpper();
            }
        }

        [NotMapped]
        public Currency Instance => this;

        public override bool Equals(object obj)
        {
            return obj is Currency entity && CurrencyId == entity.CurrencyId;
        }

        public override int GetHashCode()
        {
            return CurrencyId;
        }

        public override string ToString()
        {
            return IsoName;
        }
    }
}
