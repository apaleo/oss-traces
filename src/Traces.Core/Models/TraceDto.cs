using System;
using NodaTime;
using Optional;
using Traces.Common.Enums;

namespace Traces.Core.Models
{
    public class TraceDto
    {
        public Guid EntityId { get; set; }

        public string Title { get; set; }

        public Option<string> Description { get; set; }

        public ZonedDateTime DueDate { get; set; }

        public Option<TimeSpan> DueTime { get; set; }

        public TaskStateEnum State { get; set; }

        public Option<string> CompletedBy { get; set; }

        public Option<ZonedDateTime> CompletedDate { get; set; }
    }
}