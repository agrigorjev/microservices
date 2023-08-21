using Mandara.ProductService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mandara.ProductService.Data;

public class MandaraEntities : DbContext
{

    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<SecurityDefinition> SecurityDefinitions { get; set; }

    public MandaraEntities(DbContextOptions<MandaraEntities> options) : base(options)
    {

    }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<CalendarExpiryDate>()
    //        .HasKey(t => new { t.CalendarId, t.FuturesDate });

    //    modelBuilder.Entity<CalendarHoliday>().HasKey(t => new { t.CalendarId, t.HolidayDate });

    //    modelBuilder.Entity<OfficialProduct>()
    //        .HasMany(e => e.official_products1)
    //        .WithOne(e => e.official_products2)
    //        .HasForeignKey(e => e.SettlementProductId);

    //    modelBuilder.Entity<OfficialProduct>()
    //        .HasMany(e => e.Products)
    //        .WithOne(e => e.OfficialProduct)
    //        .IsRequired()
    //        .HasForeignKey(e => e.OfficialProductId)
    //        .OnDelete(DeleteBehavior.Restrict);

    //    modelBuilder.Entity<OfficialProduct>()
    //        .HasMany(e => e.MonthlyProducts)
    //        .WithOne(e => e.MonthlyOfficialProduct)
    //        .HasForeignKey(e => e.MonthlyOfficialProductId)
    //        .OnDelete(DeleteBehavior.Restrict);

    //    modelBuilder.Entity<OfficialProduct>()
    //        .HasMany(e => e.products1)
    //        .WithOne(e => e.TasOfficialProduct)
    //        .HasForeignKey(e => e.TasOfficialProductId);

    //    modelBuilder.Entity<OfficialProduct>()
    //        .HasOne(e => e.Currency)
    //        .WithMany()
    //        .IsRequired()
    //        .HasForeignKey(e => e.CurrencyId);

    //    modelBuilder.Entity<Portfolio>()
    //        .HasMany(e => e.PortfolioTrades)
    //        .WithOne(e => e.Portfolio)
    //        .IsRequired()
    //        .HasForeignKey(e => e.PortfolioId)
    //        .OnDelete(DeleteBehavior.Restrict);

    //    modelBuilder.Entity<Portfolio>()
    //        .HasMany(e => e.Portfolios)
    //        .WithOne(e => e.ParentPortfolio)
    //        .HasForeignKey(e => e.ParentId);

    //    modelBuilder.Entity<Portfolio>()
    //        .HasMany(e => e.trade_captures)
    //        .WithOne(e => e.BuyBook)
    //        .HasForeignKey(e => e.BuyBookID);

    //    modelBuilder.Entity<Portfolio>()
    //        .HasMany(e => e.trade_captures1)
    //        .WithOne(e => e.Portfolio)
    //        .HasForeignKey(e => e.PortfolioId);

    //    modelBuilder.Entity<Portfolio>()
    //        .HasMany(e => e.trade_captures2)
    //        .WithOne(e => e.SellBook)
    //        .HasForeignKey(e => e.SellBookID);

    //    modelBuilder.Entity<ProductCategory>()
    //        .HasMany(e => e.SwapCrossPerProducts)
    //        .WithOne(e => e.Category)
    //        .IsRequired()
    //        .HasForeignKey(e => e.CategoryId)
    //        .OnDelete(DeleteBehavior.Restrict);

    //    modelBuilder.Entity<ProductCategory>()
    //        .HasMany(e => e.Products)
    //        .WithOne(e => e.Category)
    //        .HasForeignKey(e => e.CategoryId);

    //    modelBuilder.Entity<ProductCategory>()
    //        .HasMany(e => e.products1)
    //        .WithOne(e => e.CategoryOverride)
    //        .HasForeignKey(e => e.CategoryOverrideId);

    //    modelBuilder.Entity<ProductCategory>()
    //        .HasMany<ProductClass>(cat => cat.Classes)
    //        .WithMany(prodClass => prodClass.Categories)
    //        .UsingEntity<Dictionary<string, object>>("product_category_class",
    //            e => e
    //                .HasOne<ProductClass>()
    //                .WithMany()
    //                .HasForeignKey("product_class_id"),
    //            e => e
    //                .HasOne<ProductCategory>()
    //                .WithMany()
    //                .HasForeignKey("category_id")
    //        );

    //    modelBuilder.Entity<Product>()
    //        .HasOne(e => e.ComplexProduct)
    //        .WithOne(e => e.Product)
    //        .HasForeignKey<ComplexProduct>(e => e.ProductId);




    //    modelBuilder.Entity<Product>()
    //        .HasMany(e => e.ComplexProductBalmos)
    //        .WithOne(e => e.BalmoOnComplexProduct)
    //        .HasForeignKey(e => e.BalmoOnComplexProductId);

    //    modelBuilder.Entity<Product>()
    //        .HasMany(e => e.CrudeSwapBalmos)
    //        .WithOne(e => e.BalmoOnCrudeProduct)
    //        .HasForeignKey(e => e.BalmoOnCrudeProductId);


    //    modelBuilder.Entity<Product>()
    //        .HasMany(e => e.security_definitions)
    //        .WithOne(e => e.Product)
    //        .IsRequired()
    //        .HasForeignKey(e => e.product_id)
    //        .OnDelete(DeleteBehavior.Restrict);

    //    modelBuilder.Entity<Product>()
    //        .HasOne(e => e.UnderlyingFuturesOverride)
    //        .WithMany(e => e.UnderlyingFuturesOverridesProducts)
    //        .HasForeignKey(e => e.UnderlyingFuturesOverrideId);

    //    modelBuilder.Entity<Product>()
    //        .HasOne(e => e.Unit)
    //        .WithMany()
    //        .HasForeignKey(e => e.UnitId);



    //    modelBuilder.Entity<Product>().Property(x => x.PositionFactor).HasPrecision(18, 8);
    //    modelBuilder.Entity<Product>().Property(x => x.PnlFactor).HasPrecision(18, 8);
    //    modelBuilder.Entity<Product>().Property(x => x.PriceConversionFactorDb).HasPrecision(18, 8);

    //    modelBuilder.Entity<ComplexProduct>().Property(x => x.ConversionFactor1).HasPrecision(18, 8);
    //    modelBuilder.Entity<ComplexProduct>().Property(x => x.ConversionFactor2).HasPrecision(18, 8);

    //}
}