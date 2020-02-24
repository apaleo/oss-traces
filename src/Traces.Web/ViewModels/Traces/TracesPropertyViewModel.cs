using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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
    public class TracesPropertyViewModel : TracesDateAwareViewModel
    {
        private readonly ITracesCollectorService _tracesCollectorService;
        private readonly NavigationManager _navigationManager;
        private string _currentPropertyId;

        public TracesPropertyViewModel(
            ITracesCollectorService tracesCollectorService,
            ITraceModifierService traceModifierService,
            IFileService fileService,
            NavigationManager navigationManager,
            IRequestContext requestContext,
            IHttpContextAccessor httpContextAccessor,
            IApaleoOneNavigationService apaleoOneNavigationService,
            IApaleoRolesCollectorService apaleoRolesCollector,
            IApaleoOneNotificationService apaleoOneNotificationService)
            : base(traceModifierService, fileService, httpContextAccessor, requestContext, apaleoOneNavigationService, apaleoRolesCollector, apaleoOneNotificationService)
        {
            _navigationManager = Check.NotNull(navigationManager, nameof(navigationManager));
            _tracesCollectorService = Check.NotNull(tracesCollectorService, nameof(tracesCollectorService));

            LoadCurrentReservationId();
        }

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
                await CreateTraceFileAsync(createResult.Result.ValueOrException(new NotImplementedException()).Id);
                await DeleteTraceFileRangeAsync();

                HideCreateTraceModal();
            }
        }

        protected override async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesAsync() => await _tracesCollectorService.GetOverdueTracesForPropertyAsync(_currentPropertyId);

        protected override async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetActiveTracesAsync(DateTime from, DateTime toDateTime) => await _tracesCollectorService.GetActiveTracesForPropertyAsync(_currentPropertyId, from, toDateTime);

        private void LoadCurrentReservationId()
            => _currentPropertyId = UrlQueryParameterExtractor.ExtractQueryParameterFromManager(_navigationManager, AppConstants.PropertyIdQueryParameter);
    }
}
