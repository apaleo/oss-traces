using System.Collections.Generic;
using System.Linq;
using Traces.Common.Utils;
using Traces.Core.Models.Files;
using Traces.Web.Models.Files;

namespace Traces.Web.Extensions.Files
{
    public static class TraceFileDtoExtensions
    {
        public static TraceFileItemModel ToTraceFileItemModel(this TraceFileDto traceFileDto)
        {
            Check.NotNull(traceFileDto, nameof(traceFileDto));

            return new TraceFileItemModel
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
        }

        public static IReadOnlyList<TraceFileItemModel> ToTraceFileItemModelList(this IReadOnlyList<TraceFileDto> traceFileDtos) => traceFileDtos?.Select(ToTraceFileItemModel).ToList() ?? new List<TraceFileItemModel>();
    }
}
