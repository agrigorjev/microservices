using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Mandara.TradeApiService.Data
{
    [Description("Company")]
    [Table("companies")]
    public class Company
    {
        private Region _region;

        public Company()
        {
            Brokers = new HashSet<Broker>();
            CompanyAliases = new HashSet<CompanyAlias>();
        }

        [Column("company_id")]
        [Key]
        public int CompanyId { get; set; }

        [Column("company_name")]
        [Required]
        [StringLength(255)]
        public string CompanyName { get; set; }

        [Column("abbreviation")]
        [StringLength(4)]
        public string? AbbreviationName { get; set; }

        public virtual ICollection<Broker> Brokers { get; set; }

        public virtual ICollection<CompanyAlias> CompanyAliases { get; set; }

        [NotMapped]
        public Company Instance
        {
            get
            {
                return this;
            }
        }

        [NotMapped]
        public string Abbr => AbbreviationName ?? CompanyName.ToUpper()[..3];

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Region return false.
            if (obj is not Company p)
            {
                return false;
            }

            return CompanyId == p.CompanyId;
        }

        public override int GetHashCode()
        {
            return CompanyId.GetHashCode();
        }

        public override string ToString()
        {
            return CompanyName;
        }
    }
}
