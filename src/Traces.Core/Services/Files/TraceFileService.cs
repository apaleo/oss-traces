using System;
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
        private readonly IRequestContext _requestContext;
        private readonly IFileStorageService _fileStorageService;

        public TraceFileService(ITraceFileRepository traceFileRepository, IRequestContext requestContext, IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
            _traceFileRepository = Check.NotNull(traceFileRepository, nameof(traceFileRepository));
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
        }

        public async Task<IReadOnlyList<TraceFileDto>> CreateTraceFileAsync(List<CreateTraceFileDto> createTraceFileDtos)
        {
            Check.NotNull(createTraceFileDtos, nameof(createTraceFileDtos));

            foreach (var createTraceFileDto in createTraceFileDtos)
            {
                if (!createTraceFileDto.IsValid())
                {
                    throw new BusinessValidationException(TextConstants.CreateTraceFileInvalidErrorMessage);
                }
            }

            List<TraceFileDto> dtos = new List<TraceFileDto>();

            foreach (var createTraceFileDto in createTraceFileDtos)
            {
                var traceFileDto = await CreateTraceFileAsync(createTraceFileDto);
                dtos.Add(traceFileDto);
            }

            return dtos.ToList();
        }

        public async Task<TraceFileDto> CreateTraceFileAsync(CreateTraceFileDto createTraceFileDto)
        {
            Check.NotNull(createTraceFileDto, nameof(createTraceFileDto));

            if (!createTraceFileDto.IsValid())
            {
                throw new BusinessValidationException(TextConstants.CreateTraceFileInvalidErrorMessage);
            }

            var tenantId = _requestContext.TenantId;
            var fileGuid = Guid.NewGuid();
            var currentSubjectId = _requestContext.SubjectId;
            var path = $"files/{tenantId}/{fileGuid}/{createTraceFileDto.Name}";

            var traceFile = new TraceFile
            {
                Name = createTraceFileDto.Name,
                Size = createTraceFileDto.Size,
                CreatedBy = currentSubjectId,
                MimeType = createTraceFileDto.MimeType,
                TraceId = createTraceFileDto.TraceId,
                Path = path,
                PublicId = Guid.NewGuid()
            };

            await _fileStorageService.CreateFileAsync(traceFile, createTraceFileDto.Data);

            _traceFileRepository.Insert(traceFile);

            await _traceFileRepository.SaveAsync();

            var newTraceFile = await _traceFileRepository.GetAsync(traceFile.Id);

            return newTraceFile.ToTraceFileDto();
        }

        public async Task<SavedFileDto> GetSavedFileFromPublicIdAsync(string publicId)
        {
            if (!await _traceFileRepository.ExistsAsync(t => t.PublicId.ToString() == publicId))
            {
                throw new BusinessValidationException(string.Format(TextConstants.TraceFilePublicIdCouldNotBeFoundErrorMessageFormat, publicId));
            }

            var traceFile = await _traceFileRepository.GetByPublicIdAsync(publicId);
            var memoryStream = await _fileStorageService.GetFileAsync(traceFile);
            var data = memoryStream.ToArray();
            await memoryStream.DisposeAsync();

            return new SavedFileDto
            {
                TraceFile = traceFile.ToTraceFileDto(),
                Data = data
            };
        }

        public async Task<bool> DeleteTraceFileRangeAsync(List<int> ids)
        {
            var traceFiles = await _traceFileRepository.GetAllTraceFilesForTenantAsync(traceFile => ids.Contains(traceFile.Id));
            if (traceFiles.Any())
            {
                await _fileStorageService.DeleteFileRangeAsync(traceFiles);
                var deleted = await _traceFileRepository.DeleteRangeAsync(ids);

                if (deleted)
                {
                    await _traceFileRepository.SaveAsync();
                }

                return deleted;
            }

            return true;
        }
    }
}
