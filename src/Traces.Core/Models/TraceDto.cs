using NodaTime;
using Optional;
using Traces.Common.Enums;

namespace Traces.Core.Models
{
    public class TraceDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public Option<string> Description { get; set; }

        public ZonedDateTime DueDate { get; set; }

        public TraceStateEnum State { get; set; }

        public Option<ZonedDateTime> CompletedDate { get; set; }
    }
}