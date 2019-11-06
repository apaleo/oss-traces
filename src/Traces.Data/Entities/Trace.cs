using System;
using System.ComponentModel.DataAnnotations;
using NodaTime;
using Traces.Common.Enums;

namespace Traces.Data.Entities
{
    public class Trace : BaseEntity
    {
        public Guid EntityId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public Instant DueDate { get; set; }

        public TimeSpan? DueTime { get; set; }

        [Required]
        public TaskStateEnum State { get; set; }

        public string CompletedBy { get; set; }

        public Instant? CompletedDate { get; set; }
    }
}