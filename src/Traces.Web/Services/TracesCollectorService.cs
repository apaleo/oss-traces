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
                var traceDtos = await _traceService.GetTracesAsync();

                var traceModels = TraceDtosToModels(traceDtos);

                return new ResultModel<IReadOnlyList<TraceItemModel>>
                {
                    Success = true,
                    Result = traceModels.Some()
                };
            }
            catch (BusinessValidationException e)
            {
                return new ResultModel<IReadOnlyList<TraceItemModel>>
                {
                    Success = false,
                    Result = Option.None<IReadOnlyList<TraceItemModel>>(),
                    ErrorMessage = e.Message.Some()
                };
            }
        }

        private IReadOnlyList<TraceItemModel> TraceDtosToModels(IReadOnlyList<TraceDto> traceDtos) =>
            traceDtos.Select(TraceDtoToModel).ToList();

        private TraceItemModel TraceDtoToModel(TraceDto traceDto)
            => new TraceItemModel
            {
                Id = traceDto.Id,
                Description = traceDto.Description.ValueOr(string.Empty),
                Title = traceDto.Title,
                DueDate = traceDto.DueDate.ToDateTimeUnspecified(),
                State = traceDto.State
            };
    }
}