using NodaTime;

namespace Traces.Web.Models
{
    public class TraceItemModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public LocalDate DueDate { get; set; }

        public bool IsComplete { get; set; }
    }
}