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
                PublicId = Guid.NewGuid(),
                FileGuid = fileGuid
            };

            _traceFileRepository.Insert(traceFile);

            await _traceFileRepository.SaveAsync();

            var newTraceFile = await _traceFileRepository.GetAsync(traceFile.Id);

            await CreateFileAsync(newTraceFile, createTraceFileDto.Data);

            return newTraceFile.ToTraceFileDto();
        }

        public async Task<SavedFileDto> GetSavedFileFromPublicIdAsync(string publicId)
        {
            if (!await _traceFileRepository.ExistsAsync(t => t.PublicId.ToString() == publicId))
            {
                throw new BusinessValidationException(string.Format(TextConstants.TraceFilePublicIdCouldNotBeFoundErrorMessageFormat, publicId));
            }

            var traceFile = await _traceFileRepository.GetByPublicIdAsync(publicId);

            return new SavedFileDto
            {
                TraceFile = traceFile.ToTraceFileDto(),
                Data = File.ReadAllBytes(traceFile.Path)
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
                    DeleteFileRange(traceFiles);
                    await _traceFileRepository.SaveAsync();
                }

                return deleted;
            }

            return true;
        }

        private static async Task CreateFileAsync(TraceFile traceFile, MemoryStream data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(traceFile.Path));

            using (var fileStream = new FileStream(traceFile.Path, FileMode.Create))
            {
                await data.CopyToAsync(fileStream);
            }
        }

        private static void DeleteFile(TraceFile traceFile)
        {
            var directoryPath = Path.GetDirectoryName(traceFile.Path);

            Directory.Delete(directoryPath, true);
        }

        private static void DeleteFileRange(IReadOnlyList<TraceFile> traceFiles)
        {
            foreach (var traceFile in traceFiles)
            {
                DeleteFile(traceFile);
            }
        }
    }
}
