using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Traces.Data.Entities;

namespace Traces.Core.Repositories
{
    public interface ITraceFileRepository
    {
        Task<bool> ExistsAsync(Expression<Func<TraceFile, bool>> predicate);

        Task<IReadOnlyList<TraceFile>> GetAllForTenantAsync();

        Task<IReadOnlyList<TraceFile>> GetAllTraceFilesForTenantAsync(Expression<Func<TraceFile, bool>> expression);

        Task<TraceFile> GetAsync(int id);

        Task<TraceFile> GetByPublicIdAsync(string publicId);

        void Insert(TraceFile traceFile);

        Task<bool> DeleteRangeAsync(List<int> ids);

        Task SaveAsync();
    }
}
