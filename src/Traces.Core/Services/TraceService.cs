using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Optional;
using Traces.Core.Models;

namespace Traces.Core.Services
{
    public class TraceService : ITraceService
    {
        public Task<IReadOnlyList<TraceDto>> GetTracesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Option<TraceDto>> GetTraceAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateTraceAsync(CreateTraceDto createTraceDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceTraceAsync(ReplaceTraceDto replaceTraceDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CompleteTraceAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTraceAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}