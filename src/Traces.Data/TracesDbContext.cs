using Microsoft.EntityFrameworkCore;
using Traces.Common;
using Traces.Data.Entities;

namespace Traces.Data
{
    public class TracesDbContext : BaseContext
    {
        public TracesDbContext(DbContextOptions options, IRequestContext requestContext)
            : base(options, requestContext)
        {
        }

        public DbSet<Trace> Trace { get; set; }

        public DbSet<TraceFile> TraceFile { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trace>()
                .Property(t => t.Id)
                .UseHiLo();

            modelBuilder.Entity<TraceFile>()
                .Property(t => t.Id)
                .UseHiLo();

            base.OnModelCreating(modelBuilder);
        }
    }
}