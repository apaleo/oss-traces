using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Traces.Core.Repositories
{
    public interface ITraceRepository
    {
        Task<bool> ExistsAsync(Expression<Func<Trace, bool>> predicate);

        Task<IReadOnlyList<Trace>> GetAllForTenantAsync();

        Task<Trace> GetAsync(int id);

        void Insert(Trace trace);

        Task<bool> DeleteAsync(int id);

        Task SaveAsync();
    }
}