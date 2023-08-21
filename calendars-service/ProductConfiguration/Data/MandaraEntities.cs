using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.ProductConfiguration.Data;

public class MandaraEntities : DbContext
{
    public virtual DbSet<OfficialProduct> OfficialProducts { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public MandaraEntities(DbContextOptions<MandaraEntities> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<OfficialProduct>()
                 .HasMany(e=>e.Products)
                 .WithOne(e => e.OfficialProduct).IsRequired()
                 .HasForeignKey(e => e.OfficialProductId);

        modelBuilder.Entity<ProductCategory>()
                 .HasMany(e => e.Products)
                 .WithOne(e => e.Category).IsRequired()
                 .HasForeignKey(e => e.CategoryId);
    }
}

[Table("official_products")]
public class OfficialProduct
{

    [Column("official_product_id")]
    [Key]
    public int OfficialProductId { get; set; }

    [Column("official_name")]
    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Column("display_name")]
    [StringLength(255)]
    public string? DisplayName { get; set; }

    [Column("price_mapping_column")]
    [StringLength(50)]
    public string? PriceMappingColumn { get; set; }

    [NotMapped]
    public string? OldPriceMappingColumn { get; set; }

    [NotMapped]
    public int? Priority { get; set; }

    [NotMapped]
    public string? Abbreviation { get; set; }

    [NotMapped]
    public decimal? ConversionFactor { get; set; }

    [NotMapped]
    public Boolean? IsDaily { get; set; }

    [NotMapped]
    public int? HolidayCalendarId { get; set; }

    [NotMapped]
    public int? CategoryId { get; set; }

    public virtual ICollection<Product> Products { get; set; }

}

[Table("products")]
public class Product
{

    [Column("product_id")]
    [Key]
    public int ProductId { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("holidays_calendar_id")]
    public int? HolidaysCalendarId { get; set; }

    [Column("official_product_id")]
    public int OfficialProductId { get; set; }

    [Column("pnl_conversion")]
    public decimal? PnlFactor { get; set; }

    [Column("position_convertion")]
    public decimal? PositionFactor { get; set; }

    private OfficialProduct _officialProduct;

    [ForeignKey("OfficialProductId")]
    public virtual OfficialProduct OfficialProduct
    {
        get => _officialProduct;
        set
        {
            _officialProduct = value;
            OfficialProductId = _officialProduct?.OfficialProductId ?? 0;
        }
    }

    private ProductCategory _category;

    [ForeignKey("CategoryId")]
    public virtual ProductCategory Category
    {
        get => _category;
        set
        {
            _category = value;
            CategoryId = _category?.CategoryId;
        }
    }
}

[Table("product_categories")]
public partial class ProductCategory
{
 
    [Column("category_id")]
    [Key]
    public int CategoryId { get; set; }

    [Column("category_name")]
    [Required]
    [StringLength(50)]
    public string Name { get; set; }


    [Column("abbreviation")]
    [StringLength(50)]
    public string? Abbreviation { get; set; }

    [NotMapped]
    public int? ExpiryCalendarId { get;set; }

    [NotMapped]
    public decimal? PnlFactor { get; set; }

    [NotMapped]
    public bool? Daily { get; set; }

    [NotMapped]
    public bool? SelectMonths { get; set; }

    public virtual ICollection<Product> Products { get; set; }

}