using System.ComponentModel.DataAnnotations;
using NodaTime;
using Traces.Common.Enums;

namespace Traces.Data.Entities
{
    public class Trace : BaseEntity
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public Instant DueDateUtc { get; set; }

        public LocalTime? DueTime { get; set; }

        [Required]
        public TraceStateEnum State { get; set; }

        public Instant? CompletedUtc { get; set; }
    }
}