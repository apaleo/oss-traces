using System.Collections.Generic;
using System.Linq;
using Traces.Core.Models.Files;
using Traces.Data.Entities;

namespace Traces.Core.Converters
{
    public static class TraceFileConverters
    {
        public static TraceFileDto ConvertToDto(this TraceFile traceFile) => new TraceFileDto
        {
            Id = traceFile.Id,
            Name = traceFile.Name,
            Path = traceFile.Path,
            Size = traceFile.Size,
            CreatedBy = traceFile.CreatedBy,
            MimeType = traceFile.MimeType,
            PublicId = traceFile.PublicId,
            FileGuid = traceFile.FileGuid,
            TraceId = traceFile.TraceId
        };

        public static IReadOnlyList<TraceFileDto> ConvertToDto(this IReadOnlyList<TraceFile> traceFiles) => traceFiles.Select(ConvertToDto).ToList();
    }
}
