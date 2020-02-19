using System.Threading.Tasks;
using Traces.Web.Models;
using Traces.Web.Models.TraceFile;

namespace Traces.Web.Services
{
    public interface IFileService
    {
        Task<ResultModel<TraceFileItemModel>> CreateTraceFileAsync(CreateTraceFileItemModel createTraceFileItemModel);
    }
}
