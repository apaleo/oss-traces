using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Traces.Common.Utils;
using Traces.Data;
using Traces.Data.Entities;

namespace Traces.Core.Repositories
{
    public class TraceFileRepository : ITraceFileRepository
    {
        private readonly TracesDbContext _dbContext;

        public TraceFileRepository(TracesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(Expression<Func<TraceFile, bool>> predicate) =>
            await _dbContext.TraceFile.AnyAsync(predicate);

        public async Task<IReadOnlyList<TraceFile>> GetAllForTenantAsync() =>
            await _dbContext.TraceFile.ToListAsync();

        public async Task<IReadOnlyList<TraceFile>> GetAllTracesForTenantAsync(Expression<Func<TraceFile, bool>> expression) =>
            await _dbContext.TraceFile.Where(expression).ToListAsync();

        public async Task<TraceFile> GetAsync(int id) =>
            await _dbContext.TraceFile.FirstOrDefaultAsync(t => t.Id == id);

        public async Task<TraceFile> GetByPublicIdAsync(string publicId) =>
            await _dbContext.TraceFile.FirstOrDefaultAsync(t => t.PublicId.ToString() == publicId);

        public void Insert(TraceFile traceFile)
        {
            Check.NotNull(traceFile, nameof(traceFile));

            _dbContext.Add(traceFile);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbContext.TraceFile.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return false;
            }

            _dbContext.Remove(entity);

            return true;
        }

        public async Task SaveAsync() => await _dbContext.SaveChangesAsync();
    }
}