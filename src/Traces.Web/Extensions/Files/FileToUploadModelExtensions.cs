using System.Collections.Generic;
using System.Linq;
using Traces.Common.Utils;
using Traces.Web.Models.Files;

namespace Traces.Web.Extensions.Files
{
    public static class FileToUploadModelExtensions
    {
        public static CreateTraceFileItemModel ToCreateTraceFileItemModel(this FileToUploadModel fileToUploadModel)
        {
            Check.NotNull(fileToUploadModel, nameof(fileToUploadModel));

            return new CreateTraceFileItemModel
            {
                Data = fileToUploadModel.Data,
                Name = fileToUploadModel.Name,
                Size = fileToUploadModel.Entry.Size,
                MimeType = fileToUploadModel.Entry.Type
            };
        }

        public static List<CreateTraceFileItemModel> ToCreateTraceFileItemModelList(this List<FileToUploadModel> fileToUploadModels) => fileToUploadModels?.Select(ToCreateTraceFileItemModel).ToList() ?? new List<CreateTraceFileItemModel>();

        public static List<CreateTraceFileItemModel> GetValidCreateTraceFileItemModels(this List<FileToUploadModel> fileToUploadModels) => fileToUploadModels?
                .Where(file => file.IsValid)
                .ToList()
                .ToCreateTraceFileItemModelList()
            ?? new List<CreateTraceFileItemModel>();
    }
}
