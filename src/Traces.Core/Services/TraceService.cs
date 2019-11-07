using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.Extensions;
using Optional;
using Optional.Unsafe;
using Traces.Common;
using Traces.Common.Enums;
using Traces.Common.Utils;
using Traces.Core.Models;
using Traces.Core.Repositories;
using Traces.Data.Entities;

namespace Traces.Core.Services
{
    public class TraceService : ITraceService
    {
        private readonly ITraceRepository _traceRepository;
        private readonly IRequestContext _requestContext;

        public TraceService(
            ITraceRepository traceRepository,
            IRequestContext requestContext)
        {
            _traceRepository = Check.NotNull(traceRepository, nameof(traceRepository));
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
        }

        public async Task<IReadOnlyList<TraceDto>> GetTracesAsync()
        {
            var traces = await _traceRepository.GetAllForTenantAsync();

            return ConvertToTraceDto(traces);
        }

        public async Task<Option<TraceDto>> GetTraceAsync(int id)
        {
            if (!await _traceRepository.ExistsAsync(t => t.Id == id))
            {
                return Option.None<TraceDto>();
            }

            var trace = await _traceRepository.GetAsync(id);

            return TraceToDto(trace).Some();
        }

        public async Task<Option<TraceDto>> CreateTraceAsync(CreateTraceDto createTraceDto)
        {
            Check.NotNull(createTraceDto, nameof(createTraceDto));

            if (string.IsNullOrWhiteSpace(createTraceDto.Title) ||
                createTraceDto.DueDate == ZonedDateTime.FromDateTimeOffset(DateTimeOffset.MinValue))
            {
                return Option.None<TraceDto>();
            }

            var trace = new Trace
            {
                Description = createTraceDto.Description.ValueOrDefault(),
                State = TaskStateEnum.Active,
                Title = createTraceDto.Title,
                DueDateUtc = createTraceDto.DueDate.ToInstant(),
                DueTime = createTraceDto.DueTime.ToNullable()
            };

            _traceRepository.Insert(trace);

            await _traceRepository.SaveAsync();

            return TraceToDto(trace).Some();
        }

        public async Task<bool> ReplaceTraceAsync(int id, ReplaceTraceDto replaceTraceDto)
        {
            Check.NotNull(replaceTraceDto, nameof(replaceTraceDto));

            if (string.IsNullOrWhiteSpace(replaceTraceDto.Title) || replaceTraceDto.DueDate ==
                ZonedDateTime.FromDateTimeOffset(DateTimeOffset.MinValue))
            {
                return false;
            }

            if (!await _traceRepository.ExistsAsync(t => t.Id == id))
            {
                return false;
            }

            var trace = await _traceRepository.GetAsync(id);

            trace.Description = replaceTraceDto.Description.ValueOrDefault();
            trace.Title = replaceTraceDto.Title;
            trace.DueDateUtc = replaceTraceDto.DueDate.ToInstant();
            trace.DueTime = replaceTraceDto.DueTime.ToNullable();

            await _traceRepository.SaveAsync();

            return true;
        }

        public async Task<bool> CompleteTraceAsync(int id)
        {
            if (!await _traceRepository.ExistsAsync(x => x.Id == id))
            {
                return false;
            }

            var trace = await _traceRepository.GetAsync(id);

            trace.CompletedUtc = DateTime.UtcNow.ToInstant();
            trace.CompletedBy = _requestContext.TenantId;
            trace.State = TaskStateEnum.Completed;

            await _traceRepository.SaveAsync();

            return true;
        }

        public async Task<bool> RevertCompleteAsync(int id)
        {
            if (!await _traceRepository.ExistsAsync(x => x.Id == id))
            {
                return false;
            }

            var trace = await _traceRepository.GetAsync(id);

            trace.CompletedUtc = null;
            trace.CompletedBy = string.Empty;
            trace.State = trace.DueDateUtc < DateTime.UtcNow.ToInstant() ? TaskStateEnum.Obsolete : TaskStateEnum.Active;

            await _traceRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteTraceAsync(int id)
        {
            if (!await _traceRepository.ExistsAsync(t => t.Id == id))
            {
                return false;
            }

            var deleted = await _traceRepository.DeleteAsync(id);

            if (deleted)
            {
                await _traceRepository.SaveAsync();
            }

            return deleted;
        }

        private static IReadOnlyList<TraceDto> ConvertToTraceDto(IReadOnlyList<Trace> traces)
        {
            var tracesDto = traces.Select(TraceToDto).ToList();
            return tracesDto;
        }

        private static TraceDto TraceToDto(Trace trace) => new TraceDto
        {
            Description = trace.Description.SomeNotNull(),
            State = trace.State,
            Title = trace.Title,
            CompletedBy = trace.CompletedBy.SomeNotNull(),
            CompletedDate = trace.CompletedUtc?.SomeNotNull().Map(x => x.InUtc()) ?? Option.None<ZonedDateTime>(),
            DueDate = trace.DueDateUtc.InUtc(),
            DueTime = trace.DueTime?.SomeNotNull() ?? Option.None<LocalTime>(),
            Id = trace.Id
        };
    }
}