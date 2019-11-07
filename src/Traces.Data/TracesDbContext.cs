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
                optionsBuilder.UseNpgsql(string.Empty);
            }
        }
    }
}