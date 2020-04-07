using Traces.Core.Models.Files;
using Traces.Web.Models.Files;

namespace Traces.Web.Extensions.Files
{
    public static class SavedFileDtoExtensions
    {
        public static SavedFileItemModel ToSavedFileItemModel(this SavedFileDto savedFileDto) => new SavedFileItemModel
        {
            Data = savedFileDto.Data,
            TraceFile = savedFileDto.TraceFile.ToTraceFileItemModel()
        };
    }
}
