using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities.Entities
{
    [Table("foreign_currency_positions")]
    public class ForeignCurrencyPosition
    {
        private Currency _currency;
        private Portfolio _portfolio;

        [Column("foreign_currency_position_id")]
        [Key]
        public int ForeignCurrencyPositionId { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("currency_id")]
        public int CurrencyId { get; set; }

        [Column("portfolio_id")]
        public int PortfolioId { get; set; }

        [Column("value")]
        public decimal TotalPositionPnLValue { get; set; }

        public virtual ICollection<ForeignCurrencyPositionDetail> ForeignCurrencyPositionDetails { get; set; }

        [ForeignKey("CurrencyId")]
        public Currency Currency
        {
            get { return _currency; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _currency = value;
                CurrencyId = _currency.CurrencyId;
            }
        }

        [ForeignKey("PortfolioId")]
        public Portfolio Portfolio
        {
            get { return _portfolio; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _portfolio = value;
                PortfolioId = _portfolio.PortfolioId;
            }
        }
    }
}
