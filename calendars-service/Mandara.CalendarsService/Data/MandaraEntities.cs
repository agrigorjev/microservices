using Microsoft.EntityFrameworkCore;

namespace Mandara.CalendarsService.Data;

public class MandaraEntities : DbContext
{
    public virtual DbSet<Portfolio> Portfolios { get; set; }

    public MandaraEntities(DbContextOptions<MandaraEntities> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Portfolio>()
            .HasMany(e => e.Portfolios)
            .WithOne(e => e.ParentPortfolio)
            .HasForeignKey(e => e.ParentId);
    }
}