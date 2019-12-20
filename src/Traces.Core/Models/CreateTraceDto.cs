using NodaTime;
using Optional;

namespace Traces.Core.Models
{
    public class CreateTraceDto
    {
        public string Title { get; set; }

        public Option<string> Description { get; set; }

        public LocalDate DueDate { get; set; }

        public string PropertyId { get; set; }

        public Option<string> ReservationId { get; set; }

        public Option<string> AssignedRole { get; set; }
    }
}