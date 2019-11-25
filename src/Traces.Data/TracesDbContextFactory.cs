using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Traces.Data
{
    public class TracesDbContextFactory : IDesignTimeDbContextFactory<TracesDbContext>
    {
        public TracesDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TracesDbContext>();
            optionsBuilder.UseNpgsql(
                "Host=127.0.0.1;Database=Traces;Username=OpenTraces;Password=OpenTraces.2019",
                npgSqlOptions => npgSqlOptions.UseNodaTime());

            return new TracesDbContext(optionsBuilder.Options);
        }
    }
}