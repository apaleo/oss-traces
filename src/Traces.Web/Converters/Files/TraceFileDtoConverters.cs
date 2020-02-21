using System.Collections.Generic;
using System.Linq;
using Traces.Core.Models.Files;
using Traces.Web.Models.Files;

namespace Traces.Web.Converters.Files
{
    public static class TraceFileDtoConverters
    {
        public static TraceFileItemModel ConvertToItemModel(this TraceFileDto traceFileDto) => new TraceFileItemModel
        {
            Id = traceFileDto.Id,
            Name = traceFileDto.Name,
            Path = traceFileDto.Path,
            Size = traceFileDto.Size,
            CreatedBy = traceFileDto.CreatedBy,
            MimeType = traceFileDto.MimeType,
            PublicId = traceFileDto.PublicId,
            TraceId = traceFileDto.TraceId
        };

        public static IReadOnlyList<TraceFileItemModel> ConvertToItemModel(this IReadOnlyList<TraceFileDto> traceFileDtos)
            => traceFileDtos.Select(ConvertToItemModel).ToList();
    }
}
