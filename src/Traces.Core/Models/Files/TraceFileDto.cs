using System;

namespace Traces.Core.Models.Files
{
    public class TraceFileDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string MimeType { get; set; }

        public long Size { get; set; }

        public Guid PublicId { get; set; }

        public string Path { get; set; }

        public string CreatedBy { get; set; }

        public int TraceId { get; set; }
    }
}
