using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DataImport.DataEntries
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

    }
}