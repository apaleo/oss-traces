using System;
using System.Collections.Generic;
using Traces.Common;
using Traces.Common.Constants;
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
                createTraceFileDto.TraceId < 0 ||
                createTraceFileDto.Data == null
            )
            {
                return false;
            }

            return true;
        }

        public static bool IsValid(this List<CreateTraceFileDto> dtos)
        {
            foreach (var createTraceFileDto in dtos)
            {
                if (!createTraceFileDto.IsValid())
                {
                    return false;
                }
            }

            return true;
        }

        public static TraceFile ToTraceFile(this CreateTraceFileDto dto, IRequestContext requestContext)
        {
            var tenantId = requestContext.TenantId;
            var fileGuid = Guid.NewGuid();
            var currentSubjectId = requestContext.SubjectId;
            var path = $"files/{tenantId}/{fileGuid}/{dto.Name}";

            return new TraceFile
            {
                Name = dto.Name,
                Size = dto.Size,
                CreatedBy = currentSubjectId,
                MimeType = dto.MimeType,
                TraceId = dto.TraceId,
                Path = path,
                PublicId = Guid.NewGuid()
            };
        }
    }
}
