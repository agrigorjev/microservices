using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("hal_products")]
    public partial class HALProduct
    {
        private OfficialProduct _officialProduct;
        private VHALMessage _vhalMessage;

        [Column("product_id")]
        [Key]
        public int ProductId { get; set; }

        [Column("message_id")]
        public long MessageId { get; set; }

        [Column("official_product_id")]
        public int OfficialProductId { get; set; }

        [Column("product_date1", TypeName = "date")]
        public DateTime? Date1 { get; set; }

        [Column("product_date1_type")]
        public short? DateTypeDb1 { get; set; }

        [Column("product_date2", TypeName = "date")]
        public DateTime? Date2 { get; set; }

        [Column("product_date2_type")]
        public short? DateTypeDb2 { get; set; }

        [Column("price_bid", TypeName = "money")]
        public decimal? Bid { get; set; }

        [Column("price_ask", TypeName = "money")]
        public decimal? Ask { get; set; }

        [Column("price_mid", TypeName = "money")]
        public decimal? Mid { get; set; }

        [Column("mandara_price", TypeName = "money")]
        public decimal? MandaraPrice { get; set; }

        [Column("marked_incorrect")]
        public bool? MarkedIncorrect { get; set; }

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

        [ForeignKey("MessageId")]
        public virtual VHALMessage VHALMessage
        {
            get { return _vhalMessage; }
            set
            {
                _vhalMessage = value;
                MessageId = _vhalMessage != null ? _vhalMessage.MessageId : 0;
            }
        }
    }
}
