using Microsoft.EntityFrameworkCore;

namespace Mandara.CalendarsService.Data;

public class MandaraEntities : DbContext
{
    public virtual DbSet<StockCalendar> StockCalendars { get; set; }
    public virtual DbSet<CalendarExpiryDate> CalendarExpiryDates { get; set; }
    public virtual DbSet<CalendarHoliday> CalendarHolidays { get; set; }

    public MandaraEntities(DbContextOptions<MandaraEntities> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalendarExpiryDate>()
            .HasKey(t => new { t.CalendarId, t.FuturesDate });

        modelBuilder.Entity<CalendarHoliday>().HasKey(t => new { t.CalendarId, t.HolidayDate });


        modelBuilder.Entity<StockCalendar>()
            .HasMany(e => e.FuturesExpiries)
            .WithOne(e => e.StockCalendar)
            .IsRequired()
            .HasForeignKey(e => e.CalendarId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockCalendar>()
            .HasMany(e => e.Holidays)
            .WithOne(e => e.StockCalendar)
            .IsRequired()
            .HasForeignKey(e => e.CalendarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}