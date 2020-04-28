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

        public async Task<byte[]> GetFileAsync(TraceFile traceFile)
        {
            try
            {
                using (var fileStream = File.OpenRead(traceFile.Path))
                using (var ms = new MemoryStream())
                {
                    await fileStream.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new BusinessValidationException(TextConstants.FileGetExceptionMessage, ex);
            }
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
