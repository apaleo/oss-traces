using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Http;
using Traces.Common;
using Traces.Common.Extensions;
using Traces.Common.Utils;
using Traces.Web.Models;
using Traces.Web.Services;
using Traces.Web.Utils;

namespace Traces.Web.ViewModels
{
    public class TracesAccountViewModel : TracesBaseViewModel
    {
        private readonly ITracesCollectorService _tracesCollectorService;

        public TracesAccountViewModel(
            ITracesCollectorService tracesCollectorService,
            ITraceModifierService traceModifierService,
            IToastService toastService,
            IRequestContext requestContext,
            IHttpContextAccessor httpContextAccessor,
            IApaleoOneService apaleoOneService,
            IApaleoRolesCollectorService apaleoRolesCollector)
            : base(
                traceModifierService,
                toastService,
                httpContextAccessor,
                requestContext,
                apaleoOneService,
                apaleoRolesCollector)
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

                ShowToastMessage(false, errorMessage);
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