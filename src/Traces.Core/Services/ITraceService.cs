using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Optional;
using Traces.Core.Models;

namespace Traces.Core.Services
{
    public interface ITraceService
    {
        Task<IReadOnlyList<TraceDto>> GetTracesAsync();

        Task<Option<TraceDto>> GetTraceAsync(Guid id);

        Task<Guid> CreateTraceAsync(CreateTraceDto createTraceDto);

        Task<bool> ReplaceTraceAsync(Guid id, ReplaceTraceDto replaceTraceDto);

        Task<bool> CompleteTraceAsync(Guid id);

        Task<bool> DeleteTraceAsync(Guid id);
    }
}