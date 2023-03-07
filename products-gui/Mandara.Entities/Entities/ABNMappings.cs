using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("abn_product_mapping")]
    public partial class ABNMappings
    {
        private Product _product;

        [Column("mapping_id")]
        [Key]
        public int mapping_id { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("exchange_code")]
        [StringLength(50)]
        public string ExchangeCode { get; set; }

        [Column("product_code")]
        [StringLength(50)]
        public string ProductCode { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product
        {
            get { return _product; }
            set
            {
                _product = value;
                ProductId = _product != null ? _product.ProductId : default (int);
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ExchangeCode) || !string.IsNullOrWhiteSpace(ProductCode);
        }
    }
}
