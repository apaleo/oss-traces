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

        Task<IReadOnlyList<TraceFile>> GetAllTracesForTenantAsync(Expression<Func<TraceFile, bool>> expression);

        Task<TraceFile> GetAsync(int id);

        void Insert(TraceFile traceFile);

        Task<bool> DeleteAsync(int id);

        Task SaveAsync();
    }
}
