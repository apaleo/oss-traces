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

        Task<Option<TraceDto>> GetTraceAsync(int id);

        Task<Option<TraceDto>> CreateTraceAsync(CreateTraceDto createTraceDto);

        Task<bool> ReplaceTraceAsync(int id, ReplaceTraceDto replaceTraceDto);

        Task<bool> CompleteTraceAsync(int id);

        Task<bool> RevertCompleteAsync(int id);

        Task<bool> DeleteTraceAsync(int id);
    }
}