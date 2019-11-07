using System;
using NodaTime;
using Optional;

namespace Traces.Core.Models
{
    public class CreateTraceDto
    {
        public string Title { get; set; }

        public Option<string> Description { get; set; }

        public ZonedDateTime DueDate { get; set; }

        public Option<TimeSpan> DueTime { get; set; }
    }
}