using System.Globalization;
using NodaTime;

namespace Traces.Web.Models
{
    public class CreateTraceItemModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string DueDateString { get; private set; }

        private LocalDate _dueDate;
        public LocalDate DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                DueDateString = _dueDate.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);
            }
        }

        public string DueTimeString { get; private set; }

        private LocalTime? _dueTime;
        public LocalTime? DueTime
        {
            get => _dueTime;
            set
            {
                _dueTime = value;
                if (value != null)
                {
                    DueTimeString = value.Value.ToString("HH:mm", CultureInfo.CurrentCulture);
                }
            }
        }
    }
}