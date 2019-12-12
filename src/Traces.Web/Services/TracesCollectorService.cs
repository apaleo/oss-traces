using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Optional;
using Traces.Common.Exceptions;
using Traces.Common.Utils;
using Traces.Core.Models;
using Traces.Core.Services;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public class TracesCollectorService : ITracesCollectorService
    {
        private readonly ITraceService _traceService;

        public TracesCollectorService(ITraceService traceService)
        {
            _traceService = Check.NotNull(traceService, nameof(traceService));
        }

        public async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesAsync()
        {
            try
            {
                var traceDtos = await _traceService.GetActiveTracesAsync();

                return SuccessModelFromTraceDtoList(traceDtos);
            }
            catch (BusinessValidationException e)
            {
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
            catch (BusinessValidationException e)
            {
                return FailModelWithErrorMessage(e.Message);
            }
        }

        public async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesForPropertyAsync(string propertyId)
        {
            try
            {
                var traceDtos = await _traceService.GetActiveTracesForPropertyAsync(propertyId);

                return SuccessModelFromTraceDtoList(traceDtos);
            }
            catch (BusinessValidationException e)
            {
                return FailModelWithErrorMessage(e.Message);
            }
        }

        public async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesForPropertyAsync(string propertyId)
        {
            try
            {
                var overdueTraces = await _traceService.GetOverdueTracesForPropertyAsync(propertyId);

                return SuccessModelFromTraceDtoList(overdueTraces);
            }
            catch (BusinessValidationException e)
            {
                return FailModelWithErrorMessage(e.Message);
            }
        }

        public async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesForReservationAsync(string reservationId)
        {
            try
            {
                var traceDtos = await _traceService.GetActiveTracesForReservationAsync(reservationId);

                return SuccessModelFromTraceDtoList(traceDtos);
            }
            catch (BusinessValidationException e)
            {
                return FailModelWithErrorMessage(e.Message);
            }
        }

        public async Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetOverdueTracesForReservationAsync(string reservationId)
        {
            try
            {
                var overdueTraces = await _traceService.GetOverdueTracesForReservationAsync(reservationId);

                return SuccessModelFromTraceDtoList(overdueTraces);
            }
            catch (BusinessValidationException e)
            {
                return FailModelWithErrorMessage(e.Message);
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
            traceDtos.Select(TraceDtoToModel).ToList();

        private static TraceItemModel TraceDtoToModel(TraceDto traceDto)
            => new TraceItemModel
            {
                Id = traceDto.Id,
                Description = traceDto.Description.ValueOr(string.Empty),
                Title = traceDto.Title,
                DueDate = traceDto.DueDate.ToDateTimeUnspecified(),
                State = traceDto.State,
                PropertyId = traceDto.PropertyId,
                ReservationId = traceDto.ReservationId.ValueOr(string.Empty)
            };
    }
}