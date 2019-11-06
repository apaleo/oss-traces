using System;
using NodaTime;
using Traces.Common.Enums;

namespace Traces.Core.Models
{
    public class TraceDto
    {
        public Guid EntityId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ZonedDateTime DueDate { get; set; }

        public TimeSpan DueTime { get; set; }

        public TaskStateEnum State { get; set; }

        public string CompletedBy { get; set; }

        public ZonedDateTime CompletedDate { get; set; }
    }
}