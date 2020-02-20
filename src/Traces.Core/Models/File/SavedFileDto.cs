namespace Traces.Core.Models.File
{
    public class SavedFileDto
    {
        public TraceFileDto TraceFile { get; set; }

        public byte[] Data { get; set; }
    }
}
