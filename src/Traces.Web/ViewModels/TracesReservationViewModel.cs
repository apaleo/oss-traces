using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Extensions;
using Traces.Common.Utils;
using Traces.Web.Models;
using Traces.Web.Services;
using Traces.Web.Utils;

namespace Traces.Web.ViewModels
{
    public class TracesReservationViewModel : TracesBaseViewModel
    {
        private readonly ITracesCollectorService _tracesCollectorService;
        private readonly NavigationManager _navigationManager;
        private string _currentReservationId;

        public TracesReservationViewModel(
            ITracesCollectorService tracesCollectorService,
            ITraceModifierService traceModifierService,
            IToastService toastService,
            NavigationManager navigationManager,
            IRequestContext requestContext,
            IHttpContextAccessor httpContextAccessor,
            IApaleoOneService apaleoOneService,
            IApaleoRolesCollectorService apaleoRolesCollector)
            : base(traceModifierService, toastService, httpContextAccessor, requestContext, apaleoOneService, apaleoRolesCollector)
        {
            _navigationManager = Check.NotNull(navigationManager, nameof(navigationManager));
            _tracesCollectorService = Check.NotNull(tracesCollectorService, nameof(tracesCollectorService));

            LoadCurrentReservationId();
        }

        public SortedDictionary<DateTime, List<TraceItemModel>> CompletedTracesDictionary { get; } = new SortedDictionary<DateTime, List<TraceItemModel>>();

        public override async Task CreateTraceItemAsync()
        {
            var createTraceItemModel = EditTraceDialogViewModel.GetCreateTraceItemModel();
            createTraceItemModel.ReservationId = _currentReservationId;

            var createResult = await TraceModifierService.CreateTraceWithReservationIdAsync(createTraceItemModel);

            if (createResult.Success)
            {
                createResult.Result.MatchSome(ActiveTracesDictionary.AddTrace);

                ShowToastMessage(true, TextConstants.TraceCreatedSuccessfullyMessage);
            }
            else
            {
                var errorMessage = createResult.ErrorMessage.ValueOrException(new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }

            if (createResult.Success)
            {
                HideCreateTraceModal();
            }
        }

        protected override async Task LoadAsync()
        {
            // On initialization we just load from today to tomorrow
            var from = DateTime.Today;

            await LoadActiveTracesAsync(from, from);
            await LoadCompletedTracesAsync();
            await LoadOverdueTracesAsync();

            UpdateLoadedUntilText();
        }

        protected override async Task LoadActiveTracesAsync(DateTime from, DateTime toDateTime)
        {
            var tracesResult = await _tracesCollectorService.GetActiveTracesForReservationAsync(_currentReservationId, from);

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                ActiveTracesDictionary.LoadTraces(traces);

                CurrentFromDate = from;
                CurrentToDate = toDateTime;
            }
            else
            {
                var errorMessage = tracesResult.ErrorMessage.ValueOrException(new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }
        }

        protected override async Task LoadOverdueTracesAsync()
        {
            var tracesResult = await _tracesCollectorService.GetOverdueTracesForReservationAsync(_currentReservationId);

            if (tracesResult.Success)
            {
                OverdueTraces.Clear();

                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                foreach (var trace in traces)
                {
                    OverdueTraces.Add(trace);
                }
            }
            else
            {
                var errorMessage = tracesResult.ErrorMessage.ValueOrException(new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }
        }

        private async Task LoadCompletedTracesAsync()
        {
            var tracesResult = await _tracesCollectorService.GetCompletedTracesForReservationAsync(_currentReservationId);

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                CompletedTracesDictionary.LoadTraces(traces);
            }
            else
            {
                var errorMessage = tracesResult.ErrorMessage.ValueOrException(new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }
        }

        private void LoadCurrentReservationId()
            => _currentReservationId = UrlQueryParameterExtractor.ExtractQueryParameterFromManager(_navigationManager, ApaleoOneConstants.ReservationIdQueryParameter);
    }
}