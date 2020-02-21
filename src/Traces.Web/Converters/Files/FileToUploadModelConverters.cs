using System.Collections.Generic;
using System.Linq;
using Traces.Web.Models.Files;

namespace Traces.Web.Converters.Files
{
    public static class FileToUploadModelConverters
    {
        public static CreateTraceFileItemModel ConvertToCreateTraceFileItemModel(this FileToUploadModel fileToUploadModel, int traceId) => new CreateTraceFileItemModel
        {
            Data = fileToUploadModel.Data,
            Name = fileToUploadModel.Name,
            Size = fileToUploadModel.Entry.Size,
            MimeType = fileToUploadModel.Entry.Type,
            TraceId = traceId
        };

        public static IReadOnlyList<CreateTraceFileItemModel> ConvertToCreateTraceFileItemModel(this IReadOnlyList<FileToUploadModel> fileToUploadModels, int traceId)
            => fileToUploadModels.Select(file => ConvertToCreateTraceFileItemModel(file, traceId)).ToList();
    }
}
