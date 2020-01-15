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
        private readonly IApaleoOneService _apaleoOneService;
        private readonly IApaleoRolesCollectorService _apaleoRolesCollector;

        protected TracesBaseViewModel(
            ITraceModifierService traceModifierService,
            IToastService toastService,
            IHttpContextAccessor httpContextAccessor,
            IRequestContext requestContext,
            IApaleoOneService apaleoOneService,
            IApaleoRolesCollectorService apaleoRolesCollector)
            : base(httpContextAccessor, requestContext)
        {
            TraceModifierService = Check.NotNull(traceModifierService, nameof(traceModifierService));
            _toastService = Check.NotNull(toastService, nameof(toastService));
            _apaleoOneService = Check.NotNull(apaleoOneService, nameof(apaleoOneService));
            _apaleoRolesCollector = Check.NotNull(apaleoRolesCollector, nameof(apaleoRolesCollector));

            SortedGroupedTracesDictionary = new SortedDictionary<DateTime, List<TraceItemModel>>();
            OverdueTraces = new List<TraceItemModel>();
            EditTraceDialogViewModel = new EditTraceDialogViewModel();
            CurrentDayIncrease = 5;
        }

        public EditTraceDialogViewModel EditTraceDialogViewModel { get; }

        public Modal CreateTraceModalRef { get; set; }

        public Modal EditTraceModalRef { get; set; }

        public List<TraceItemModel> OverdueTraces { get; }

        public SortedDictionary<DateTime, List<TraceItemModel>> SortedGroupedTracesDictionary { get; }

        public string LoadedUntilDateMessage { get; private set; }

        public string LoadMoreDaysText { get; private set; }

        public int CurrentDayIncrease { get; protected set; }

        protected ITraceModifierService TraceModifierService { get; }

        protected DateTime CurrentFromDate { get; set; }

        protected DateTime CurrentToDate { get; set; }

        public async Task LoadAsync()
        {
            await InitializeContextAsync();

            // On initialization we just load from today to tomorrow
            var from = DateTime.Today;
            var to = DateTime.Today.AddDays(1);

            await LoadTracesAsync(from, to);
            await LoadOverdueTracesAsync();

            UpdateLoadedUntilText();

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
                await LoadTracesAsync(CurrentFromDate, CurrentToDate);
                await LoadOverdueTracesAsync();

                ShowToastMessage(true, TextConstants.TraceUpdatedSuccessfullyMessage);
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

        public async Task DeleteItemAsync(TraceItemModel trace)
        {
            var deleteResult = await TraceModifierService.DeleteTraceAsync(trace.Id);

            if (deleteResult.Success)
            {
                RemoveTraceFromList(trace);

                ShowToastMessage(true, TextConstants.TraceDeletedSuccessfullyMessage);
            }
            else
            {
                var errorMessage = deleteResult.ErrorMessage.Match(
                    v => v,
                    () => throw new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }
        }

        public async Task CompleteTraceAsync(TraceItemModel trace)
        {
            var completeResult = await TraceModifierService.MarkTraceAsCompleteAsync(trace.Id);

            if (completeResult.Success)
            {
                RemoveTraceFromList(trace);

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

        public async Task NavigateToReservationAsync(TraceItemModel trace)
        {
            var navigationResult = await _apaleoOneService.NavigateToReservationAsync(trace);

            if (navigationResult.Success)
            {
                return;
            }

            var errorMessage = navigationResult.ErrorMessage.Match(
                v => v,
                () => throw new NotImplementedException());

            ShowToastMessage(false, errorMessage);
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

        public abstract Task LoadNextDaysAsync();

        protected abstract Task LoadTracesAsync(DateTime from, DateTime toDateTime);

        protected abstract Task LoadOverdueTracesAsync();

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

        protected void AddTraceToDictionary(TraceItemModel trace)
        {
            if (SortedGroupedTracesDictionary.ContainsKey(trace.DueDate))
            {
                var existentTraces = SortedGroupedTracesDictionary[trace.DueDate];
                if (existentTraces.Exists(item => item.Id == trace.Id))
                {
                    return;
                }

                existentTraces.Add(trace);
            }
            else
            {
                SortedGroupedTracesDictionary.Add(
                    trace.DueDate,
                    new List<TraceItemModel>
                    {
                        trace
                    });
            }
        }

        protected void LoadSortedDictionaryFromList(IReadOnlyList<TraceItemModel> traces)
        {
            SortedGroupedTracesDictionary.Clear();

            var groupedTraces = traces.GroupBy(x => x.DueDate).ToList();

            foreach (var group in groupedTraces)
            {
                SortedGroupedTracesDictionary.Add(group.Key, group.ToList());
            }
        }

        protected void UpdateLoadedUntilText()
        {
            LoadedUntilDateMessage =
                string.Format(TextConstants.TracesLoadedUntilTextFormat, CurrentToDate.ToShortDateString());

            LoadMoreDaysText = string.Format(TextConstants.TracesLoadMoreButtonTextFormat, CurrentDayIncrease);
        }

        private void RemoveTraceFromList(TraceItemModel trace)
        {
            if (SortedGroupedTracesDictionary.ContainsKey(trace.DueDate))
            {
                SortedGroupedTracesDictionary[trace.DueDate].Remove(trace);

                if (SortedGroupedTracesDictionary[trace.DueDate].Count == 0)
                {
                    SortedGroupedTracesDictionary.Remove(trace.DueDate);
                }
            }
            else if (OverdueTraces.Contains(trace))
            {
                OverdueTraces.Remove(trace);
            }
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