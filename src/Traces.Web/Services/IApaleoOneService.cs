using System.Threading.Tasks;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public interface IApaleoOneService
    {
        Task<ResultModel<bool>> NavigateToReservation(TraceItemModel traceItemModel);
    }
}