using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Common.Utils;
using Traces.Core.Extensions;
using Traces.Core.Models.Files;
using Traces.Core.Repositories;
using Traces.Core.Validators;
using Traces.Data.Entities;

namespace Traces.Core.Services.Files
{
    public class TraceFileService : ITraceFileService
    {
        private readonly ITraceFileRepository _traceFileRepository;
        private readonly IRequestContext _requestContext;
        private readonly IFileManagerService _fileManagerService;

        public TraceFileService(ITraceFileRepository traceFileRepository, IRequestContext requestContext, IFileManagerService fileManagerService)
        {
            _fileManagerService = fileManagerService;
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
            var path = $"./files/{tenantId}/{fileGuid}/{createTraceFileDto.Name}";

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

            await _fileManagerService.CreateFileAsync(traceFile, createTraceFileDto.Data);

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
            var memoryStream = await _fileManagerService.GetFileAsync(traceFile);

            return new SavedFileDto
            {
                TraceFile = traceFile.ToTraceFileDto(),
                Data = memoryStream.ToArray()
            };
        }

        public async Task<bool> DeleteTraceFileRangeAsync(Expression<Func<TraceFile, bool>> expression)
        {
            var traceFiles = await _traceFileRepository.GetAllTraceFilesForTenantAsync(expression);
            if (traceFiles.Any())
            {
                var deleted = await _traceFileRepository.DeleteRangeAsync(expression);

                if (deleted)
                {
                    await _fileManagerService.DeleteFileRangeAsync(traceFiles);
                    await _traceFileRepository.SaveAsync();
                }

                return deleted;
            }

            return true;
        }
    }
}
