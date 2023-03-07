using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Serializable]
    [Table("fx_trades")]
    public class FxTrade : INewable
    {

        [Column("fx_trade_id")]
        [Key]
        public int FxTradeId { get; set; }

        [Column("product_type")]
        [StringLength(20)]
        public string ProductType { get; set; }

        [Column("specified_currency_id")]
        public int SpecifiedCurrencyId { get; set; }

        [Column("specified_amount")]
        public decimal SpecifiedAmount { get; set; }

        [Column("against_currency_id")]
        public int AgainstCurrencyId { get; set; }

        [Column("against_amount")]
        public decimal AgainstAmount { get; set; }

        [Column("rate")]
        public decimal Rate { get; set; }

        [Column("spot_rate")]
        public decimal SpotRate { get; set; }

        [Column("tenor")]
        [StringLength(20)]
        public string Tenor { get; set; }

        [Column("link_trade_report_id")]
        [StringLength(50)]
        public string LinkTradeReportId { get; set; }

        [Column("link_type")]
        [StringLength(50)]
        public string LinkType { get; set; }

        [Column("value_date")]
        public DateTime ValueDate { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("trade_capture_id")]
        public int TradeCaptureId { get; set; }

        private Product _product;
        [ForeignKey("ProductId")]
        public virtual Product Product
        {
            get { return _product; }
            set
            {
                _product = value;
                ProductId = _product != null ? _product.ProductId : 0;
            }
        }

        private TradeCapture _tradeCapture;
        [ForeignKey("TradeCaptureId")]
        public virtual TradeCapture TradeCapture
        {
            get { return _tradeCapture; }
            set
            {
                _tradeCapture = value;
                TradeCaptureId = _tradeCapture != null ? _tradeCapture.TradeId : 0;
            }
        }

        private Currency _againstCurrency;
        [ForeignKey("AgainstCurrencyId")]
        public virtual Currency AgainstCurrency
        {
            get
            {
                return _againstCurrency;
            }
            set
            {
                _againstCurrency = value;
                AgainstCurrencyId = _againstCurrency != null ? _againstCurrency.CurrencyId : 0;
            }
        }

        private Currency _specifiedCurrency;
        [ForeignKey("SpecifiedCurrencyId")]
        public virtual Currency SpecifiedCurrency
        {
            get
            {
                return _specifiedCurrency;
            }
            set
            {
                _specifiedCurrency = value;
                SpecifiedCurrencyId = _specifiedCurrency != null ? _specifiedCurrency.CurrencyId : 0;
            }
        }

        public bool IsNew()
        {
            // Implicit assumption that the related TradeCapture will be checked independently.
            return 0 == FxTradeId;
        }
    }
}