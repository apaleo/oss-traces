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

        Task<IReadOnlyList<TraceDto>> GetActiveTracesAsync(DateTime from, DateTime to);

        Task<IReadOnlyList<TraceDto>> GetActiveTracesForPropertyAsync(string propertyId, DateTime from, DateTime to);

        Task<IReadOnlyList<TraceDto>> GetActiveTracesForReservationAsync(string reservationId, DateTime from, DateTime to);

        Task<IReadOnlyList<TraceDto>> GetOverdueTracesAsync();

        Task<IReadOnlyList<TraceDto>> GetOverdueTracesForPropertyAsync(string propertyId);

        Task<IReadOnlyList<TraceDto>> GetOverdueTracesForReservationAsync(string reservationId);

        Task<Option<TraceDto>> GetTraceAsync(int id);

        Task<int> CreateTraceAsync(CreateTraceDto createTraceDto);

        Task<int> CreateTraceFromReservationAsync(CreateTraceDto createTraceDto);

        Task<bool> ReplaceTraceAsync(int id, ReplaceTraceDto replaceTraceDto);

        Task<bool> CompleteTraceAsync(int id);

        Task<bool> RevertCompleteAsync(int id);

        Task<bool> DeleteTraceAsync(int id);
    }
}