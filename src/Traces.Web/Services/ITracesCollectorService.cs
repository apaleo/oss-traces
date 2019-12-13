using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public interface ITracesCollectorService
    {
        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesAsync(DateTime from, DateTime to);

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesAsync();

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesForPropertyAsync(string propertyId, DateTime from, DateTime to);

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesForPropertyAsync(string propertyId);

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesForReservationAsync(string reservationId, DateTime from, DateTime to);

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesForReservationAsync(string reservationId);
    }
}