using System.Threading.Tasks;
using Traces.Web.Models;
using Traces.Web.Models.Files;

namespace Traces.Web.Services
{
    public interface IFileService
    {
        Task<ResultModel<SavedFileItemModel>> GetSavedFileFromPublicIdAsync(string publicId);
    }
}
