using System.IO;
using BlazorInputFile;

namespace Traces.Web.Models.Files
{
    public class FileToUploadModel
    {
        public string Name { get; set; }

        public MemoryStream Data { get; set; }

        public IFileListEntry Entry { get; set; }
    }
}
