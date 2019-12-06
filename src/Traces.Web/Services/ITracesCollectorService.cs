using System.Collections.Generic;
using System.Threading.Tasks;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public interface ITracesCollectorService
    {
        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesAsync();

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesForPropertyAsync(string propertyIde);

        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesForReservationAsync(string reservationId);
    }
}