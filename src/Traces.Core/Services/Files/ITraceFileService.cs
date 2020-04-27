using System.Collections.Generic;
using System.Threading.Tasks;
using Traces.Core.Models.Files;
using Traces.Data.Entities;

namespace Traces.Core.Services.Files
{
    public interface ITraceFileService
    {
        Task<List<TraceFile>> UploadStorageFilesAsync(List<CreateTraceFileDto> files);

        Task<SavedFileDto> GetSavedFileFromPublicIdAsync(string publicId);

        Task<IReadOnlyList<TraceFile>> DeleteStorageFilesAsync(List<int> ids);
    }
}
