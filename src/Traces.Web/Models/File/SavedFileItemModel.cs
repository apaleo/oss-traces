namespace Traces.Web.Models.File
{
    public class SavedFileItemModel
    {
        public TraceFileItemModel TraceFile { get; set; }

        public byte[] Data { get; set; }
    }
}
