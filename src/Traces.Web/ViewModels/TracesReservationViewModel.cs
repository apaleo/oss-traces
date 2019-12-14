using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Utils;
using Traces.Web.Models;
using Traces.Web.Services;

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
            IHttpContextAccessor httpContextAccessor)
            : base(traceModifierService, toastService, httpContextAccessor, requestContext)
        {
            _navigationManager = Check.NotNull(navigationManager, nameof(navigationManager));
            _tracesCollectorService = Check.NotNull(tracesCollectorService, nameof(tracesCollectorService));

            LoadCurrentReservationId();
        }

        public override async Task CreateTraceItemAsync()
        {
            var createTraceItemModel = EditTraceDialogViewModel.GetCreateTraceItemModel();
            createTraceItemModel.ReservationId = _currentReservationId;

            var createResult = await TraceModifierService.CreateTraceWithReservationIdAsync(createTraceItemModel);

            if (createResult.Success)
            {
                createResult.Result.MatchSome(AddTraceToDictionary);

                ShowToastMessage(true, TextConstants.TraceCreatedSuccessfullyMessage);
            }
            else
            {
                var errorMessage = createResult.ErrorMessage.Match(
                    v => v,
                    () => throw new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }

            if (createResult.Success)
            {
                HideCreateTraceModal();
            }
        }

        public override async Task LoadNextDaysAsync()
        {
            var loadFromDate = CurrentToDate.AddDays(1);
            var loadToDate = CurrentToDate.AddDays(CurrentDayIncrease);

            var tracesResult =
                await _tracesCollectorService.GetTracesForReservationAsync(
                    _currentReservationId,
                    loadFromDate,
                    loadToDate);

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                foreach (var trace in traces)
                {
                    AddTraceToDictionary(trace);
                }

                CurrentToDate = loadToDate;
                CurrentDayIncrease = 7;

                UpdateLoadedUntilText();
            }
            else
            {
                var errorMessage = tracesResult.ErrorMessage.Match(
                    v => v,
                    () => throw new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }
        }

        protected override async Task LoadTracesAsync(DateTime from, DateTime to)
        {
            var tracesResult = await _tracesCollectorService.GetTracesForReservationAsync(_currentReservationId, from, to);

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                LoadSortedDictionaryFromList(traces);

                CurrentFromDate = from;
                CurrentToDate = to;
            }
            else
            {
                var errorMessage = tracesResult.ErrorMessage.Match(
                    v => v,
                    () => throw new NotImplementedException());

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
                var errorMessage = tracesResult.ErrorMessage.Match(
                    v => v,
                    () => throw new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }
        }

        private void LoadCurrentReservationId()
            => _currentReservationId = ExtractQueryParameterFromUrl(ApaleoOneConstants.ReservationIdQueryParameter);

        private string ExtractQueryParameterFromUrl(string parameterKey)
        {
            var uri = new Uri(_navigationManager.Uri);

            var queryNameValue = HttpUtility.ParseQueryString(uri.Query);

            return queryNameValue[parameterKey];
        }
    }
}