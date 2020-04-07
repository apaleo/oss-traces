using System.Collections.Generic;
using System.Linq;
using Traces.Core.Models.Files;
using Traces.Web.Models.Files;

namespace Traces.Web.Extensions.Files
{
    public static class CreateTraceFileItemModelExtensions
    {
        public static CreateTraceFileDto ToCreateTraceFileDto(this CreateTraceFileItemModel createTraceFileItemModel) => new CreateTraceFileDto
        {
            Data = createTraceFileItemModel.Data,
            Name = createTraceFileItemModel.Name,
            Size = createTraceFileItemModel.Size,
            MimeType = createTraceFileItemModel.MimeType,
            TraceId = createTraceFileItemModel.TraceId
        };

        public static List<CreateTraceFileDto> ToCreateTraceFileDtoList(this List<CreateTraceFileItemModel> createTraceFileItemModels)
            => createTraceFileItemModels.Select(ToCreateTraceFileDto).ToList();
    }
}
