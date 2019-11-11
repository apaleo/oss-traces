using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Traces.Common.Utils;
using Traces.Data;
using Traces.Data.Entities;

namespace Traces.Core.Repositories
{
    public class TraceRepository : ITraceRepository
    {
        private readonly TracesDbContext _dbContext;

        public TraceRepository(TracesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Trace, bool>> predicate) =>
            await _dbContext.Traces.AnyAsync(predicate);

        public async Task<IReadOnlyList<Trace>> GetAllForTenantAsync() =>
            await _dbContext.Traces.ToListAsync();

        public async Task<Trace> GetAsync(int id) =>
            await _dbContext.Traces.FirstOrDefaultAsync(t => t.Id == id);

        public void Insert(Trace trace)
        {
            Check.NotNull(trace, nameof(trace));

            _dbContext.Add(trace);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbContext.Traces.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return false;
            }

            _dbContext.Remove(entity);

            return true;
        }

        public async Task SaveAsync() =>
            await _dbContext.SaveChangesAsync();
    }
}