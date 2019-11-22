using System.Threading.Tasks;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public interface ITraceModifierService
    {
        Task<ResultModel<bool>> MarkTraceAsCompleteAsync(int id);

        Task<ResultModel<TraceItemModel>> CreateTraceAsync(CreateTraceItemModel createTraceItemModel);

        Task<ResultModel<bool>> ReplaceTraceAsync(ReplaceTraceItemModel replaceTraceItemModel);

        Task<ResultModel<bool>> DeleteTraceAsync(int id);
    }
}