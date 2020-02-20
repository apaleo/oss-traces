using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traces.Data.Entities
{
    public class TraceFile : BaseEntity
    {
        [Required]
        [Range(1, 255)]
        public string Name { get; set; }

        [Required]
        public string MimeType { get; set; }

        [Required]
        public long Size { get; set; }

        [Required]
        public Guid PublicId { get; set; }

        [Required]
        public Guid FileGuid { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [ForeignKey(nameof(Trace))]
        public int TraceId { get; set; }

        public Trace Trace { get; set; }
    }
}
