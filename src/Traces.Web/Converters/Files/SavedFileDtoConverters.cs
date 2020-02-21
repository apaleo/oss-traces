using Traces.Core.Models.Files;
using Traces.Web.Models.Files;

namespace Traces.Web.Converters.Files
{
    public static class SavedFileDtoConverters
    {
        public static SavedFileItemModel ConvertToSavedFileItemModel(this SavedFileDto savedFileDto) => new SavedFileItemModel
        {
            Data = savedFileDto.Data,
            TraceFile = savedFileDto.TraceFile.ConvertToItemModel()
        };
    }
}
