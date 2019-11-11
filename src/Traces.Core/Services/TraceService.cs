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
            throw new System.NotImplementedException();
        }

        public Task<Option<TraceDto>> GetTraceAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Option<TraceDto>> CreateTraceAsync(CreateTraceDto createTraceDto)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ReplaceTraceAsync(int id, ReplaceTraceDto replaceTraceDto)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> CompleteTraceAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> RevertCompleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteTraceAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}