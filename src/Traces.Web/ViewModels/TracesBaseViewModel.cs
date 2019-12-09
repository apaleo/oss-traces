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
            EditTraceModificationModel = new EditTraceDialogViewModel();
        }

        public EditTraceDialogViewModel EditTraceModificationModel { get; }

        public Modal CreateTraceModalRef { get; set; }

        public List<TraceItemModel> Traces { get; }

        protected ITraceModifierService TraceModifierService { get; }

        public abstract Task LoadAsync();

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

        public void ShowCreateTraceModal()
        {
            EditTraceModificationModel.ClearCurrentState();
            EditTraceModificationModel.IsReplace = false;

            CreateTraceModalRef?.Show();
        }

        public void HideCreateTraceModal()
        {
            EditTraceModificationModel.ClearCurrentState();
            CreateTraceModalRef?.Hide();
        }

        /// <summary>
        /// Currently each viewmodel that can create a trace should override this method.
        /// For instance the TracesViewModel should not be able to create a trace at this current stage.
        /// </summary>
        /// <returns>Trace was created or not</returns>
        protected virtual Task<bool> CreateTraceItemAsync() => throw new NotImplementedException();

        protected async Task<bool> ReplaceTraceItemAsync()
        {
            var replaceTraceItemModel = EditTraceModificationModel.GetReplaceTraceItemModel();
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

            return replaceResult.Success;
        }

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