using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.Entities.Entities
{

    [Table("exchange_account")]
    public partial class ExchangeAccount
    {

        [Column("exchange_account_id")]
        [Key]
        public int ExchangeAccountId { get; set; }

        [Column("account_name")]
        [Required]
        public string AccountName { get; set; }

        [Column("portfolio_id")]
        [Required]
        public int PortfolioId { get; set; }

        [ForeignKey("PortfolioId")]
        public virtual Portfolio Portfolio { get; set; }

    }
}
