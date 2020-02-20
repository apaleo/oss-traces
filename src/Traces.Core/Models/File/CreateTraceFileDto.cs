using System.IO;

namespace Traces.Core.Models.File
{
    public class CreateTraceFileDto
    {
        public string Name { get; set; }

        public string MimeType { get; set; }

        public long Size { get; set; }

        public int TraceId { get; set; }

        public MemoryStream Data { get; set; }
    }
}
