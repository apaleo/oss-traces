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

        public async Task<Option<TraceDto>> GetTraceAsync(Guid id)
        {
            if (!await _traceRepository.ExistsAsync(t => t.EntityId == id))
            {
                return Option.None<TraceDto>();
            }

            var trace = await _traceRepository.GetAsync(id);

            return TraceToDto(trace).Some();
        }

        public async Task<Guid> CreateTraceAsync(CreateTraceDto createTraceDto)
        {
            Check.NotNull(createTraceDto, nameof(createTraceDto));

            if (string.IsNullOrWhiteSpace(createTraceDto.Title) ||
                createTraceDto.DueDate == ZonedDateTime.FromDateTimeOffset(DateTimeOffset.MinValue))
            {
                return Guid.Empty;
            }

            var trace = new Trace
            {
                Description = createTraceDto.Description.ValueOrDefault(),
                State = TaskStateEnum.Active,
                Title = createTraceDto.Title,
                DueDate = createTraceDto.DueDate.ToInstant(),
                DueTime = createTraceDto.DueTime.ToNullable()
            };

            _traceRepository.Insert(trace);

            await _traceRepository.SaveAsync();

            return trace.EntityId;
        }

        public async Task<bool> ReplaceTraceAsync(Guid id, ReplaceTraceDto replaceTraceDto)
        {
            Check.NotNull(replaceTraceDto, nameof(replaceTraceDto));

            if (!await _traceRepository.ExistsAsync(t => t.EntityId == id))
            {
                return false;
            }

            var trace = await _traceRepository.GetAsync(id);

            trace.Description = replaceTraceDto.Description.ValueOrDefault();
            trace.Title = replaceTraceDto.Title;
            trace.DueDate = replaceTraceDto.DueDate.ToInstant();
            trace.DueTime = replaceTraceDto.DueTime.ToNullable();

            await _traceRepository.SaveAsync();

            return true;
        }

        public async Task<bool> CompleteTraceAsync(Guid id)
        {
            if (!await _traceRepository.ExistsAsync(x => x.EntityId == id))
            {
                return false;
            }

            var trace = await _traceRepository.GetAsync(id);

            trace.CompletedDate = DateTime.UtcNow.ToInstant();
            trace.CompletedBy = _requestContext.TenantId;
            trace.State = TaskStateEnum.Completed;

            await _traceRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteTraceAsync(Guid id)
        {
            if (!await _traceRepository.ExistsAsync(t => t.EntityId == id))
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
            CompletedDate = trace.CompletedDate?.SomeNotNull().Map(x => x.InUtc()) ?? Option.None<ZonedDateTime>(),
            DueDate = trace.DueDate.InUtc(),
            DueTime = trace.DueTime?.SomeNotNull() ?? Option.None<TimeSpan>(),
            EntityId = trace.EntityId
        };
    }
}