using System;
using System.Collections.Generic;
using System.Linq;
using Traces.Web.Models;

namespace Traces.Web.ViewModels
{
    public class TracesViewModel
    {
        public TracesViewModel()
        {
            Traces = new List<TraceItemModel>();
        }

        public List<TraceItemModel> Traces { get; }

        public TraceItemModel ConfiguringTrace { get; set; }

        public bool ShowCreateTraceDialog { get; set; }

        public bool ShowingUpdateTraceDialog { get; set; }

        public void AddItem()
        {
            ShowCreateTraceDialog = true;
        }

        public void ShowReplaceTraceDialog(TraceItemModel traceItemModel)
        {
            ConfiguringTrace = traceItemModel;

            ShowingUpdateTraceDialog = true;
        }

        public void CompleteTrace(int id)
        {
            var trace = Traces.FirstOrDefault(t => t.Id == id);

            if (trace == null)
            {
                return;
            }

            Traces.Remove(trace);
        }

        public void CloseCreateDialog()
        {
            ShowCreateTraceDialog = false;
        }

        public void HideUpdateTraceDialog()
        {
            ShowingUpdateTraceDialog = false;
        }

        public bool CreateTraceItem(CreateTraceItemModel createTraceItemModel)
        {
            if (createTraceItemModel == null ||
                string.IsNullOrWhiteSpace(createTraceItemModel.Title) ||
                createTraceItemModel.DueDate == DateTime.MinValue)
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
                DueDate = createTraceItemModel.DueDate
            };

            Traces.Add(trace);

            return true;
        }

        public bool ReplaceTraceItem(ReplaceTraceItemModel replaceTraceItemModel)
        {
            if (replaceTraceItemModel == null ||
                string.IsNullOrWhiteSpace(replaceTraceItemModel.Title) ||
                replaceTraceItemModel.DueDate == DateTime.MinValue)
            {
                return false;
            }

            var trace = Traces.FirstOrDefault(t => t.Id == replaceTraceItemModel.Id);

            if (trace == null)
            {
                return false;
            }

            trace.Title = replaceTraceItemModel.Title;
            trace.Description = replaceTraceItemModel.Description;
            trace.DueDate = replaceTraceItemModel.DueDate;

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
    }
}