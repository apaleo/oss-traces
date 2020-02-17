using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public abstract class TracesDateAwareViewModel : TracesBaseViewModel
    {
        protected TracesDateAwareViewModel(
            ITraceModifierService traceModifierService,
            IHttpContextAccessor httpContextAccessor,
            IRequestContext requestContext,
            IApaleoOneNavigationService apaleoOneNavigationService,
            IApaleoRolesCollectorService apaleoRolesCollector,
            IApaleoOneNotificationService apaleoOneNotificationService)
            : base(traceModifierService, httpContextAccessor, requestContext, apaleoRolesCollector, apaleoOneNotificationService)
        {
            ApaleoOneNavigationService = Check.NotNull(apaleoOneNavigationService, nameof(apaleoOneNavigationService));
        }

        public SortedDictionary<DateTime, List<TraceItemModel>> ActiveTracesDictionary { get; } =
            new SortedDictionary<DateTime, List<TraceItemModel>>();

        public List<TraceItemModel> OverdueTraces { get; } = new List<TraceItemModel>();

        public string LoadedUntilDateMessage { get; private set; }

        public string LoadMoreDaysText { get; private set; }

        public int CurrentDayIncrease { get; private set; } = 5;

        public DateTime CurrentFromDate { get; private set; } = DateTime.Today;

        public DateTime CurrentToDate { get; set; }

        private IApaleoOneNavigationService ApaleoOneNavigationService { get; }

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
                await LoadOverdueTracesAsync();

                UpdateLoadedUntilText();
            }
        }

        public async Task NavigateToReservationAsync(TraceItemModel trace)
        {
            var navigationResult = await ApaleoOneNavigationService.NavigateToReservationAsync(trace);

            if (navigationResult.Success)
            {
                return;
            }

            var errorMessage = navigationResult.ErrorMessage.ValueOrException(new NotImplementedException());

            await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
        }

        public async Task LoadNextDaysAsync()
        {
            var loadFromDate = CurrentToDate.AddDays(1);
            var loadUntilDate = CurrentToDate.AddDays(CurrentDayIncrease);

            var tracesResult = await GetActiveTracesAsync(loadFromDate, loadUntilDate);

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                foreach (var trace in traces)
                {
                    ActiveTracesDictionary.AddTrace(trace);
                }

                CurrentToDate = loadUntilDate;
                CurrentDayIncrease = 7;

                UpdateLoadedUntilText();
            }
            else
            {
                var errorMessage = tracesResult.ErrorMessage.ValueOrException(new NotImplementedException());

                await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
            }
        }

        protected override async Task LoadTracesAsync()
        {
            // On initialization we just load from today to tomorrow
            var from = DateTime.Today;
            var to = DateTime.Today.AddDays(1);

            await LoadActiveTracesAsync(from, to);
            await LoadOverdueTracesAsync();

            UpdateLoadedUntilText();
        }

        protected override async Task RefreshAsync()
        {
            await LoadActiveTracesAsync(CurrentFromDate, CurrentToDate);
            await LoadOverdueTracesAsync();
        }

        protected abstract Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetActiveTracesAsync(DateTime from, DateTime toDateTime);

        protected abstract Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesAsync();

        private async Task LoadOverdueTracesAsync()
        {
            if (CurrentFromDate == DateTime.Today)
            {
                var tracesResult = await GetOverdueTracesAsync();

                if (tracesResult.Success)
                {
                    OverdueTraces.Clear();

                    var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                    foreach (var trace in traces)
                    {
                        OverdueTraces.Add(trace);
                    }
                }
            }
            else
            {
                OverdueTraces.Clear();
            }
        }

        private async Task LoadActiveTracesAsync(DateTime from, DateTime toDateTime)
        {
            var tracesResult = await GetActiveTracesAsync(from, toDateTime);

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                ActiveTracesDictionary.LoadTraces(traces);

                CurrentFromDate = from;
                CurrentToDate = toDateTime;
            }
        }

        private void UpdateLoadedUntilText()
        {
            LoadedUntilDateMessage =
                string.Format(TextConstants.TracesLoadedUntilTextFormat, CurrentToDate.ToShortDateString());

            LoadMoreDaysText = string.Format(TextConstants.TracesLoadMoreButtonTextFormat, CurrentDayIncrease);
        }
    }
}
