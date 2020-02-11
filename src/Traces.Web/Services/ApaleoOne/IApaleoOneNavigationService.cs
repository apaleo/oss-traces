using System.Threading.Tasks;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public interface IApaleoOneNavigationService
    {
        Task<ResultModel<bool>> NavigateToReservationAsync(TraceItemModel traceItemModel);
    }
}