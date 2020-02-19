using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Common.Utils;
using Traces.Core.Models.TraceFile;
using Traces.Core.Repositories;
using Traces.Data.Entities;

namespace Traces.Core.Services.Files
{
    public class TraceFileService : ITraceFileService
    {
        private readonly ITraceFileRepository _traceFileRepository;
        private readonly IRequestContext _requestContext;

        public TraceFileService(ITraceFileRepository traceFileRepository, IRequestContext requestContext)
        {
            _traceFileRepository = Check.NotNull(traceFileRepository, nameof(traceFileRepository));
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
        }

        public async Task<TraceFileDto> CreateTraceFileAsync(CreateTraceFileDto createTraceFileDto)
        {
            Check.NotNull(createTraceFileDto, nameof(createTraceFileDto));

            if (string.IsNullOrWhiteSpace(createTraceFileDto.Name) ||
                string.IsNullOrWhiteSpace(createTraceFileDto.MimeType) ||
                createTraceFileDto.Size <= 0 || createTraceFileDto.Size > 2097152 ||
                createTraceFileDto.TraceId <= 0
            )
            {
                throw new BusinessValidationException("Error create trace file" + TextConstants.CreateTraceWithoutTitleOrFutureDateErrorMessage);
            }

            var tenantId = _requestContext.TenantId;
            var fileGuid = Guid.NewGuid();
            var path = $"/files/{tenantId}/{fileGuid}/{createTraceFileDto.Name}";
            var currentSubjectId = _requestContext.SubjectId;

            var traceFile = new TraceFile
            {
                Name = createTraceFileDto.Name,
                Size = createTraceFileDto.Size,
                CreatedBy = currentSubjectId,
                MimeType = createTraceFileDto.MimeType,
                TraceId = createTraceFileDto.TraceId,
                Path = path,
                PublicId = Guid.NewGuid(),
                FileGuid = fileGuid
            };

            _traceFileRepository.Insert(traceFile);

            await _traceFileRepository.SaveAsync();

            return TraceFileToDto(await _traceFileRepository.GetAsync(traceFile.Id));
        }

        public Task<IReadOnlyList<TraceFileDto>> GetAllTraceFilesForTraceAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public static TraceFileDto TraceFileToDto(TraceFile traceFile) => new TraceFileDto
        {
            Id = traceFile.Id,
            Name = traceFile.Name,
            Path = traceFile.Path,
            Size = traceFile.Size,
            Trace = TraceService.TraceToDto(traceFile.Trace),
            CreatedBy = traceFile.CreatedBy,
            MimeType = traceFile.MimeType,
            PublicId = traceFile.PublicId,
            FileGuid = traceFile.FileGuid
        };
    }
}
