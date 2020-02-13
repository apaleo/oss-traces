using System;
using System.Collections.Generic;
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
using Traces.Web.Utils;

namespace Traces.Web.ViewModels.Traces
{
    public abstract class TracesBaseViewModel : BaseViewModel
    {
        private readonly IApaleoOneNavigationService _apaleoOneNavigationService;
        private readonly IApaleoRolesCollectorService _apaleoRolesCollector;

        protected TracesBaseViewModel(
            ITraceModifierService traceModifierService,
            IHttpContextAccessor httpContextAccessor,
            IRequestContext requestContext,
            IApaleoOneNavigationService apaleoOneNavigationService,
            IApaleoRolesCollectorService apaleoRolesCollector,
            IApaleoOneNotificationService apaleoOneNotificationService)
            : base(httpContextAccessor, requestContext)
        {
            TraceModifierService = Check.NotNull(traceModifierService, nameof(traceModifierService));
            _apaleoOneNavigationService = Check.NotNull(apaleoOneNavigationService, nameof(apaleoOneNavigationService));
            _apaleoRolesCollector = Check.NotNull(apaleoRolesCollector, nameof(apaleoRolesCollector));
            ApaleoOneNotificationService = Check.NotNull(apaleoOneNotificationService, nameof(apaleoOneNotificationService));
        }

        public EditTraceDialogViewModel EditTraceDialogViewModel { get; } = new EditTraceDialogViewModel();

        public Modal CreateTraceModalRef { get; set; }

        public Modal EditTraceModalRef { get; set; }

        public List<TraceItemModel> OverdueTraces { get; } = new List<TraceItemModel>();

        public SortedDictionary<DateTime, List<TraceItemModel>> ActiveTracesDictionary { get; } =
            new SortedDictionary<DateTime, List<TraceItemModel>>();

        public string LoadedUntilDateMessage { get; private set; }

        public string LoadMoreDaysText { get; private set; }

        public int CurrentDayIncrease { get; protected set; } = 5;

        public DateTime CurrentFromDate { get; protected set; } = DateTime.Today;

        protected ITraceModifierService TraceModifierService { get; }

        protected IApaleoOneNotificationService ApaleoOneNotificationService { get; }

        protected DateTime CurrentToDate { get; set; }

        public async Task InitializeAsync()
        {
            await InitializeContextAsync();

            await LoadAsync();

            await LoadApaleoRolesAsync();
        }

        /// <summary>
        /// The traces for the given from date are loaded. The overdue traces are also loaded if the date is set to today.
        /// If the param from lies before the date of today, then nothing happens.
        /// </summary>
        /// <param name="from">The date that will be used to load the traces.</param>
        public async Task LoadFromDateAsync(DateTime from)
        {
            if (from >= DateTime.Today)
            {
                var to = from.AddDays(1);

                await LoadActiveTracesAsync(from, to);

                if (from.Date == DateTime.Today)
                {
                    await LoadOverdueTracesAsync();
                }
                else
                {
                    OverdueTraces.Clear();
                }

                UpdateLoadedUntilText();
            }
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
                await LoadActiveTracesAsync(CurrentFromDate, CurrentToDate);
                await LoadOverdueTracesAsync();

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
                RemoveTraceFromList(trace);

                await ApaleoOneNotificationService.ShowSuccessAsync(TextConstants.TraceDeletedSuccessfullyMessage);
            }
            else
            {
                var errorMessage = deleteResult.ErrorMessage.ValueOrException(new NotImplementedException());

                await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
            }
        }

        public async Task CompleteTraceAsync(TraceItemModel trace)
        {
            var completeResult = await TraceModifierService.MarkTraceAsCompleteAsync(trace.Id);

            if (completeResult.Success)
            {
                RemoveTraceFromList(trace);

                await ApaleoOneNotificationService.ShowSuccessAsync(TextConstants.TraceMarkedAsCompletedMessage);
            }
            else
            {
                var errorMessage = completeResult.ErrorMessage.ValueOrException(new NotImplementedException());

                await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
            }
        }

        public async Task NavigateToReservationAsync(TraceItemModel trace)
        {
            var navigationResult = await _apaleoOneNavigationService.NavigateToReservationAsync(trace);

            if (navigationResult.Success)
            {
                return;
            }

            var errorMessage = navigationResult.ErrorMessage.ValueOrException(new NotImplementedException());

            await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
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

        public virtual Task LoadNextDaysAsync() => throw new NotImplementedException();

        protected abstract Task LoadActiveTracesAsync(DateTime from, DateTime toDateTime);

        protected abstract Task LoadOverdueTracesAsync();

        protected virtual async Task LoadAsync()
        {
            // On initialization we just load from today to tomorrow
            var from = DateTime.Today;
            var to = DateTime.Today.AddDays(1);

            await LoadActiveTracesAsync(from, to);
            await LoadOverdueTracesAsync();

            UpdateLoadedUntilText();
        }

        protected void UpdateLoadedUntilText()
        {
            LoadedUntilDateMessage =
                string.Format(TextConstants.TracesLoadedUntilTextFormat, CurrentToDate.ToShortDateString());

            LoadMoreDaysText = string.Format(TextConstants.TracesLoadMoreButtonTextFormat, CurrentDayIncrease);
        }

        private void RemoveTraceFromList(TraceItemModel trace)
        {
            ActiveTracesDictionary.RemoveTrace(trace);
            OverdueTraces.Remove(trace);
        }

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
