using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public interface ITracesCollectorService
    {
        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetActiveTracesAsync(DateTime from, DateTime toDateTime);

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesAsync();

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetActiveTracesForPropertyAsync(string propertyId, DateTime from, DateTime toDateTime);

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesForPropertyAsync(string propertyId);

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetAllTracesForReservationAsync(string reservationId);
    }
}