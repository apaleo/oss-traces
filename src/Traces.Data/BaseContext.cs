using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NodaTime.Extensions;
using Traces.Data.Entities;

namespace Traces.Data
{
    public abstract class BaseContext : DbContext
    {
        private const string CreatedUtcName = nameof(BaseEntity.CreatedUtc);
        private const string UpdatedUtcName = nameof(BaseEntity.UpdatedUtc);

        protected BaseContext(DbContextOptions options)
            : base(options)
        {
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

        protected virtual void PreSaveChanges()
        {
            ChangeTracker.DetectChanges();

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
    }
}