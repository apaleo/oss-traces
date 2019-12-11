using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Http;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Utils;
using Traces.Web.Models;
using Traces.Web.Services;

namespace Traces.Web.ViewModels
{
    public abstract class TracesBaseViewModel : BaseViewModel
    {
        private readonly IToastService _toastService;

        protected TracesBaseViewModel(ITraceModifierService traceModifierService, IToastService toastService, IHttpContextAccessor httpContextAccessor, IRequestContext requestContext)
            : base(httpContextAccessor, requestContext)
        {
            TraceModifierService = Check.NotNull(traceModifierService, nameof(traceModifierService));
            _toastService = Check.NotNull(toastService, nameof(toastService));
            Traces = new List<TraceItemModel>();
            SortedGroupedTracesDictionary = new SortedDictionary<DateTime, List<TraceItemModel>>();
            OverdueTraces = new List<TraceItemModel>();
            EditTraceDialogViewModel = new EditTraceDialogViewModel();
        }

        public EditTraceDialogViewModel EditTraceDialogViewModel { get; }

        public Modal CreateTraceModalRef { get; set; }

        public Modal EditTraceModalRef { get; set; }

        public List<TraceItemModel> Traces { get; }

        public List<TraceItemModel> OverdueTraces { get; }

        public SortedDictionary<DateTime, List<TraceItemModel>> SortedGroupedTracesDictionary { get; }

        protected ITraceModifierService TraceModifierService { get; }

        public async Task LoadAsync()
        {
            await InitializeContextAsync();
            await LoadTracesAsync();
        }

        /// <summary>
        /// Currently each viewmodel that can create a trace should override this method.
        /// For instance the TracesViewModel should not be able to create a trace at this current stage.
        /// </summary>
        /// <returns>Trace was created or not</returns>
        public virtual Task CreateTraceItemAsync() => throw new NotImplementedException();

        public async Task EditTraceItemAsync()
        {
            var replaceTraceItemModel = EditTraceDialogViewModel.GetReplaceTraceItemModel();
            var replaceResult = await TraceModifierService.ReplaceTraceAsync(replaceTraceItemModel);

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

            if (replaceResult.Success)
            {
                HideEditTraceModal();
            }
        }

        public async Task DeleteItemAsync(int id)
        {
            var deleteResult = await TraceModifierService.DeleteTraceAsync(id);

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

        public async Task CompleteTraceAsync(int id)
        {
            var completeResult = await TraceModifierService.MarkTraceAsCompleteAsync(id);

            if (completeResult.Success)
            {
                var trace = Traces.FirstOrDefault(t => t.Id == id);

                if (trace == null)
                {
                    return;
                }

                Traces.Remove(trace);

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

        public void ShowReplaceTraceModal(TraceItemModel traceItemModel)
        {
            EditTraceDialogViewModel.ClearCurrentState();

            EditTraceDialogViewModel.Id = traceItemModel.Id;
            EditTraceDialogViewModel.Title = traceItemModel.Title;
            EditTraceDialogViewModel.Description = traceItemModel.Description;
            EditTraceDialogViewModel.DueDate = traceItemModel.DueDate;

            EditTraceModalRef?.Show();
        }

        public void ShowCreateTraceModal()
        {
            EditTraceDialogViewModel.ClearCurrentState();

            CreateTraceModalRef?.Show();
        }

        public void HideCreateTraceModal()
        {
            EditTraceDialogViewModel.ClearCurrentState();
            CreateTraceModalRef?.Hide();
        }

        public void HideEditTraceModal()
        {
            EditTraceDialogViewModel.ClearCurrentState();
            EditTraceModalRef?.Hide();
        }

        protected abstract Task LoadTracesAsync();

        protected void ShowToastMessage(bool success, string message)
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