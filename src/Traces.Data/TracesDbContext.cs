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

        public DbSet<Trace> Trace { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trace>()
                .Property(t => t.Id)
                .UseHiLo();

            base.OnModelCreating(modelBuilder);
        }
    }
}