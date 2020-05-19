using System;
using System.Collections.Generic;
using Traces.Common.Constants;
using Traces.Common.Utils;
using Traces.Core.Models.Files;
using Traces.Data.Entities;

namespace Traces.Core.Extensions.Files
{
    public static class CreateTraceFileDtoExtensions
    {
        public static bool IsValid(this CreateTraceFileDto createTraceFileDto)
        {
            if (string.IsNullOrWhiteSpace(createTraceFileDto.Name) ||
                string.IsNullOrWhiteSpace(createTraceFileDto.MimeType) ||
                createTraceFileDto.Size <= 0 ||
                createTraceFileDto.Size > AppConstants.MaxFileSizeInBytes ||
                createTraceFileDto.Data == null
            )
            {
                return false;
            }

            return true;
        }

        public static bool IsValid(this List<CreateTraceFileDto> dtos) => dtos.TrueForAll(dto => dto.IsValid());

        public static TraceFile ToTraceFile(this CreateTraceFileDto dto, string subjectId, string path, Guid publicId)
        {
            Check.NotNull(dto, nameof(dto));

            return new TraceFile
            {
                Name = dto.Name,
                Size = dto.Size,
                CreatedBy = subjectId,
                MimeType = dto.MimeType,
                Path = path,
                PublicId = publicId
            };
        }
    }
}
