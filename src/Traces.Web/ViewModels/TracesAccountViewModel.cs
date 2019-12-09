using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Http;
using Traces.Common;
using Traces.Common.Utils;
using Traces.Web.Models;
using Traces.Web.Services;

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
            IHttpContextAccessor httpContextAccessor)
            : base(traceModifierService, toastService, httpContextAccessor, requestContext)
        {
            _tracesCollectorService = Check.NotNull(tracesCollectorService, nameof(tracesCollectorService));
        }

        protected override async Task LoadTracesAsync()
        {
            var tracesResult = await _tracesCollectorService.GetTracesAsync();

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                foreach (var trace in traces)
                {
                    Traces.Add(trace);
                }
            }
        }
    }
}