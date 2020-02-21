using System.Collections.Generic;
using System.Linq;
using Traces.Core.Models.Files;
using Traces.Web.Models.Files;

namespace Traces.Web.Converters.Files
{
    public static class CreateTraceFileItemModelConverters
    {
        public static CreateTraceFileDto ConvertToDto(this CreateTraceFileItemModel createTraceFileItemModel) => new CreateTraceFileDto
        {
            Data = createTraceFileItemModel.Data,
            Name = createTraceFileItemModel.Name,
            Size = createTraceFileItemModel.Size,
            MimeType = createTraceFileItemModel.MimeType,
            TraceId = createTraceFileItemModel.TraceId
        };

        public static IReadOnlyList<CreateTraceFileDto> ConvertToDto(this IReadOnlyList<CreateTraceFileItemModel> createTraceFileItemModels)
            => createTraceFileItemModels.Select(ConvertToDto).ToList();
    }
}
