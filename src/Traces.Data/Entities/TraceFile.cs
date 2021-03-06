using System;
using System.ComponentModel.DataAnnotations;

namespace Traces.Data.Entities
{
    public class TraceFile : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string MimeType { get; set; }

        [Required]
        public long Size { get; set; }

        [Required]
        public Guid PublicId { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public int TraceId { get; set; }

        public Trace Trace { get; set; }
    }
}
