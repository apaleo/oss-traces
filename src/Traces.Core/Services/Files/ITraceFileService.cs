using System.Collections.Generic;
using System.Threading.Tasks;
using Traces.Core.Models.TraceFile;

namespace Traces.Core.Services.Files
{
    public interface ITraceFileService
    {
        Task<TraceFileDto> CreateTraceFileAsync(CreateTraceFileDto createTraceFileDto);

        Task<IReadOnlyList<TraceFileDto>> GetAllTraceFilesForTraceAsync(string id);
    }
}
