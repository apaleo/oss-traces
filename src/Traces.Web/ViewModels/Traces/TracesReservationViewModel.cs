using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Enums;
using Traces.Common.Extensions;
using Traces.Common.Utils;
using Traces.Web.Models;
using Traces.Web.Services;
using Traces.Web.Services.Apaleo;
using Traces.Web.Services.ApaleoOne;
using Traces.Web.Utils;

namespace Traces.Web.ViewModels.Traces
{
    public class TracesReservationViewModel : TracesBaseViewModel
    {
        private readonly ITracesCollectorService _tracesCollectorService;
        private readonly NavigationManager _navigationManager;
        private string _currentReservationId;

        public TracesReservationViewModel(
            ITracesCollectorService tracesCollectorService,
            ITraceModifierService traceModifierService,
            NavigationManager navigationManager,
            IRequestContext requestContext,
            IHttpContextAccessor httpContextAccessor,
            IApaleoRolesCollectorService apaleoRolesCollector,
            IApaleoOneNotificationService apaleoOneNotificationService)
            : base(traceModifierService, httpContextAccessor, requestContext, apaleoRolesCollector, apaleoOneNotificationService)
        {
            _navigationManager = Check.NotNull(navigationManager, nameof(navigationManager));
            _tracesCollectorService = Check.NotNull(tracesCollectorService, nameof(tracesCollectorService));

            LoadCurrentReservationId();
        }

        public SortedDictionary<DateTime, List<TraceItemModel>> AllTracesDictionary { get; } = new SortedDictionary<DateTime, List<TraceItemModel>>();

        public SortedDictionary<DateTime, List<TraceItemModel>> ActiveTracesDictionary { get; } = new SortedDictionary<DateTime, List<TraceItemModel>>();

        public List<TraceItemModel> OverdueTraces { get; private set; } = new List<TraceItemModel>();

        public bool IsCompletedTracesVisible { get; set; } = false;

        public bool HasCompletedTraces { get; private set; } = false;

        public string CompletedTracesCheckBoxText { get; set; }

        public override async Task CreateTraceItemAsync()
        {
            var createTraceItemModel = EditTraceDialogViewModel.GetCreateTraceItemModel();
            createTraceItemModel.ReservationId = _currentReservationId;

            var createResult = await TraceModifierService.CreateTraceWithReservationIdAsync(createTraceItemModel);

            if (createResult.Success)
            {
                await RefreshAsync();

                await ApaleoOneNotificationService.ShowSuccessAsync(TextConstants.TraceCreatedSuccessfullyMessage);
            }
            else
            {
                var errorMessage = createResult.ErrorMessage.ValueOrException(new NotImplementedException());

                await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
            }

            if (createResult.Success)
            {
                HideCreateTraceModal();
            }
        }

        protected override async Task LoadTracesAsync()
        {
            await LoadAllTracesAsync();

            ActiveTracesDictionary.AddTracesRange(
                AllTracesDictionary
                    .SelectMany(dict => dict.Value)
                    .Where(trace => trace.State == TraceState.Active && trace.DueDate >= DateTime.Today)
                    .ToList());

            OverdueTraces =
                AllTracesDictionary
                    .SelectMany(dict => dict.Value)
                    .Where(trace => trace.State == TraceState.Active && trace.DueDate < DateTime.Today)
                    .ToList();

            AllTracesDictionary.SortValues(new TraceStateComparer());

            UpdateCompletedTracesText();
        }

        protected override async Task RefreshAsync()
        {
            await LoadTracesAsync();
        }

        private async Task LoadAllTracesAsync()
        {
            var tracesResult = await _tracesCollectorService.GetAllTracesForReservationAsync(_currentReservationId);

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                AllTracesDictionary.AddTracesRange(traces);
            }
            else
            {
                var errorMessage = tracesResult.ErrorMessage.ValueOrException(new NotImplementedException());

                await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
            }
        }

        private void UpdateCompletedTracesText()
        {
            var elementCount = AllTracesDictionary
                .SelectMany(dict => dict.Value)
                .Where(trace => trace.State == TraceState.Completed)
                .ToList()
                .Count;

            HasCompletedTraces = elementCount > 0;
            if (!HasCompletedTraces)
            {
                IsCompletedTracesVisible = false;
            }

            CompletedTracesCheckBoxText =
                string.Format(TextConstants.TracesShowCompletedCheckboxTextFormat, elementCount);
        }

        private void LoadCurrentReservationId()
            => _currentReservationId = UrlQueryParameterExtractor.ExtractQueryParameterFromManager(_navigationManager, AppConstants.ReservationIdQueryParameter);
    }
}
