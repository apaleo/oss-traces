using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Traces.Common;
using Traces.Common.Utils;
using Traces.Web.Models;
using Traces.Web.Services;
using Traces.Web.Services.Apaleo;
using Traces.Web.Services.ApaleoOne;

namespace Traces.Web.ViewModels.Traces
{
    public class TracesAccountViewModel : TracesDateAwareViewModel
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

        protected override async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesAsync() => await _tracesCollectorService.GetOverdueTracesAsync();

        protected override async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetActiveTracesAsync(DateTime @from, DateTime toDateTime) => await _tracesCollectorService.GetActiveTracesAsync(from, toDateTime);
    }
}
