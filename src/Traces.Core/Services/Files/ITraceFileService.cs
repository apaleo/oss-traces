using System.Collections.Generic;
using System.Threading.Tasks;
using Traces.Core.Models.Files;

namespace Traces.Core.Services.Files
{
    public interface ITraceFileService
    {
        Task<IReadOnlyList<TraceFileDto>> CreateTraceFileAsync(IReadOnlyList<CreateTraceFileDto> createTraceFileDtos);

        Task<SavedFileDto> GetSavedFileFromPublicIdAsync(string publicId);

        Task<bool> DeleteTraceFileAsync(int id);

        Task<bool> DeleteTraceFileByTraceIdAsync(int traceId);
    }
}
