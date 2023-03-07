using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("portfolio_trades")]
    public partial class PortfolioTrade
    {
        private Portfolio _portfolio;

        [Column("trade_id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TradeId { get; set; }
        
        [Column("portfolio_trade_idx")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PortfolioTradeIdx { get; set; }

        [Column("portfolio_id")]
        public int PortfolioId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("created_by")]
        [Required]
        [StringLength(255)]
        public string CreatedBy { get; set; }

        [Column("is_parent_trade")]
        public bool IsParentTrade { get; set; }

        [ForeignKey("PortfolioId")]
        public virtual Portfolio Portfolio
        {
            get { return _portfolio; }
            set
            {
                _portfolio = value;
                PortfolioId = _portfolio != null ? _portfolio.PortfolioId : 0;
            }
        }
    }
}
