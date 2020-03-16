using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Traces.Common.Constants;
using Traces.Common.Extensions;
using Traces.Web.Models;
using Traces.Web.Services;
using Traces.Web.Services.Apaleo;
using Traces.Web.Services.ApaleoOne;

namespace Traces.Web.ViewModels.Traces
{
    public abstract class TracesBaseViewModel : ApaleoBaseComponent
    {
        public EditTraceDialogViewModel EditTraceDialogViewModel { get; } = new EditTraceDialogViewModel();

        public Modal CreateTraceModalRef { get; set; }

        public Modal EditTraceModalRef { get; set; }

        [Inject]
        protected ITraceModifierService TraceModifierService { get; set; }

        [Inject]
        protected IApaleoOneNotificationService ApaleoOneNotificationService { get; set; }

        [Inject]
        private IApaleoRolesCollectorService ApaleoRolesCollector { get; set; }

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
                await RefreshAsync();

                await ApaleoOneNotificationService.ShowSuccessAsync(TextConstants.TraceUpdatedSuccessfullyMessage);
            }
            else
            {
                var errorMessage = replaceResult.ErrorMessage.ValueOrException(new NotImplementedException());

                await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
            }

            if (replaceResult.Success)
            {
                HideEditTraceModal();
            }
        }

        public async Task DeleteItemAsync(TraceItemModel trace)
        {
            var deleteResult = await TraceModifierService.DeleteTraceAsync(trace.Id);

            if (deleteResult.Success)
            {
                await RefreshAsync();

                await ApaleoOneNotificationService.ShowSuccessAsync(TextConstants.TraceDeletedSuccessfullyMessage);
            }
            else
            {
                var errorMessage = deleteResult.ErrorMessage.ValueOrException(new NotImplementedException());

                await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
            }
        }

        public async Task RevertCompleteItemAsync(TraceItemModel trace)
        {
            var revertResult = await TraceModifierService.RevertCompleteTraceAsync(trace.Id);

            if (revertResult.Success)
            {
                await RefreshAsync();

                await ApaleoOneNotificationService.ShowSuccessAsync(TextConstants.TraceRevertedCompleteSuccessfullyMessage);
            }
            else
            {
                var errorMessage = revertResult.ErrorMessage.ValueOrException(new NotImplementedException());

                await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
            }
        }

        public async Task CompleteTraceAsync(TraceItemModel trace)
        {
            var completeResult = await TraceModifierService.MarkTraceAsCompleteAsync(trace.Id);

            if (completeResult.Success)
            {
                await RefreshAsync();

                await ApaleoOneNotificationService.ShowSuccessAsync(TextConstants.TraceMarkedAsCompletedMessage);
            }
            else
            {
                var errorMessage = completeResult.ErrorMessage.ValueOrException(new NotImplementedException());

                await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
            }
        }

        public void ShowReplaceTraceModal(TraceItemModel traceItemModel)
        {
            EditTraceDialogViewModel.ClearCurrentState();

            EditTraceDialogViewModel.Id = traceItemModel.Id;
            EditTraceDialogViewModel.Title = traceItemModel.Title;
            EditTraceDialogViewModel.Description = traceItemModel.Description;
            EditTraceDialogViewModel.DueDate = traceItemModel.DueDate;
            EditTraceDialogViewModel.SelectedRole = traceItemModel.AssignedRole;

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

        protected override async Task OnInitializedAsync()
        {
            await InitializeContextAsync();

            await LoadTracesAsync();

            await LoadApaleoRolesAsync();

            StateHasChanged();

            await base.OnInitializedAsync();
        }

        protected abstract Task LoadTracesAsync();

        protected abstract Task RefreshAsync();

        private async Task LoadApaleoRolesAsync()
        {
            var roles = await ApaleoRolesCollector.GetRolesAsync();

            EditTraceDialogViewModel.Roles.Clear();

            // This is required so the user can select a no role assigned option
            EditTraceDialogViewModel.Roles.Add(TextConstants.TracesEditDialogNoRoleAssignedText);

            foreach (var role in roles)
            {
                EditTraceDialogViewModel.Roles.Add(role);
            }
        }
    }
}
