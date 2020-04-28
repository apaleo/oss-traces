using Traces.Common.Utils;
using Traces.Core.Models.Files;
using Traces.Web.Models.Files;

namespace Traces.Web.Extensions.Files
{
    public static class SavedFileDtoExtensions
    {
        public static SavedFileItemModel ToSavedFileItemModel(this SavedFileDto savedFileDto)
        {
            Check.NotNull(savedFileDto, nameof(savedFileDto));

            return new SavedFileItemModel
            {
                Data = savedFileDto.Data,
                TraceFile = savedFileDto.TraceFile.ToTraceFileItemModel()
            };
        }
    }
}
