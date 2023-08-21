using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("product_aliases")]
    public partial class ProductAlias
    {
        private Product _product;

        [Column("alias_id")]
        [Key]
        public int AliasId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("alias")]
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

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

        public override string ToString()
        {
            return Name;
        }
    }
}
