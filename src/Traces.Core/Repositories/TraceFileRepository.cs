using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IReadOnlyList<TraceFile>> GetAllTraceFilesForTenantAsync(Expression<Func<TraceFile, bool>> expression) =>
            await _dbContext.TraceFile.Where(expression).ToListAsync();

        public async Task<TraceFile> GetByPublicIdAsync(string publicId) =>
            await _dbContext.TraceFile.FirstOrDefaultAsync(t => t.PublicId.ToString() == publicId);
    }
}