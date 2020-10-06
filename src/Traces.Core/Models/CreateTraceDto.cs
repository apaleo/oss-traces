using System.Collections.Generic;
using NodaTime;
using Optional;
using Traces.Core.Models.Files;

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

        public Option<List<CreateTraceFileDto>> FilesToUpload { get; set; }

        public bool FileContainsNoPii { get; set; }
    }
}
