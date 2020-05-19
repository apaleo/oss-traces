namespace Traces.Core.Models.Files
{
    public class SavedFileDto
    {
        public TraceFileDto TraceFile { get; set; }

        public byte[] Data { get; set; }
    }
}
