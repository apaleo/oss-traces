using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Traces.Common;

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

            return new TracesDbContext(optionsBuilder.Options, new NoRequestContext());
        }

        private class NoRequestContext : IRequestContext
        {
            public string AccessToken => throw new InvalidOperationException("No Request Context has been initialized");

            public string TenantId => throw new InvalidOperationException("No Request Context has been initialized");

            public string SubjectId => throw new InvalidOperationException("No Request Context has been initialized");

            public bool IsInitialized => throw new InvalidOperationException("No Request Context has been initialized");

            public void Initialize(string tenantId, string subjectId) => throw new NotImplementedException();

            public void InitializeOrUpdateAccessToken(string accessToken) => throw new NotImplementedException();
        }
    }
}