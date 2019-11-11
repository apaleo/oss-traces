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
                // todo: Setup dbConnection string
                optionsBuilder.UseNpgsql(string.Empty);
            }
        }
    }
}