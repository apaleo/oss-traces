using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Traces.Data.Entities;

namespace Traces.Core.Repositories
{
    public interface ITraceRepository
    {
        Task<bool> ExistsAsync(Expression<Func<Trace, bool>> predicate);

        Task<IReadOnlyList<Trace>> GetAllForTenantAsync();

        Task<IReadOnlyList<Trace>> GetAllTracesForTenantAsync(Expression<Func<Trace, bool>> expression);

        Task<Trace> GetAsync(int id);

        void Insert(Trace trace);

        Task<bool> DeleteAsync(int id);

        Task SaveAsync();
    }
}