using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Traces.Common.Constants;
using Traces.Common.Extensions;
using Traces.Web.Models;
using Traces.Web.Services;
using Traces.Web.Utils;

namespace Traces.Web.Pages
{
    public partial class TracesPropertyPage
    {
        private string _currentPropertyId;

        [Inject]
        private ITracesCollectorService TracesCollectorService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        public override async Task CreateTraceItemAsync()
        {
            var createTraceItemModel = EditTraceDialogViewModel.GetCreateTraceItemModel();
            createTraceItemModel.PropertyId = _currentPropertyId;

            var createResult = await TraceModifierService.CreateTraceAsync(createTraceItemModel);

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

        protected override async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesAsync() => await TracesCollectorService.GetOverdueTracesForPropertyAsync(_currentPropertyId);

        protected override async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetActiveTracesAsync(DateTime from, DateTime toDateTime) => await TracesCollectorService.GetActiveTracesForPropertyAsync(_currentPropertyId, from, toDateTime);

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadCurrentReservationId();
        }

        private void LoadCurrentReservationId()
            => _currentPropertyId = UrlQueryParameterExtractor.ExtractQueryParameterFromManager(NavigationManager, AppConstants.PropertyIdQueryParameter);
    }
}
