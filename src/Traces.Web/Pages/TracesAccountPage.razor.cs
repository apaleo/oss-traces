using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Traces.Web.Models;
using Traces.Web.Services;

namespace Traces.Web.Pages
{
    public partial class TracesAccountPage
    {
        [Inject]
        private ITracesCollectorService TracesCollectorService { get; set; }

        protected override async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesAsync() => await TracesCollectorService.GetOverdueTracesAsync();

        protected override async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetActiveTracesAsync(DateTime from, DateTime toDateTime) => await TracesCollectorService.GetActiveTracesAsync(from, toDateTime);
    }
}
