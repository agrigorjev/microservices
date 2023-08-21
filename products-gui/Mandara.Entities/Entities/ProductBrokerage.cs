using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("products_brokerage")]
    public partial class ProductBrokerage
    {
        private Product _product;
        private Company _company;

        [Key]
        [Column("product_id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Key]
        [Column("company_id", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CompanyId { get; set; }

        [Column("brokerage")]
        public decimal Brokerage { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company
        {
            get { return _company; }
            set
            {
                _company = value;
                if (_company != null)
                    CompanyId = _company.CompanyId;
            }
        }

        [ForeignKey("ProductId")]
        public virtual Product Product
        {
            get { return _product; }
            set
            {
                _product = value;
                if (_product != null)
                    ProductId = _product.ProductId;
            }
        }
    }
}
