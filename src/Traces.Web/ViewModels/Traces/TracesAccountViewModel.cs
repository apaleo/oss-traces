using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Traces.Common;
using Traces.Common.Extensions;
using Traces.Common.Utils;
using Traces.Web.Models;
using Traces.Web.Services;
using Traces.Web.Services.Apaleo;
using Traces.Web.Services.ApaleoOne;
using Traces.Web.Utils;

namespace Traces.Web.ViewModels.Traces
{
    public class TracesAccountViewModel : TracesBaseViewModel
    {
        private readonly ITracesCollectorService _tracesCollectorService;

        public TracesAccountViewModel(
            ITracesCollectorService tracesCollectorService,
            ITraceModifierService traceModifierService,
            IRequestContext requestContext,
            IHttpContextAccessor httpContextAccessor,
            IApaleoOneNavigationService apaleoOneNavigationService,
            IApaleoRolesCollectorService apaleoRolesCollector,
            IApaleoOneNotificationService apaleoOneNotificationService)
            : base(
                traceModifierService,
                httpContextAccessor,
                requestContext,
                apaleoOneNavigationService,
                apaleoRolesCollector,
                apaleoOneNotificationService)
        {
            _tracesCollectorService = Check.NotNull(tracesCollectorService, nameof(tracesCollectorService));
        }

        public override async Task LoadNextDaysAsync()
        {
            var loadFromDate = CurrentToDate.AddDays(1);
            var loadUntilDate = CurrentToDate.AddDays(CurrentDayIncrease);

            var tracesResult = await _tracesCollectorService.GetActiveTracesAsync(loadFromDate, loadUntilDate);

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                foreach (var trace in traces)
                {
                    ActiveTracesDictionary.AddTrace(trace);
                }

                CurrentToDate = loadUntilDate;

                // After the first run, we always load the next 7 days
                CurrentDayIncrease = 7;

                UpdateLoadedUntilText();
            }
            else
            {
                var errorMessage = tracesResult.ErrorMessage.ValueOrException(new NotImplementedException());

                await ApaleoOneNotificationService.ShowErrorAsync(errorMessage);
            }
        }

        protected override async Task LoadActiveTracesAsync(DateTime from, DateTime toDateTime)
        {
            var tracesResult = await _tracesCollectorService.GetActiveTracesAsync(from, toDateTime);

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                ActiveTracesDictionary.LoadTraces(traces);

                CurrentFromDate = from;
                CurrentToDate = toDateTime;
            }
        }

        protected override async Task LoadOverdueTracesAsync()
        {
            var tracesResult = await _tracesCollectorService.GetOverdueTracesAsync();

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
    }
}
