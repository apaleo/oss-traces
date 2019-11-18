using System.Globalization;
using NodaTime;

namespace Traces.Web.Models
{
    public class CreateTraceItemModel
    {
        private LocalDate _dueDate;

        public string Title { get; set; }

        public string Description { get; set; }

        public string DueDateString { get; private set; }

        public LocalDate DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                DueDateString = _dueDate.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);
            }
        }
    }
}