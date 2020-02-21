using System.IO;
using BlazorInputFile;
using Traces.Web.Enums;
using Traces.Web.Utils.Converters;

namespace Traces.Web.Models.Files
{
    public class FileToUploadModel
    {
        public string Name { get; set; }

        public MemoryStream Data { get; set; }

        public FileToUploadState State { get; set; } = FileToUploadState.Unknown;

        public IFileListEntry Entry { get; set; }

        public bool IsValid => State == FileToUploadState.Valid;

        public bool HasInvalidSize => State == FileToUploadState.InvalidSize;

        public string SizeInMbFormatted => $"{UnitConverter.ByteToMIB(Entry.Size):0.00} MB";

        public string ProgressFormatted => $"{100.0 * Entry.Data.Position / Entry.Size:0}%";
    }
}
