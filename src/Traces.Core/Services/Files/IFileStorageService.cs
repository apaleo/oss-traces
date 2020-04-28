using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Traces.Data.Entities;

namespace Traces.Core.Services.Files
{
    public interface IFileStorageService
    {
        Task CreateFileAsync(TraceFile traceFile, MemoryStream data);

        Task<byte[]> GetFileAsync(TraceFile traceFile);

        Task DeleteFileRangeAsync(IReadOnlyList<TraceFile> traceFiles);
    }
}
