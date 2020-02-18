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

        Task<IReadOnlyList<TraceDto>> GetActiveTracesAsync(DateTime from, DateTime toDateTime);

        Task<IReadOnlyList<TraceDto>> GetActiveTracesForPropertyAsync(string propertyId, DateTime from, DateTime toDateTime);

        Task<IReadOnlyList<TraceDto>> GetOverdueTracesAsync();

        Task<IReadOnlyList<TraceDto>> GetOverdueTracesForPropertyAsync(string propertyId);

        Task<IReadOnlyList<TraceDto>> GetAllTracesForReservationAsync(string reservationId);

        Task<Option<TraceDto>> GetTraceAsync(int id);

        Task<TraceDto> CreateTraceAsync(CreateTraceDto createTraceDto);

        Task<TraceDto> CreateTraceFromReservationAsync(CreateTraceDto createTraceDto);

        Task<bool> ReplaceTraceAsync(int id, ReplaceTraceDto replaceTraceDto);

        Task<bool> CompleteTraceAsync(int id);

        Task<bool> RevertCompleteAsync(int id);

        Task<bool> DeleteTraceAsync(int id);
    }
}