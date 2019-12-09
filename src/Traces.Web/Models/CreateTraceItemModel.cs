using System;

namespace Traces.Web.Models
{
    public class CreateTraceItemModel
    {
        public CreateTraceItemModel()
        {
            DueDate = DateTime.Now;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public string ReservationId { get; set; }

        public string PropertyId { get; set; }
    }
}