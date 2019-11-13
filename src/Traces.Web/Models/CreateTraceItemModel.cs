using NodaTime;

namespace Traces.Web.Models
{
    public class CreateTraceItemModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public LocalDate DueDate { get; set; }

        public LocalTime DueTime { get; set; }
    }
}