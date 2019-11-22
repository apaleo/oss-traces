using Microsoft.EntityFrameworkCore;
using Traces.Data.Entities;

namespace Traces.Data
{
    public class TracesDbContext : BaseContext
    {
        public TracesDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Trace> Traces { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "Host=127.0.0.1;Database=Traces;Username=OpenTraces;Password=OpenTraces.2019",
                    npgSqlOption => npgSqlOption.UseNodaTime());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trace>()
                .Property(t => t.Id)
                .UseHiLo();

            base.OnModelCreating(modelBuilder);
        }
    }
}