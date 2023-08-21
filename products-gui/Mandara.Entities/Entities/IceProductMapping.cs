using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("ice_product_mappings")]
    public partial class IceProductMapping
    {
        private Product _product;

        [Column("ice_product_id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IceProductId { get; set; }

        [Column("internal_product_id")]
        public int? InternalProductId { get; set; }

        [Column("updated")]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("InternalProductId")]
        public virtual Product Product
        {
            get { return _product; }
            set
            {
                _product = value;
                InternalProductId = _product != null ? _product.ProductId : (int?) null;
            }
        }
    }
}
