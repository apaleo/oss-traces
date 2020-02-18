using System.Collections.Generic;
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
        public LocalDate DueDate { get; set; }

        [Required]
        public TraceState State { get; set; }

        public LocalDate? CompletedDate { get; set; }

        public string CompletedBy { get; set; }

        [Required]
        public string PropertyId { get; set; }

        public string ReservationId { get; set; }

        public string AssignedRole { get; set; }

        public List<TraceFile> Files { get; }
    }
}