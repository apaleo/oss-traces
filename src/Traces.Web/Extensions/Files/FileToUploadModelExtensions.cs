using System.Collections.Generic;
using System.Linq;
using Traces.Web.Models.Files;

namespace Traces.Web.Extensions.Files
{
    public static class FileToUploadModelExtensions
    {
        public static CreateTraceFileItemModel ToCreateTraceFileItemModel(this FileToUploadModel fileToUploadModel, int traceId) => new CreateTraceFileItemModel
        {
            Data = fileToUploadModel.Data,
            Name = fileToUploadModel.Name,
            Size = fileToUploadModel.Entry.Size,
            MimeType = fileToUploadModel.Entry.Type,
            TraceId = traceId
        };

        public static List<CreateTraceFileItemModel> ToCreateTraceFileItemModelList(this List<FileToUploadModel> fileToUploadModels, int traceId)
            => fileToUploadModels.Select(file => ToCreateTraceFileItemModel(file, traceId)).ToList();
    }
}
