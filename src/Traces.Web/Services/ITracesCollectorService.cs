using System.Collections.Generic;
using System.Threading.Tasks;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public interface ITracesCollectorService
    {
        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesAsync();

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesAsyn();

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesForPropertyAsync(string propertyIde);

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesForPropertyAsync(string propertyId);

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesForReservationAsync(string reservationId);

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesForReservationAsync(string reservationId);
    }
}