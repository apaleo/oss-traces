namespace Traces.Core.Models.TraceFile
{
    public class CreateTraceFileDto
    {
        public string Name { get; set; }

        public string MimeType { get; set; }

        public long Size { get; set; }

        public int TraceId { get; set; }
    }
}
