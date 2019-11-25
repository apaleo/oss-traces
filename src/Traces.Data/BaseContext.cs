using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NodaTime.Extensions;
using Traces.Common;
using Traces.Common.Utils;
using Traces.Data.Entities;

namespace Traces.Data
{
    public abstract class BaseContext : DbContext
    {
        private const string CreatedUtcName = nameof(BaseEntity.CreatedUtc);
        private const string UpdatedUtcName = nameof(BaseEntity.UpdatedUtc);

        private static readonly ConcurrentDictionary<Type, MethodInfo> CachedGenericAccountFilterMethods = new ConcurrentDictionary<Type, MethodInfo>();

        private static readonly MethodInfo CachedSetAccountFilterMethod =
            typeof(BaseContext).GetMethod(nameof(SetAccountFilter), BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly IRequestContext _requestContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestContext"/> class. Should only be
        /// used for ef migrations.
        /// </summary>
        /// <param name="options">Pass through dependency on DbContextOptions.</param>
        protected BaseContext(DbContextOptions options)
            : base(options)
        {
            _requestContext = new NoRequestContext();
        }

        protected BaseContext(DbContextOptions options, IRequestContext requestContext)
            : base(options)
        {
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
        }

        public override int SaveChanges()
        {
            PreSaveChanges();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            PreSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            PreSaveChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            PreSaveChanges();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureAccountGlobalFilter(modelBuilder);
        }

        protected virtual void PreSaveChanges()
        {
            ChangeTracker.DetectChanges();

            SetAccountCode();
            SetTrackingFields();
        }

        protected virtual void SetTrackingFields()
        {
            var now = DateTime.UtcNow.ToInstant();

            IEnumerable<EntityEntry> entities = ChangeTracker.Entries<BaseEntity>();

            var addedEntities = entities.Where(e => e.State == EntityState.Added);

            foreach (var entry in addedEntities)
            {
                entry.Property(CreatedUtcName).CurrentValue = now;
                entry.Property(UpdatedUtcName).CurrentValue = now;
            }

            var modifiedEntities = entities.Where(e => e.State == EntityState.Modified);

            foreach (var entry in modifiedEntities)
            {
                entry.Property(CreatedUtcName).IsModified = false;
                entry.Property(UpdatedUtcName).CurrentValue = now;
            }
        }

        protected virtual void SetAccountCode()
        {
            const string propertyName = nameof(BaseEntity.TenantId);

            var entities = ChangeTracker.Entries<BaseEntity>();

            var addedEntities = entities
                .Where(x =>
                    x.State == EntityState.Added &&
                    x.Property(propertyName).Metadata.ClrType == typeof(string) &&
                    x.Property(propertyName).CurrentValue == null);

            if (addedEntities.Any())
            {
                foreach (EntityEntry entry in addedEntities)
                {
                    entry.Property(propertyName).CurrentValue = _requestContext.TenantId;
                }
            }

            IEnumerable<EntityEntry> modifiedEntities =
                entities.Where(x => x.State == EntityState.Modified && x.Property(propertyName).IsModified);

            if (modifiedEntities.Any())
            {
                throw new InvalidOperationException($"Attempted to change {propertyName} of existing entity.");
            }
        }

        protected virtual void ConfigureAccountGlobalFilter(ModelBuilder modelBuilder)
        {
            var allTables = modelBuilder.Model.GetEntityTypes().Select(e => e.ClrType).ToList();
            var tablesWithTenantSeparation = allTables.Where(typeof(BaseEntity).IsAssignableFrom).ToList();
            var tablesWithoutMandatoryTenantSeparation = allTables.Except(tablesWithTenantSeparation);
            var allowedTablesWithoutTenantSeparation = WhiteListedEntitiesWithoutAccountCode().ToImmutableHashSet();

            foreach (var table in tablesWithTenantSeparation)
            {
                CachedGenericAccountFilterMethods.GetOrAdd(
                    table,
                    t => CachedSetAccountFilterMethod.MakeGenericMethod(t));
            }

            foreach (var table in tablesWithoutMandatoryTenantSeparation)
            {
                if (!allowedTablesWithoutTenantSeparation.Contains(table))
                {
                    throw new InvalidOperationException(
                        $"Type {table.FullName} neither derives from {nameof(BaseEntity)} nor is a white listed type according to {nameof(WhiteListedEntitiesWithoutAccountCode)}");
                }
            }

            foreach (var method in CachedGenericAccountFilterMethods.Values)
            {
                method.Invoke(this, new object[] { modelBuilder });
            }
        }

        /// <summary>
        /// Overwrite if you have entities which don't need to derive from
        /// BaseTrackedEntityWithAccountCode Will be checked at runtime that all entities are either
        /// in this set, or derive from BaseTrackedEntityWithAccountCode
        /// </summary>
        protected virtual IEnumerable<Type> WhiteListedEntitiesWithoutAccountCode() => Enumerable.Empty<Type>();

        /// <summary>
        /// Because we are referencing global properties the expression will be evaluated with
        /// correct values for every query
        /// </summary>
        /// <param name="modelBuilder">modelBuilder from OnModelCreating method</param>
        private void SetAccountFilter<T>(ModelBuilder modelBuilder)
            where T : BaseEntity =>
            modelBuilder.Entity<T>().HasQueryFilter(q => q.TenantId == _requestContext.TenantId);

        private class NoRequestContext : IRequestContext
        {
            public string AccessToken => throw new InvalidOperationException("No Request Context has been initialized");

            public string TenantId => throw new InvalidOperationException("No Request Context has been initialized");

            public string SubjectId => throw new InvalidOperationException("No Request Context has been initialized");

            public void Initialize(string tenantId, string accessToken, string subjectId) => throw new NotImplementedException();
        }
    }
}