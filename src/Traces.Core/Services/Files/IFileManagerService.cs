using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Traces.Data.Entities;

namespace Traces.Core.Services.Files
{
    public interface IFileManagerService
    {
        Task CreateFileAsync(TraceFile traceFile, MemoryStream data);

        Task<MemoryStream> GetFileAsync(TraceFile traceFile);

        Task DeleteFileRangeAsync(IReadOnlyList<TraceFile> traceFiles);
    }
}
