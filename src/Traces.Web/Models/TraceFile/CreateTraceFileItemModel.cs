namespace Traces.Web.Models.TraceFile
{
    public class CreateTraceFileItemModel
    {
        public string Name { get; set; }

        public string MimeType { get; set; }

        public long Size { get; set; }

        public int TraceId { get; set; }
    }
}
