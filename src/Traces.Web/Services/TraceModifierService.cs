using System.Threading.Tasks;
using NodaTime.Extensions;
using Optional;
using Traces.Common.Enums;
using Traces.Common.Exceptions;
using Traces.Common.Utils;
using Traces.Core.Models;
using Traces.Core.Services;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public class TraceModifierService : ITraceModifierService
    {
        private readonly ITraceService _traceService;

        public TraceModifierService(ITraceService traceService)
        {
            _traceService = Check.NotNull(traceService, nameof(traceService));
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
            catch (BusinessValidationException e)
            {
                return new ResultModel<bool>
                {
                    Result = Option.None<bool>(),
                    Success = false,
                    ErrorMessage = e.Message.Some()
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
                    DueDate = createTraceItemModel.DueDate.ToLocalDateTime().Date
                };

                var traceId = await _traceService.CreateTraceAsync(createTraceDto);

                var traceItemModel = new TraceItemModel
                {
                    Id = traceId,
                    Title = createTraceItemModel.Title,
                    Description = createTraceItemModel.Description,
                    State = TraceStateEnum.Active,
                    DueDate = createTraceItemModel.DueDate
                };

                return new ResultModel<TraceItemModel>
                {
                    Result = traceItemModel.Some(),
                    Success = true
                };
            }
            catch (BusinessValidationException e)
            {
                return new ResultModel<TraceItemModel>
                {
                    Success = false,
                    ErrorMessage = e.Message.Some()
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
                    ReservationId = createTraceItemModel.ReservationId.SomeNotNull()
                };

                var traceId = await _traceService.CreateTraceFromReservationAsync(createTraceDto);

                var traceItemModel = new TraceItemModel
                {
                    Id = traceId,
                    Title = createTraceItemModel.Title,
                    Description = createTraceItemModel.Description,
                    State = TraceStateEnum.Active,
                    DueDate = createTraceItemModel.DueDate
                };

                return new ResultModel<TraceItemModel>
                {
                    Result = traceItemModel.Some(),
                    Success = true
                };
            }
            catch (BusinessValidationException e)
            {
                return new ResultModel<TraceItemModel>
                {
                    Success = false,
                    ErrorMessage = e.Message.Some()
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
                    DueDate = replaceTraceItemModel.DueDate.ToLocalDateTime().Date
                };

                var replaceResult = await _traceService.ReplaceTraceAsync(replaceTraceItemModel.Id, replaceTraceDto);

                return new ResultModel<bool>
                {
                    Success = true,
                    Result = replaceResult.Some()
                };
            }
            catch (BusinessValidationException e)
            {
                return new ResultModel<bool>
                {
                    Success = false,
                    ErrorMessage = e.Message.Some()
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
            catch (BusinessValidationException e)
            {
                return new ResultModel<bool>
                {
                    Success = false,
                    ErrorMessage = e.Message.Some()
                };
            }
        }
    }
}