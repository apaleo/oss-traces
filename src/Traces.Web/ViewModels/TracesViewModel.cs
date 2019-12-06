using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Enums;
using Traces.Common.Utils;
using Traces.Web.Models;
using Traces.Web.Services;

namespace Traces.Web.ViewModels
{
    public class TracesViewModel : TracesBaseViewModel
    {
        private readonly ITracesCollectorService _tracesCollectorService;

        public TracesViewModel(
            ITracesCollectorService tracesCollectorService,
            ITraceModifierService traceModifierService,
            IToastService toastService,
            IRequestContext requestContext,
            IHttpContextAccessor httpContextAccessor)
            : base(traceModifierService, toastService, httpContextAccessor, requestContext)
        {
            _tracesCollectorService = Check.NotNull(tracesCollectorService, nameof(tracesCollectorService));
        }

        public override async Task LoadAsync()
        {
            await InitializeContextAsync();
            await LoadTracesAsync();
        }

        public override async Task<bool> CreateTraceItemAsync()
        {
            var createTraceItemModel = EditTraceModificationModel.GetCreateTraceItemModel();
            var createResult = await TraceModifierService.CreateTraceAsync(createTraceItemModel);

            if (createResult.Success)
            {
                createResult.Result.MatchSome(Traces.Add);

                ShowToastMessage(true, TextConstants.TraceCreatedSuccessfullyMessage);
            }
            else
            {
                var errorMessage = createResult.ErrorMessage.Match(
                    v => v,
                    () => throw new NotImplementedException());

                ShowToastMessage(false, errorMessage);
            }

            return createResult.Success;
        }

        private async Task LoadTracesAsync()
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