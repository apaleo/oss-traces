using Microsoft.EntityFrameworkCore;
using Traces.Common;
using Traces.Data.Entities;

namespace Traces.Data
{
    public class TracesDbContext : BaseContext
    {
        public TracesDbContext()
            : base()
        {
        }

        public TracesDbContext(DbContextOptions options, IRequestContext requestContext)
            : base(options, requestContext)
        {
        }

        public DbSet<Trace> Traces { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder
                .Entity<Trace>()
                .HasIndex(x => new { x.EntityId, x.TenantId })
                .IsUnique();

            modelBuilder
                .Entity<Trace>()
                .Property(x => x.EntityId).HasDefaultValueSql("uuid_generate_v4()");

            base.OnModelCreating(modelBuilder);
        }
    }
}