using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Traces.Web.Models;

namespace Traces.Web.ViewModels
{
    public class TracesViewModel
    {
        public TracesViewModel()
        {
            Traces = new List<TraceItemModel>();
        }

        public bool ShowCreateTraceDialog { get; set; }

        public List<TraceItemModel> Traces { get; }

        public void AddItem()
        {
            ToggleCreateTraceDialog(true);
        }

        public void CloseCreateDialog()
        {
            ToggleCreateTraceDialog(false);
        }

        public bool CreateTraceItem(CreateTraceItemModel createTraceItemModel)
        {
            if (createTraceItemModel == null ||
                string.IsNullOrWhiteSpace(createTraceItemModel.Title) ||
                createTraceItemModel.DueDate == LocalDate.MinIsoValue)
            {
                return false;
            }

            var lastTrace = Traces.LastOrDefault();

            var id = lastTrace == null ? 0 : ++lastTrace.Id;

            var trace = new TraceItemModel
            {
                Id = id,
                Title = createTraceItemModel.Title,
                Description = createTraceItemModel.Description,
                DueDate = createTraceItemModel.DueDate,
                DueTime = createTraceItemModel.DueTime
            };

            Traces.Add(trace);

            return true;
        }

        public void DeleteItem(int id)
        {
            var trace = Traces.FirstOrDefault(t => t.Id == id);

            if (trace == null)
            {
                return;
            }

            Traces.Remove(trace);
        }

        private void ToggleCreateTraceDialog(bool isVisible) => ShowCreateTraceDialog = isVisible;
    }
}