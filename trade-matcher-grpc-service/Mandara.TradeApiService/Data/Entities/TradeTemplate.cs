using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.TradeApiService.Data
{
    [Serializable]
    [Table("trade_templates")]
    public partial class TradeTemplate
    {
        private OfficialProduct _officialProduct;
        private Portfolio _portfolio;
        private Unit _unit;

        [Column("trade_template_id")]
        [Key]
        public int TradeTemplateId { get; set; }

        [Column("portfolio_id")]
        public int PortfolioId { get; set; }

        [Column("official_product_id")]
        public int OfficialProductId { get; set; }

        [Column("exchange_id")]
        public int ExchangeId { get; set; }

        [Column("volume")]
        public decimal Volume { get; set; }

//        [Column("units_db")]
//        public int UnitsDb { get; set; }

        [Column("template_name")]
        [Required]
        [StringLength(250)]
        public string TemplateName { get; set; }

        [Column("unit_id")]
        public int? UnitId { get; set; }

        [ForeignKey("ExchangeId")]
        public virtual Exchange Exchange { get; set; }

        [ForeignKey("OfficialProductId")]
        public virtual OfficialProduct OfficialProduct
        {
            get { return _officialProduct; }
            set
            {
                _officialProduct = value;
                OfficialProductId = _officialProduct != null ? _officialProduct.OfficialProductId : 0;
            }
        }

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

        [ForeignKey("UnitId")]
        public virtual Unit Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                UnitId = _unit != null ? _unit.UnitId : 0;
            }
        }


//        [NotMapped]
//        public Units Units
//        {
//            get { return (Units)UnitsDb; }
//            set { UnitsDb = (int)value; }
//        }

    }
}
