using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Common.Utils;
using Traces.Core.Extensions.Files;
using Traces.Core.Models.Files;
using Traces.Core.Repositories;
using Traces.Data.Entities;

namespace Traces.Core.Services.Files
{
    public class TraceFileService : ITraceFileService
    {
        private readonly ITraceFileRepository _traceFileRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IRequestContext _requestContext;

        public TraceFileService(ITraceFileRepository traceFileRepository, IFileStorageService fileStorageService, IRequestContext requestContext)
        {
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
            _fileStorageService = Check.NotNull(fileStorageService, nameof(fileStorageService));
            _traceFileRepository = Check.NotNull(traceFileRepository, nameof(traceFileRepository));
        }

        public async Task<List<TraceFile>> UploadStorageFilesAsync(List<CreateTraceFileDto> files)
        {
            var traceFiles = new List<TraceFile>();

            if (!files.IsValid())
            {
                throw new BusinessValidationException(TextConstants.CreateTraceFileInvalidErrorMessage);
            }

            foreach (var createFile in files)
            {
                var traceFile = createFile.ToTraceFile(this._requestContext);
                traceFiles.Add(traceFile);
                await this._fileStorageService.CreateFileAsync(traceFile, createFile.Data);
            }

            return traceFiles;
        }

        public async Task<SavedFileDto> GetSavedFileFromPublicIdAsync(string publicId)
        {
            if (!await _traceFileRepository.ExistsAsync(t => t.PublicId.ToString() == publicId))
            {
                throw new BusinessValidationException(string.Format(TextConstants.TraceFilePublicIdCouldNotBeFoundErrorMessageFormat, publicId));
            }

            var traceFile = await _traceFileRepository.GetByPublicIdAsync(publicId);
            var data = await _fileStorageService.GetFileAsync(traceFile);

            return new SavedFileDto
            {
                TraceFile = traceFile.ToTraceFileDto(),
                Data = data
            };
        }

        public async Task<IReadOnlyList<TraceFile>> DeleteStorageFilesAsync(List<int> ids)
        {
            var traceFiles = await _traceFileRepository.GetAllTraceFilesForTenantAsync(traceFile => ids.Contains(traceFile.Id));
            if (traceFiles.Any())
            {
                await _fileStorageService.DeleteFileRangeAsync(traceFiles);
            }

            return traceFiles;
        }
    }
}
