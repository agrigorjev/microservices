using DataEntries;
using Mandara.ProductService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DataEntries
{

    public class MandaraEntities : DbContext
    {

        public virtual DbSet<OfficialProduct> OfficialProducts { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<Region> Regions { get; set; }

        public virtual DbSet<Currency> Currencies { get; set; }

        public MandaraEntities(DbContextOptions<MandaraEntities> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<OfficialProduct>()
                .HasMany(e => e.official_products1)
                .WithOne(e => e.official_products2)
                .HasForeignKey(e => e.SettlementProductId);


            modelBuilder.Entity<OfficialProduct>()
                .HasOne(e => e.Currency)
                .WithMany()
                .IsRequired()
                .HasForeignKey(e => e.CurrencyId);




        }
    }
}