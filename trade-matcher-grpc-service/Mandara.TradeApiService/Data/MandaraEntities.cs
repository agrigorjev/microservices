using Mandara.TradeApiService.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.TradeApiService.Data;

public class MandaraEntities : DbContext
{
    public virtual DbSet<Portfolio> Portfolios { get; set; }
    public virtual DbSet<OfficialProduct> OfficialProducts { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductCategory> ProductCategories { get; set; }
    public virtual DbSet<Company> Companies { get; set; }
    public virtual DbSet<CompanyAlias> CompanyAlias { get; set; }
    public virtual DbSet<Exchange> Exchanges { get; set; }
    public virtual DbSet<TradeTemplate> TradeTemplates { get; set; }
    public virtual DbSet<Unit> Units { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<TradeCapture> TradeCaptures { get; set; }
    public virtual DbSet<FxTrade> FxTrades { get; set; }
    public virtual DbSet<SecurityDefinition> SecurityDefinitions { get; set; }


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

        modelBuilder.Entity<Portfolio>()
            .HasMany(e => e.Users)
            .WithOne(e => e.Portfolio)
            .HasForeignKey(e => e.DefaultPortfolioId);

        modelBuilder.Entity<Portfolio>()
                .HasMany(e => e.Portfolios)
                .WithOne(e => e.ParentPortfolio)
                .HasForeignKey(e => e.ParentId);

        modelBuilder.Entity<Company>()
                .HasMany(e => e.CompanyAliases)
                .WithOne(e => e.Company).IsRequired()
                .HasForeignKey(e => e.CompanyId);

        modelBuilder.Entity<TradeTemplate>()
                .HasOne(e => e.Unit)
                .WithMany()
                .HasForeignKey(e => e.UnitId);

        modelBuilder.Entity<SecurityDefinition>()
                .HasMany(e => e.TradeCaptures)
                .WithOne(e => e.SecurityDefinition).IsRequired()
                .HasForeignKey(e => e.SecurityDefinitionId);

        modelBuilder.Entity<FxTrade>()
                .HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId);

        modelBuilder.Entity<Exchange>()
                .HasOne(e => e.Calendar)
                .WithMany()
                .HasForeignKey(e => e.CalendarId);
       
        modelBuilder.Entity<StockCalendar>()
                .HasMany(e => e.Products)
                .WithOne()
                .HasForeignKey(e => e.calendar_id);
    }
}

[Table("official_products")]
public class OfficialProduct
{
    public const int DefaultId = -1;
    public const string DefaultName = "DefaultOfficialProduct";

    public static readonly OfficialProduct Default = new OfficialProduct()
    {
        OfficialProductId = DefaultId,
        Name = DefaultName,

    };

    [NotMapped]
    public bool IsDefault => DefaultId == OfficialProductId &&
                         DefaultName == Name;


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

    [Column("is_allowed_for_manual_trades")]
    public bool? IsAllowedForManualTradesDb { get; set; }

    [NotMapped]
    public bool IsAllowedForManualTrades
    {
        get => IsAllowedForManualTradesDb ?? true;
    }

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

    [Column("currency_id")]
    public int CurrencyId { get; set; }

    private Currency _currency;

    [ForeignKey("CurrencyId")]
    public Currency Currency
    {
        get => _currency;
        set
        {
            _currency = value ?? Currency.Default;
            CurrencyId = _currency.CurrencyId;
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