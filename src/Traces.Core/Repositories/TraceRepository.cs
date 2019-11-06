using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Traces.Core.Repositories
{
    public class TraceRepository : ITraceRepository
    {
        public Task<bool> ExistsAsync(Expression<Func<Trace, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Trace>> GetAllForTenantAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Trace> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Insert(Trace trace)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}