using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandara.Entities.Entities;

namespace Mandara.Entities
{
    [Table("product_categories")]
    public partial class ProductCategory
    {
        public const int DefaultId = -1;
        public const string DefaultName = "DefaultProductCategory";
        private Product _spreaderProduct;
        private Product _swapCrossProduct;
        private Product _transferProduct;
        private Product _swapCrossBalmoProduct;

        public ProductCategory()
        {
            this.Products = new HashSet<Product>();
            this.adm_alerts = new HashSet<AdministrativeAlert>();
            this.UserPortfolios = new HashSet<UserProductCategoryPortfolio>();
            this.SwapCrossPerProducts = new HashSet<SwapCrossPerProduct>();
            this.products1 = new HashSet<Product>();
            Classes = new HashSet<ProductClass>();
        }

        public static ProductCategory Default = new ProductCategory()
        {
            CategoryId = DefaultId,
            Name = DefaultName,
            SpreaderProduct = Product.Default,
            SwapCrossProduct = Product.Default,
            SwapCrossBalmoProduct = Product.Default,
            TransferProduct = Product.Default
        };

        [NotMapped]
        public bool IsDefault => DefaultId == CategoryId &&
                                 DefaultName == Name &&
                                 Product.Default.Equals(SpreaderProduct) &&
                                 Product.Default.Equals(SwapCrossProduct) &&
                                 Product.Default.Equals(SwapCrossBalmoProduct) &&
                                 Product.Default.Equals(TransferProduct);

        [Column("category_id")]
        [Key]
        public int CategoryId { get; set; }

        [Column("category_name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column("spreader_product_id")]
        public int? SpreaderProductId { get; set; }

        [Column("tas_check_required")]
        public bool? TasCheckRequired { get; set; }

        [Column("swap_cross_product_id")]
        public int? SwapCrossProductId { get; set; }

        [Column("abbreviation")]
        [StringLength(50)]
        public string Abbreviation { get; set; }

        [Column("transfer_product_id")]
        public int? TransferProductId { get; set; }

        [Column("tas_check_time")]
        public DateTime? TasCheckTime { get; set; }

        [Column("swap_cross_balmo_product_id")]
        public int? SwapCrossBalmoProductId { get; set; }

        [Column("is_illiquid")]
        public bool IsIlliquid { get; set; }

        [ForeignKey("SpreaderProductId")]
        public virtual Product SpreaderProduct
        {
            get { return _spreaderProduct; }
            set
            {
                _spreaderProduct = value;
                SpreaderProductId = _spreaderProduct != null ? _spreaderProduct.ProductId : (int?)null;
            }
        }

        [ForeignKey("SwapCrossProductId")]
        public virtual Product SwapCrossProduct
        {
            get { return _swapCrossProduct; }
            set
            {
                _swapCrossProduct = value;
                SwapCrossProductId = _swapCrossProduct != null ? _swapCrossProduct.ProductId : (int?)null;
            }
        }

        [ForeignKey("TransferProductId")]
        public virtual Product TransferProduct
        {
            get { return _transferProduct; }
            set
            {
                _transferProduct = value;
                TransferProductId = _transferProduct != null ? _transferProduct.ProductId : (int?)null;
            }
        }

        [ForeignKey("SwapCrossBalmoProductId")]
        public virtual Product SwapCrossBalmoProduct
        {
            get { return _swapCrossBalmoProduct; }
            set
            {
                _swapCrossBalmoProduct = value;
                SwapCrossBalmoProductId = _swapCrossBalmoProduct != null ? _swapCrossBalmoProduct.ProductId : (int?)null;
            }
        }

        public virtual ICollection<SwapCrossPerProduct> SwapCrossPerProducts { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<AdministrativeAlert> adm_alerts { get; set; }

        public virtual ICollection<UserProductCategoryPortfolio> UserPortfolios { get; set; }

        public virtual ICollection<Product> products1 { get; set; }

        public virtual ICollection<ProductClass> Classes { get; set; }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Unit return false.
            ProductCategory p = obj as ProductCategory;
            if ((System.Object)p == null)
            {
                return false;
            }

            return CategoryId == p.CategoryId;
        }

        public override int GetHashCode()
        {
            return CategoryId;
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
