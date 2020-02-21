using System.IO;

namespace Traces.Web.Models.Files
{
    public class CreateTraceFileItemModel
    {
        public string Name { get; set; }

        public string MimeType { get; set; }

        public long Size { get; set; }

        public int TraceId { get; set; }

        public MemoryStream Data { get; set; }
    }
}
