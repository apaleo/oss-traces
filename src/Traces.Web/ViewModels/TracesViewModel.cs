using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Traces.Common.Utils;
using Traces.Web.Models;
using Traces.Web.Services;

namespace Traces.Web.ViewModels
{
    public class TracesViewModel
    {
        private readonly ITracesCollectorService _tracesCollectorService;
        private readonly ITraceModifierService _traceModifierService;
        private readonly IToastService _toastService;

        public TracesViewModel(
            ITracesCollectorService tracesCollectorService,
            ITraceModifierService traceModifierService,
            IToastService toastService)
        {
            _tracesCollectorService = Check.NotNull(tracesCollectorService, nameof(tracesCollectorService));
            _traceModifierService = Check.NotNull(traceModifierService, nameof(traceModifierService));
            _toastService = Check.NotNull(toastService, nameof(toastService));
            Traces = new List<TraceItemModel>();
        }

        public List<TraceItemModel> Traces { get; }

        public TraceItemModel ConfiguringTrace { get; private set; }

        public bool ShowingCreateTraceDialog { get; private set; }

        public bool ShowingUpdateTraceDialog { get; private set; }

        public async Task LoadTracesAsync()
        {
            var tracesResult = await _tracesCollectorService.GetTracesAsync();

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                foreach (var trace in traces)
                {
                    Traces.Add(trace);
                }
            }
        }

        public void ShowCreateTraceDialog()
        {
            ShowingCreateTraceDialog = true;
        }

        public void ShowReplaceTraceDialog(TraceItemModel traceItemModel)
        {
            ConfiguringTrace = traceItemModel;

            ShowingUpdateTraceDialog = true;
        }

        public async Task CompleteTraceAsync(int id)
        {
            var completeResult = await _traceModifierService.MarkTraceAsCompleteAsync(id);

            if (completeResult.Success)
            {
                var trace = Traces.FirstOrDefault(t => t.Id == id);

                if (trace == null)
                {
                    return;
                }

                Traces.Remove(trace);

                ShowToastMessage(true, "Trace marked as completed successfully.");
            }
            else
            {
                var errorMessage = completeResult.ErrorMessage.Match(
                    v => v,
                    () => throw new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }
        }

        public void HideCreateTraceDialog()
        {
            ShowingCreateTraceDialog = false;
        }

        public void HideUpdateTraceDialog()
        {
            ShowingUpdateTraceDialog = false;
        }

        public async Task<bool> CreateTraceItemAsync(CreateTraceItemModel createTraceItemModel)
        {
            var createResult = await _traceModifierService.CreateTraceAsync(createTraceItemModel);

            if (createResult.Success)
            {
                createResult.Result.MatchSome(v =>
                {
                    Traces.Add(v);
                });

                ShowToastMessage(true, "Trace added successfully");
            }
            else
            {
                var errorMessage = createResult.ErrorMessage.Match(
                    v => v,
                    () => throw new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }

            return createResult.Success;
        }

        public async Task<bool> ReplaceTraceItemAsync(ReplaceTraceItemModel replaceTraceItemModel)
        {
            var replaceResult = await _traceModifierService.ReplaceTraceAsync(replaceTraceItemModel);

            if (replaceResult.Success)
            {
                replaceResult.Result.MatchSome(v =>
                {
                    var trace = Traces.FirstOrDefault(x => x.Id == replaceTraceItemModel.Id);

                    if (trace == null)
                    {
                        return;
                    }

                    trace.Title = replaceTraceItemModel.Title;
                    trace.Description = replaceTraceItemModel.Description;
                    trace.DueDate = replaceTraceItemModel.DueDate;

                    ShowToastMessage(true, "Trace updated successfully.");
                });
            }
            else
            {
                var errorMessage = replaceResult.ErrorMessage.Match(
                    v => v,
                    () => throw new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }

            return replaceResult.Success;
        }

        public async Task DeleteItemAsync(int id)
        {
            var deleteResult = await _traceModifierService.DeleteTraceAsync(id);

            if (deleteResult.Success)
            {
                deleteResult.Result.MatchSome(v =>
                {
                    var trace = Traces.FirstOrDefault(t => t.Id == id);

                    if (trace == null)
                    {
                        return;
                    }

                    Traces.Remove(trace);

                    ShowToastMessage(true, "Trace removed successfully.");
                });
            }
            else
            {
                var errorMessage = deleteResult.ErrorMessage.Match(
                    v => v,
                    () => throw new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }
        }

        private void ShowToastMessage(bool success, string message)
        {
            var header = success ? "Success" : "Oops";

            if (success)
            {
                _toastService.ShowSuccess(message, header);
            }
            else
            {
                _toastService.ShowError(message, header);
            }
        }
    }
}