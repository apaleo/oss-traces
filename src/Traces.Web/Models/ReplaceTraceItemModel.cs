using System.Globalization;
using NodaTime;

namespace Traces.Web.Models
{
    public class ReplaceTraceItemModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DueDateString { get; set; }

        private LocalDate _dueDate;

        public LocalDate DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                DueDateString = value.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);
            }
        }

        public string DueTimeString { get; set; }

        private LocalTime _dueTime;

        public LocalTime DueTime
        {
            get => _dueTime;
            set
            {
                _dueTime = value;
                DueTimeString = value.ToString("HH:mm", CultureInfo.CurrentCulture);
            }
        }
    }
}