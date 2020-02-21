using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Common.Utils;
using Traces.Core.Converters;
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

        public TraceFileService(ITraceFileRepository traceFileRepository, IRequestContext requestContext)
        {
            _traceFileRepository = Check.NotNull(traceFileRepository, nameof(traceFileRepository));
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
        }

        public async Task<IReadOnlyList<TraceFileDto>> CreateTraceFileAsync(IReadOnlyList<CreateTraceFileDto> createTraceFileDtos)
        {
            Check.NotNull(createTraceFileDtos, nameof(createTraceFileDtos));

            foreach (var createTraceFileDto in createTraceFileDtos)
            {
                if (!createTraceFileDto.IsValid())
                {
                    throw new BusinessValidationException("Error create trace file" + TextConstants.CreateTraceWithoutTitleOrFutureDateErrorMessage);
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
                throw new BusinessValidationException("Error create trace file" + TextConstants.CreateTraceWithoutTitleOrFutureDateErrorMessage);
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
                PublicId = Guid.NewGuid(),
                FileGuid = fileGuid
            };

            _traceFileRepository.Insert(traceFile);

            await _traceFileRepository.SaveAsync();

            var newTraceFile = await _traceFileRepository.GetAsync(traceFile.Id);

            await CreateFileAsync(newTraceFile, createTraceFileDto.Data);

            return newTraceFile.ConvertToDto();
        }

        public async Task<SavedFileDto> GetSavedFileFromPublicIdAsync(string publicId)
        {
            if (!await _traceFileRepository.ExistsAsync(t => t.PublicId.ToString() == publicId))
            {
                throw new BusinessValidationException("Error" + TextConstants.CreateTraceWithoutTitleOrFutureDateErrorMessage);
            }

            var traceFile = await _traceFileRepository.GetByPublicIdAsync(publicId);

            return new SavedFileDto
            {
                TraceFile = traceFile.ConvertToDto(),
                Data = File.ReadAllBytes(traceFile.Path)
            };
        }

        private static async Task CreateFileAsync(TraceFile traceFile, MemoryStream data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(traceFile.Path));

            using (var fileStream = new FileStream(traceFile.Path, FileMode.Create))
            {
                await data.CopyToAsync(fileStream);
            }
        }
    }
}
