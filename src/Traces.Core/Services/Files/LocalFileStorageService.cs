using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Data.Entities;

namespace Traces.Core.Services.Files
{
    public class LocalFileStorageService : IFileStorageService
    {
        public async Task CreateFileAsync(TraceFile traceFile, MemoryStream data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(traceFile.Path));

            using (var fileStream = new FileStream(traceFile.Path, FileMode.Create))
            {
                await data.CopyToAsync(fileStream);
            }
        }

        public async Task<MemoryStream> GetFileAsync(TraceFile traceFile)
        {
            var memoryStream = new MemoryStream();

            try
            {
                using (var fileStream = File.OpenRead(traceFile.Path))
                {
                    await fileStream.CopyToAsync(memoryStream);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new BusinessValidationException(TextConstants.FileGetExceptionMessage, ex);
            }

            return memoryStream;
        }

        public Task DeleteFileRangeAsync(IReadOnlyList<TraceFile> traceFiles)
        {
            try
            {
                foreach (var traceFile in traceFiles)
                {
                    var directoryPath = Path.GetDirectoryName(traceFile.Path);

                    Directory.Delete(directoryPath, true);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new BusinessValidationException(TextConstants.FileDeleteExceptionMessage, ex);
            }

            return Task.CompletedTask;
        }
    }
}
