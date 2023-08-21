using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("dictionary_products")]
    public partial class ParserWord
    {
        private OfficialProduct _officialProduct;
        private Broker _broker;

        [Column("word_id")]
        [Key]
        public int WordId { get; set; }

        [Column("word")]
        [StringLength(50)]
        public string Word { get; set; }

        [Column("official_product_id")]
        public int? OfficialProductId { get; set; }

        [Column("broker_id")]
        public int? BrokerId { get; set; }

        [Column("yahoo_group_id")]
        public int? yahoo_group_id { get; set; }

        [ForeignKey("OfficialProductId")]
        public virtual OfficialProduct OfficialProduct
        {
            get { return _officialProduct; }
            set
            {
                _officialProduct = value;
                OfficialProductId = _officialProduct != null ? _officialProduct.OfficialProductId : (int?) null;
            }
        }

        [ForeignKey("BrokerId")]
        public virtual Broker Broker
        {
            get { return _broker; }
            set
            {
                _broker = value;
                BrokerId = _broker != null ? _broker.BrokerId : (int?) null;
            }
        }

        public override string ToString()
        {
            return string.Format("Word: {0}; Broker: {1}", Word, Broker != null ? Broker.ToString() : "<Empty>");
        }
    }
}
