using NodaTime;

namespace Traces.Web.Models
{
    public class ReplaceTraceItemModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DueDateString { get; private set; }

        public LocalDate DueDate { get; set; }
    }
}