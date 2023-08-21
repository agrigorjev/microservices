using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("trade_scenarios")]
    public partial class TradeScenario
    {
        [Column("trade_scenario_id")]
        [Key]
        public int TradeScenarioId { get; set; }

        [Column("product_id")]
        public int product_id { get; set; }

        [Column("strip1_date_type")]
        public int Strip1DateType { get; set; }

        [Column("strip1_date_index")]
        public int Strip1DateIndex { get; set; }

        [Column("strip2_date_type")]
        public int? Strip2DateType { get; set; }

        [Column("strip2_date_index")]
        public int? Strip2DateIndex { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [ForeignKey("product_id")]
        public virtual Product Product
        {
            get { return _product; }
            set
            {
                _product = value;
                product_id = _product != null ? _product.ProductId : 0;
            }
        }

        private int? _productId;

        [NotMapped]
        public int ProductId
        {
            get
            {
                if (_productId.HasValue)
                {
                    return _productId.Value;
                }

                return Product.ProductId;
            }
            set { _productId = value; }
        }

        private RelativeStrip? _relativeStrip1 = null;
        
        [NotMapped]
        public RelativeStrip Strip1
        {
            get
            {
                return new RelativeStrip
                {
                    DateType = (ProductDateType)Strip1DateType,
                    DateIndex = Strip1DateIndex
                };
            }

            set
            {
                Strip1DateIndex = value.DateIndex;
                Strip1DateType = (int)value.DateType;
            }
        }

        private RelativeStrip? _relativeStrip2 = null;
        private Product _product;

        [NotMapped]
        public RelativeStrip? Strip2
        {
            get
            {
                if (Strip2DateType.HasValue && Strip2DateIndex.HasValue)
                {
                    return new RelativeStrip
                    {
                        DateType = (ProductDateType)Strip2DateType.Value,
                        DateIndex = Strip2DateIndex.Value
                    };
                }
                else
                    return null;

            }

            set
            {
                Strip2DateIndex = value != null ? value.Value.DateIndex : (int?)null;
                Strip2DateType = value != null ? (int)value.Value.DateType : (int?)null;
            }
        }

        public bool IsTimeSpread { get { return Strip2.HasValue; } }

        [NotMapped]
        public string RelativeStripName
        {
            get
            {
                string strip1String = Strip1.ToString();

                if (IsTimeSpread)
                    return string.Format("{0}/{1}", strip1String, Strip2.Value.ToString());
                else
                    return strip1String;
            }
        }

        [NotMapped]
        public string AbsoluteStripName
        {
            get
            {
                string strip1String = Strip1.ToAbsoluteStrip();

                if (IsTimeSpread)
                    return string.Format("{0}/{1}", strip1String, Strip2.Value.ToAbsoluteStrip());
                else
                    return strip1String;
            }
        }

        [NotMapped]
        public string Side
        {
            get { return Quantity < 0 ? "Sell" : "Buy"; }
        }

        public override string ToString()
        {
            return string.Format("Product: {0}, Strip Name: {1}, Quantity: {2:F2}.",
                                 Product != null ? Product.Name : string.Empty, RelativeStripName, Quantity);
        }

    }
}
