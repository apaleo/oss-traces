using System;
using NodaTime;

namespace Traces.Core.Models
{
    public class CreateTraceDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public ZonedDateTime DueDate { get; set; }

        public TimeSpan? DueTime { get; set; }
    }
}