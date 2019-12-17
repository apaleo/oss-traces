using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Optional;
using Traces.Common.Exceptions;
using Traces.Common.Utils;
using Traces.Core.Models;
using Traces.Core.Services;
using Traces.Web.Models;
using Traces.Web.Utils;

namespace Traces.Web.Services
{
    public class TracesCollectorService : ITracesCollectorService
    {
        private readonly ITraceService _traceService;
        private readonly ILogger _logger;

        public TracesCollectorService(ITraceService traceService, ILogger<TracesCollectorService> logger)
        {
            _traceService = Check.NotNull(traceService, nameof(traceService));
            _logger = Check.NotNull(logger, nameof(logger));
        }

        public async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesAsync(DateTime from, DateTime to)
        {
            try
            {
                var traceDtos = await _traceService.GetActiveTracesAsync(from, to);

                return SuccessModelFromTraceDtoList(traceDtos);
            }
            catch (BusinessValidationException e)
            {
                _logger.LogWarning(e, $"{nameof(TracesCollectorService)}.{nameof(GetTracesAsync)} Exception while trying to get all traces for account from {from} to {to}");
                return FailModelWithErrorMessage(e.Message);
            }
        }

        public async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesAsync()
        {
            try
            {
                var overdueTraces = await _traceService.GetOverdueTracesAsync();

                return SuccessModelFromTraceDtoList(overdueTraces);
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TracesCollectorService)}.{nameof(GetOverdueTracesAsync)} Exception while trying to get overdue traces for account");
                return FailModelWithErrorMessage(ex.Message);
            }
        }

        public async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesForPropertyAsync(string propertyId, DateTime from, DateTime to)
        {
            try
            {
                var traceDtos = await _traceService.GetActiveTracesForPropertyAsync(propertyId, from, to);

                return SuccessModelFromTraceDtoList(traceDtos);
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TracesCollectorService)}.{nameof(GetTracesForPropertyAsync)} - Exception while trying to get traces for property {propertyId} from {from} to {to}");
                return FailModelWithErrorMessage(ex.Message);
            }
        }

        public async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesForPropertyAsync(string propertyId)
        {
            try
            {
                var overdueTraces = await _traceService.GetOverdueTracesForPropertyAsync(propertyId);

                return SuccessModelFromTraceDtoList(overdueTraces);
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TracesCollectorService)}.{nameof(GetOverdueTracesForPropertyAsync)} - Exception while trying to get overdue traces for property with Id {propertyId}");
                return FailModelWithErrorMessage(ex.Message);
            }
        }

        public async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesForReservationAsync(string reservationId, DateTime from, DateTime to)
        {
            try
            {
                var traceDtos = await _traceService.GetActiveTracesForReservationAsync(reservationId, from, to);

                return SuccessModelFromTraceDtoList(traceDtos);
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TracesCollectorService)}.{nameof(GetTracesForReservationAsync)} - Exception while trying to get traces for reservation with Id {reservationId} and dates from {from} to {to}");
                return FailModelWithErrorMessage(ex.Message);
            }
        }

        public async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesForReservationAsync(string reservationId)
        {
            try
            {
                var overdueTraces = await _traceService.GetOverdueTracesForReservationAsync(reservationId);

                return SuccessModelFromTraceDtoList(overdueTraces);
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TracesCollectorService)}.{nameof(GetOverdueTracesForReservationAsync)} - Exception while trying to get overdue traces for reservation with Id {reservationId}");
                return FailModelWithErrorMessage(ex.Message);
            }
        }

        private static ResultModel<IReadOnlyList<TraceItemModel>> SuccessModelFromTraceDtoList(
            IReadOnlyList<TraceDto> traceDtoList)
            => new ResultModel<IReadOnlyList<TraceItemModel>>
            {
                Success = true,
                Result = TraceDtosToModels(traceDtoList).Some()
            };

        private static ResultModel<IReadOnlyList<TraceItemModel>> FailModelWithErrorMessage(string errorMessage)
            => new ResultModel<IReadOnlyList<TraceItemModel>>
            {
                Success = false,
                Result = Option.None<IReadOnlyList<TraceItemModel>>(),
                ErrorMessage = errorMessage.SomeNotNull()
            };

        private static IReadOnlyList<TraceItemModel> TraceDtosToModels(IReadOnlyList<TraceDto> traceDtos) =>
            traceDtos.Select(t => t.ConvertToTraceItemModel()).ToList();
    }
}