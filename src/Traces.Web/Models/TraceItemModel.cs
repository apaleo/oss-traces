using System;
using Traces.Common.Enums;

namespace Traces.Web.Models
{
    public class TraceItemModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public TraceState State { get; set; }

        public DateTime DueDate { get; set; }

        public string PropertyId { get; set; }

        public string ReservationId { get; set; }

        public string AssignedRole { get; set; }
    }
}