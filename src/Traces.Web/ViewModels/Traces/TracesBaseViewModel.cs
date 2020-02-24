using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Http;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Extensions;
using Traces.Common.Utils;
using Traces.Web.Models;
using Traces.Web.Services;
using Traces.Web.Services.Apaleo;
using Traces.Web.Services.ApaleoOne;

namespace Traces.Web.ViewModels.Traces
{
    public abstract class TracesBaseViewModel : BaseViewModel
    {
        private readonly IApaleoRolesCollectorService _apaleoRolesCollector;

        protected TracesBaseViewModel(
            ITraceModifierService traceModifierService,
            IFileService fileService,
            IHttpContextAccessor httpContextAccessor,
            IRequestContext requestContext,
            IApaleoRolesCollectorService apaleoRolesCollector,
            IApaleoOneNotificationService apaleoOneNotificationService)
            : base(httpContextAccessor, requestContext)
        {
            TraceModifierService = Check.NotNull(traceModifierService, nameof(traceModifierService));
            FileService = Check.NotNull(fileService, nameof(fileService));
            ApaleoOneNotificationService = Check.NotNull(apaleoOneNotificationService, nameof(apaleoOneNotificationService));

            _apaleoRolesCollector = Check.NotNull(apaleoRolesCollector, nameof(apaleoRolesCollector));
        }

        public EditTraceDialogViewModel EditTraceDialogViewModel { get; } = new EditTraceDialogViewModel();

        public Modal CreateTraceModalRef { get; set; }

        public Modal EditTraceModalRef { get; set; }

        protected ITraceModifierService TraceModifierService { get; }

        protected IApaleoOneNotificationService ApaleoOneNotificationService { get; }

        protected IFileService FileService { get; }

        public async Task InitializeAsync()
        {
            await InitializeContextAsync();

            await LoadTracesAsync();

            await LoadApaleoRolesAsync();
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
                var filesCreatedResult = await CreateTraceFileAsync(replaceTraceItemModel.Id);
                var filesDeletedResult = await DeleteTraceFileRangeAsync();
                if (filesCreatedResult && filesDeletedResult)
                {
                    HideEditTraceModal();
                }
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
            EditTraceDialogViewModel.TraceFiles = traceItemModel.Files;

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

        public async Task<bool> CreateTraceFileAsync(int traceId)
        {
            var createTraceFileModels = EditTraceDialogViewModel.GetCreateTraceFileItemModels(traceId);
            if (createTraceFileModels.Count > 0)
            {
                var result = await FileService.CreateTraceFileAsync(createTraceFileModels);
                if (result.Success)
                {
                    await RefreshAsync();
                }
                else
                {
                    var errorMessage = result.ErrorMessage.ValueOrException(new NotImplementedException());

                    await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);

                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteTraceFileRangeAsync()
        {
            var traceFileIds = EditTraceDialogViewModel.GetTraceFilesToDelete();
            if (traceFileIds.Count > 0)
            {
                var result = await FileService.DeleteTraceFileRangeAsync(traceFileIds);
                if (result.Success)
                {
                    await RefreshAsync();
                }
                else
                {
                    var errorMessage = result.ErrorMessage.ValueOrException(new NotImplementedException());

                    await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);

                    return false;
                }
            }

            return true;
        }

        protected abstract Task LoadTracesAsync();

        protected abstract Task RefreshAsync();

        private async Task LoadApaleoRolesAsync()
        {
            var roles = await _apaleoRolesCollector.GetRolesAsync();

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
