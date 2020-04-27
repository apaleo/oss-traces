using System.Collections.Generic;
using System.Linq;
using Traces.Core.Models.Files;
using Traces.Data.Entities;

namespace Traces.Core.Extensions.Files
{
    public static class TraceFileExtensions
    {
        public static TraceFileDto ToTraceFileDto(this TraceFile traceFile) => new TraceFileDto
        {
            Id = traceFile.Id,
            Name = traceFile.Name,
            Path = traceFile.Path,
            Size = traceFile.Size,
            CreatedBy = traceFile.CreatedBy,
            MimeType = traceFile.MimeType,
            PublicId = traceFile.PublicId,
            TraceId = traceFile.TraceId
        };

        public static IReadOnlyList<TraceFileDto> ToTraceFileDtoList(this IReadOnlyList<TraceFile> traceFiles) => traceFiles.Select(ToTraceFileDto).ToList();
    }
}
