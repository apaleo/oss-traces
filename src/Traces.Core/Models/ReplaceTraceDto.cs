using System.Collections.Generic;
using NodaTime;
using Optional;
using Traces.Core.Models.Files;

namespace Traces.Core.Models
{
    public class ReplaceTraceDto
    {
        public string Title { get; set; }

        public Option<string> Description { get; set; }

        public LocalDate DueDate { get; set; }

        public Option<string> AssignedRole { get; set; }

        public Option<List<CreateTraceFileDto>> FilesToUpload { get; set; }

        public Option<List<int>> FilesToDelete { get; set; }

        public bool FileContainsNoPii { get; set; }
    }
}
