namespace Traces.Web.Models.Files
{
    public class SavedFileItemModel
    {
        public TraceFileItemModel TraceFile { get; set; }

        public byte[] Data { get; set; }
    }
}
