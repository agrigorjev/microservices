using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("swap_cross_per_product")]
    public partial class SwapCrossPerProduct
    {
        public const int DefaultId = -1;
        private ProductCategory _category;
        private Product _categoryProduct;
        private Product _balmoSwapCrossProduct;
        
        public static readonly SwapCrossPerProduct Default = new SwapCrossPerProduct()
        {
            SwapCrossPerProductId = DefaultId,
            Category = ProductCategory.Default,
            CategoryProduct = Product.Default,
            BalmoSwapCrossProduct = Product.Default
        };

        [NotMapped]
        public bool IsDefault => DefaultId == SwapCrossPerProductId &&
                                 ProductCategory.Default.Equals(Category) &&
                                 Product.Default.Equals(CategoryProduct) &&
                                 Product.Default.Equals(BalmoSwapCrossProduct);

        [Column("swap_cross_per_product_id")]
        [Key]
        public int SwapCrossPerProductId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("category_product_id")]
        public int CategoryProductId { get; set; }

        [Column("balmo_swap_cross_product_id")]
        public int BalmoSwapCrossProductId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual ProductCategory Category
        {
            get { return _category; }
            set
            {
                _category = value;
                CategoryId = _category != null ? _category.CategoryId : 0;
            }
        }

        [ForeignKey("CategoryProductId")]
        public virtual Product CategoryProduct
        {
            get { return _categoryProduct; }
            set
            {
                _categoryProduct = value;
                CategoryProductId = _categoryProduct != null ? _categoryProduct.ProductId : 0;
            }
        }

        [ForeignKey("BalmoSwapCrossProductId")]
        public virtual Product BalmoSwapCrossProduct
        {
            get { return _balmoSwapCrossProduct; }
            set
            {
                _balmoSwapCrossProduct = value;
                BalmoSwapCrossProductId = _balmoSwapCrossProduct != null ? _balmoSwapCrossProduct.ProductId : 0;
            }
        }
    }
}
