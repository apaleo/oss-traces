using System.Collections.Generic;
using System.Threading.Tasks;
using Traces.Web.Models;
using Traces.Web.Models.Files;

namespace Traces.Web.Services
{
    public interface IFileService
    {
        Task<ResultModel<IReadOnlyList<TraceFileItemModel>>> CreateTraceFileAsync(IReadOnlyList<CreateTraceFileItemModel> createTraceFileItemModels);

        Task<ResultModel<SavedFileItemModel>> GetSavedFileFromPublicIdAsync(string publicId);

        Task<ResultModel<bool>> DeleteTraceFileRangeAsync(IReadOnlyList<int> ids);
    }
}
