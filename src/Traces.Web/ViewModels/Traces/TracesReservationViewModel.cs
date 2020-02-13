using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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
            IApaleoOneNavigationService apaleoOneNavigationService,
            IApaleoRolesCollectorService apaleoRolesCollector,
            IApaleoOneNotificationService apaleoOneNotificationService)
            : base(traceModifierService, httpContextAccessor, requestContext, apaleoOneNavigationService, apaleoRolesCollector, apaleoOneNotificationService)
        {
            _navigationManager = Check.NotNull(navigationManager, nameof(navigationManager));
            _tracesCollectorService = Check.NotNull(tracesCollectorService, nameof(tracesCollectorService));

            LoadCurrentReservationId();
        }

        public SortedDictionary<DateTime, List<TraceItemModel>> CompletedTracesDictionary { get; } =
            new SortedDictionary<DateTime, List<TraceItemModel>>(new DescendingComparer<DateTime>());

        public bool IsCompletedTracesVisible { get; set; } = false;

        public bool HasCompletedTraces => CompletedTracesDictionary.Count > 0;

        public string CompletedTracesCheckBoxText { get; set; }

        public override async Task CreateTraceItemAsync()
        {
            var createTraceItemModel = EditTraceDialogViewModel.GetCreateTraceItemModel();
            createTraceItemModel.ReservationId = _currentReservationId;

            var createResult = await TraceModifierService.CreateTraceWithReservationIdAsync(createTraceItemModel);

            if (createResult.Success)
            {
                createResult.Result.MatchSome(ActiveTracesDictionary.AddTrace);

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
            await LoadActiveTracesAsync();
            await LoadCompletedTracesAsync();
            await LoadOverdueTracesAsync();

            UpdateCompletedTracesText();
        }

        protected override async Task RefreshAsync()
        {
            await LoadTracesAsync();
        }

        protected override void CompleteTraceInList(TraceItemModel trace)
        {
            base.CompleteTraceInList(trace);
            CompletedTracesDictionary.AddTrace(trace);
            UpdateCompletedTracesText();
        }

        protected override async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesAsync() => await _tracesCollectorService.GetOverdueTracesForReservationAsync(_currentReservationId);

        private async Task LoadActiveTracesAsync()
        {
            var tracesResult = await _tracesCollectorService.GetActiveTracesForReservationAsync(_currentReservationId);

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                ActiveTracesDictionary.LoadTraces(traces);
            }
            else
            {
                var errorMessage = tracesResult.ErrorMessage.ValueOrException(new NotImplementedException());

                await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
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

                await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
            }
        }

        private void UpdateCompletedTracesText()
        {
            var elementCount = CompletedTracesDictionary.Values.Sum(list => list.Count);

            CompletedTracesCheckBoxText =
                string.Format(TextConstants.TracesShowCompletedCheckboxTextFormat, elementCount);
        }

        private void LoadCurrentReservationId()
            => _currentReservationId = UrlQueryParameterExtractor.ExtractQueryParameterFromManager(_navigationManager, AppConstants.ReservationIdQueryParameter);
    }
}
