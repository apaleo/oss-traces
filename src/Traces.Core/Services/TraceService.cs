using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NodaTime;
using Optional;
using Optional.Unsafe;
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

        public TraceService(ITraceRepository traceRepository)
        {
            _traceRepository = Check.NotNull(traceRepository, nameof(traceRepository));
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

        public async Task<Option<int>> CreateTraceAsync(CreateTraceDto createTraceDto)
        {
            Check.NotNull(createTraceDto, nameof(createTraceDto));

            if (string.IsNullOrWhiteSpace(createTraceDto.Title) ||
                createTraceDto.DueDate.ToDateTimeUnspecified() == DateTime.MinValue)
            {
                return Option.None<int>();
            }

            var trace = new Trace
            {
                Description = createTraceDto.Description.ValueOrDefault(),
                State = TraceStateEnum.Active,
                Title = createTraceDto.Title,
                DueDateUtc = createTraceDto.DueDate
            };

            _traceRepository.Insert(trace);

            await _traceRepository.SaveAsync();

            return trace.Id.Some();
        }

        public async Task<bool> ReplaceTraceAsync(int id, ReplaceTraceDto replaceTraceDto)
        {
            Check.NotNull(replaceTraceDto, nameof(replaceTraceDto));

            if (string.IsNullOrWhiteSpace(replaceTraceDto.Title) ||
                replaceTraceDto.DueDate.ToDateTimeUnspecified() == DateTime.MinValue)
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
            trace.DueDateUtc = replaceTraceDto.DueDate;

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

            trace.CompletedUtc = SystemClock.Instance.GetCurrentInstant().InUtc().Date;
            trace.State = TraceStateEnum.Completed;

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
            trace.State = trace.DueDateUtc < SystemClock.Instance.GetCurrentInstant().InUtc().Date ? TraceStateEnum.Obsolete : TraceStateEnum.Active;

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
            CompletedDate = trace.CompletedUtc?.Some() ?? Option.None<LocalDate>(),
            DueDate = trace.DueDateUtc,
            Id = trace.Id
        };
    }
}