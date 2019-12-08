using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Utils;
using Traces.Web.Models;
using Traces.Web.Services;

namespace Traces.Web.ViewModels
{
    public class TracesViewModel : BaseViewModel
    {
        private readonly ITracesCollectorService _tracesCollectorService;
        private readonly ITraceModifierService _traceModifierService;
        private readonly IToastService _toastService;

        public TracesViewModel(
            ITracesCollectorService tracesCollectorService,
            ITraceModifierService traceModifierService,
            IToastService toastService,
            IRequestContext requestContext,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor, requestContext)
        {
            _tracesCollectorService = Check.NotNull(tracesCollectorService, nameof(tracesCollectorService));
            _traceModifierService = Check.NotNull(traceModifierService, nameof(traceModifierService));
            _toastService = Check.NotNull(toastService, nameof(toastService));

            TracesGroupModel = new TracesGroupModel();
            EditTraceModificationModel = new EditTraceDialogViewModel();
        }

        public event Action RefreshRequested;

        public TracesGroupModel TracesGroupModel { get; }

        public EditTraceDialogViewModel EditTraceModificationModel { get; }

        public Modal CreateTraceModalRef { get; set; }

        public async Task LoadAsync()
        {
            await InitializeContextAsync();
            await LoadTracesAsync();
        }

        public void ShowCreateTraceModal()
        {
            ShowCreateTraceModal(DateTime.Today);
        }

        public void ShowCreateTraceModal(DateTime dueDate)
        {
            EditTraceModificationModel.ClearCurrentState();
            EditTraceModificationModel.IsReplace = false;

            EditTraceModificationModel.Title = "a";
            EditTraceModificationModel.DueDate = dueDate;

            CreateTraceModalRef?.Show();
        }

        public void HideCreateTraceModal()
        {
            EditTraceModificationModel.ClearCurrentState();
            CreateTraceModalRef?.Hide();
        }

        public void ShowReplaceTraceModal(TraceItemModel traceItemModel)
        {
            EditTraceModificationModel.ClearCurrentState();

            EditTraceModificationModel.IsReplace = true;
            EditTraceModificationModel.Id = traceItemModel.Id;
            EditTraceModificationModel.Title = traceItemModel.Title;
            EditTraceModificationModel.Description = traceItemModel.Description;
            EditTraceModificationModel.DueDate = traceItemModel.DueDate;

            CreateTraceModalRef?.Show();
        }

        public async Task CompleteTraceAsync(int id)
        {
            var completeResult = await _traceModifierService.MarkTraceAsCompleteAsync(id);

            if (completeResult.Success)
            {
                var trace = TracesGroupModel.Traces.FirstOrDefault(t => t.Id == id);

                if (trace == null)
                {
                    return;
                }

                TracesGroupModel.Remove(trace);
                RefreshRequested?.Invoke();

                ShowToastMessage(true, TextConstants.TraceMarkedAsCompletedMessage);
            }
            else
            {
                var errorMessage = completeResult.ErrorMessage.Match(
                    v => v,
                    () => throw new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }
        }

        public async Task<bool> CreateTraceItemAsync()
        {
            var createTraceItemModel = EditTraceModificationModel.GetCreateTraceItemModel();
            var createResult = await _traceModifierService.CreateTraceAsync(createTraceItemModel);

            if (createResult.Success)
            {
                createResult.Result.MatchSome(TracesGroupModel.Add);

                RefreshRequested?.Invoke();

                ShowToastMessage(true, TextConstants.TraceCreatedSuccessfullyMessage);
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

        public async Task<bool> ReplaceTraceItemAsync()
        {
            var replaceTraceItemModel = EditTraceModificationModel.GetReplaceTraceItemModel();
            var replaceResult = await _traceModifierService.ReplaceTraceAsync(replaceTraceItemModel);

            if (replaceResult.Success)
            {
                replaceResult.Result.MatchSome(v =>
                {
                    var trace = TracesGroupModel.Traces.FirstOrDefault(x => x.Id == replaceTraceItemModel.Id);

                    if (trace == null)
                    {
                        return;
                    }

                    TracesGroupModel.Replace(trace, replaceTraceItemModel);
                    RefreshRequested?.Invoke();

                    ShowToastMessage(true, TextConstants.TraceUpdatedSuccessfullyMessage);
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
                    var trace = TracesGroupModel.Traces.FirstOrDefault(t => t.Id == id);

                    if (trace == null)
                    {
                        return;
                    }

                    TracesGroupModel.Remove(trace);
                    RefreshRequested?.Invoke();

                    ShowToastMessage(true, TextConstants.TraceDeletedSuccessfullyMessage);
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

        private async Task LoadTracesAsync()
        {
            var tracesResult = await _tracesCollectorService.GetTracesAsync();

            if (tracesResult.Success)
            {
                TracesGroupModel.Clear();

                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                foreach (var trace in traces)
                {
                    TracesGroupModel.Add(trace);
                }

                RefreshRequested?.Invoke();
            }
        }

        private void ShowToastMessage(bool success, string message)
        {
            var header = success ? TextConstants.SuccessHeaderText : TextConstants.ErrorHeaderText;

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