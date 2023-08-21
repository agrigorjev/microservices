using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mandara.TradeApiService.Data
{
    [Table("company_aliases")]
    public class CompanyAlias
    {
        private Company _company;

        [Column("alias_id")]
        [Key]
        public int AliasId { get; set; }

        [Column("alias_name")]
        [Required]
        [StringLength(255)]
        public string AliasName { get; set; }

        [Column("company_id")]
        public int CompanyId { get; set; }

        public virtual Company Company
        {
            get { return _company; }
            set
            {
                _company = value;
                CompanyId = _company != null ? _company.CompanyId : default (int);
            }
        }

        public override string ToString()
        {
            return AliasName;
        }

    }
}
