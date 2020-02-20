using System.Threading.Tasks;
using Traces.Core.Models.File;

namespace Traces.Core.Services.Files
{
    public interface ITraceFileService
    {
        Task<TraceFileDto> CreateTraceFileAsync(CreateTraceFileDto createTraceFileDto);

        Task<SavedFileDto> GetSavedFileFromPublicIdAsync(string publicId);
    }
}
