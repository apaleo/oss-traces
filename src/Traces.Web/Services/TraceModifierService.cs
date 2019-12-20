using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NodaTime.Extensions;
using Optional;
using Traces.Common.Exceptions;
using Traces.Common.Utils;
using Traces.Core.Models;
using Traces.Core.Services;
using Traces.Web.Models;
using Traces.Web.Utils;

namespace Traces.Web.Services
{
    public class TraceModifierService : ITraceModifierService
    {
        private readonly ITraceService _traceService;
        private readonly ILogger _logger;

        public TraceModifierService(ITraceService traceService, ILogger<TraceModifierService> logger)
        {
            _traceService = Check.NotNull(traceService, nameof(traceService));
            _logger = Check.NotNull(logger, nameof(logger));
        }

        public async Task<ResultModel<bool>> MarkTraceAsCompleteAsync(int id)
        {
            try
            {
                var result = await _traceService.CompleteTraceAsync(id);

                return new ResultModel<bool>
                {
                    Result = result.Some(),
                    Success = true
                };
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TraceModifierService)}.{nameof(MarkTraceAsCompleteAsync)} - Exception while trying to mark trace as complete with Id {id}");

                return new ResultModel<bool>
                {
                    Result = Option.None<bool>(),
                    Success = false,
                    ErrorMessage = ex.Message.Some()
                };
            }
        }

        public async Task<ResultModel<TraceItemModel>> CreateTraceAsync(CreateTraceItemModel createTraceItemModel)
        {
            try
            {
                var createTraceDto = new CreateTraceDto
                {
                    Title = createTraceItemModel.Title,
                    Description = createTraceItemModel.Description.SomeNotNull(),
                    DueDate = createTraceItemModel.DueDate.ToLocalDateTime().Date,
                    PropertyId = createTraceItemModel.PropertyId,
                    ReservationId = createTraceItemModel.ReservationId.SomeNotNull(),
                    AssignedRole = createTraceItemModel.AssignedRole.SomeNotNull()
                };

                var traceDto = await _traceService.CreateTraceAsync(createTraceDto);

                var traceItemModel = traceDto.ConvertToTraceItemModel();

                return new ResultModel<TraceItemModel>
                {
                    Result = traceItemModel.Some(),
                    Success = true
                };
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TraceModifierService)}.{nameof(CreateTraceAsync)} - Exception while trying to create trace");

                return new ResultModel<TraceItemModel>
                {
                    Success = false,
                    ErrorMessage = ex.Message.Some()
                };
            }
        }

        public async Task<ResultModel<TraceItemModel>> CreateTraceWithReservationIdAsync(CreateTraceItemModel createTraceItemModel)
        {
            try
            {
                var createTraceDto = new CreateTraceDto
                {
                    Title = createTraceItemModel.Title,
                    Description = createTraceItemModel.Description.SomeNotNull(),
                    DueDate = createTraceItemModel.DueDate.ToLocalDateTime().Date,
                    ReservationId = createTraceItemModel.ReservationId.SomeNotNull(),
                    AssignedRole = createTraceItemModel.AssignedRole.SomeNotNull()
                };

                var traceDto = await _traceService.CreateTraceFromReservationAsync(createTraceDto);

                var traceItemModel = traceDto.ConvertToTraceItemModel();

                return new ResultModel<TraceItemModel>
                {
                    Result = traceItemModel.Some(),
                    Success = true
                };
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TraceModifierService)}.{nameof(CreateTraceWithReservationIdAsync)} - Exception while trying to create reservation for reservation with Id {createTraceItemModel.ReservationId}");

                return new ResultModel<TraceItemModel>
                {
                    Success = false,
                    ErrorMessage = ex.Message.Some()
                };
            }
        }

        public async Task<ResultModel<bool>> ReplaceTraceAsync(ReplaceTraceItemModel replaceTraceItemModel)
        {
            try
            {
                var replaceTraceDto = new ReplaceTraceDto
                {
                    Title = replaceTraceItemModel.Title,
                    Description = replaceTraceItemModel.Description.SomeWhen(t => !string.IsNullOrWhiteSpace(t)),
                    DueDate = replaceTraceItemModel.DueDate.ToLocalDateTime().Date,
                    AssignedRole = replaceTraceItemModel.AssignedRole.SomeNotNull()
                };

                var replaceResult = await _traceService.ReplaceTraceAsync(replaceTraceItemModel.Id, replaceTraceDto);

                return new ResultModel<bool>
                {
                    Success = true,
                    Result = replaceResult.Some()
                };
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TraceModifierService)}.{nameof(ReplaceTraceAsync)} - Exception while trying to replace trace with Id {replaceTraceItemModel.Id}");

                return new ResultModel<bool>
                {
                    Success = false,
                    ErrorMessage = ex.Message.Some()
                };
            }
        }

        public async Task<ResultModel<bool>> DeleteTraceAsync(int id)
        {
            try
            {
                var deleteResult = await _traceService.DeleteTraceAsync(id);

                return new ResultModel<bool>
                {
                    Result = deleteResult.Some(),
                    Success = true
                };
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(TraceModifierService)}.{nameof(DeleteTraceAsync)} - Exception while deleting trace with id {id}");

                return new ResultModel<bool>
                {
                    Success = false,
                    ErrorMessage = ex.Message.Some()
                };
            }
        }
    }
}