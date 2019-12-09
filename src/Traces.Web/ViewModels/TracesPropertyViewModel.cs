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
using Traces.Web.Interfaces;
using Traces.Web.Models;
using Traces.Web.Services;

namespace Traces.Web.ViewModels
{
    public class TracesPropertyViewModel : TracesBaseViewModel, ITraceModifier
    {
        private readonly ITracesCollectorService _tracesCollectorService;
        private readonly NavigationManager _navigationManager;
        private string _currentPropertyId;

        public TracesPropertyViewModel(
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

        public override async Task LoadAsync()
        {
            await InitializeContextAsync();
            await LoadTracesAsync();
        }

        public async Task<bool> CreateOrEditTraceAsync()
        {
            var result = EditTraceModificationModel.IsReplace
                ? await ReplaceTraceItemAsync()
                : await CreateTraceItemAsync();

            if (result)
            {
                HideCreateTraceModal();
            }

            return result;
        }

        protected override async Task<bool> CreateTraceItemAsync()
        {
            var createTraceItemModel = EditTraceModificationModel.GetCreateTraceItemModel();
            createTraceItemModel.PropertyId = _currentPropertyId;

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
            var tracesResult = await _tracesCollectorService.GetTracesForPropertyAsync(_currentPropertyId);

            if (tracesResult.Success)
            {
                var traces = tracesResult.Result.ValueOr(new List<TraceItemModel>());

                foreach (var trace in traces)
                {
                    Traces.Add(trace);
                }
            }
        }

        private void LoadCurrentReservationId()
            => _currentPropertyId = ExtractQueryParameterFromUrl(ApaleoOneConstants.PropertyIdQueryParameter);

        private string ExtractQueryParameterFromUrl(string parameterKey)
        {
            var uri = new Uri(_navigationManager.Uri);

            var queryNameValue = HttpUtility.ParseQueryString(uri.Query);

            return queryNameValue[parameterKey];
        }
    }
}