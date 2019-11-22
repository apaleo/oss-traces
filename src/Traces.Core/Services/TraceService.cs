using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NodaTime;
using Optional;
using Optional.Unsafe;
using Traces.Common.Constants;
using Traces.Common.Enums;
using Traces.Common.Exceptions;
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

        public async Task<IReadOnlyList<TraceDto>> GetActiveTracesAsync()
        {
            var traces = await _traceRepository.GetAllTracesForTenantAsync(t => t.State == TraceStateEnum.Active);

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

        public async Task<int> CreateTraceAsync(CreateTraceDto createTraceDto)
        {
            Check.NotNull(createTraceDto, nameof(createTraceDto));

            if (string.IsNullOrWhiteSpace(createTraceDto.Title) ||
                createTraceDto.DueDate < LocalDate.FromDateTime(DateTime.Today))
            {
                throw new BusinessValidationException(TextConstants.CreateTraceWithoutTitleOrFutureDateErrorMessage);
            }

            var trace = new Trace
            {
                Description = createTraceDto.Description.ValueOrDefault(),
                State = TraceStateEnum.Active,
                Title = createTraceDto.Title,
                DueDate = createTraceDto.DueDate
            };

            _traceRepository.Insert(trace);

            await _traceRepository.SaveAsync();

            return trace.Id;
        }

        public async Task<bool> ReplaceTraceAsync(int id, ReplaceTraceDto replaceTraceDto)
        {
            Check.NotNull(replaceTraceDto, nameof(replaceTraceDto));

            if (string.IsNullOrWhiteSpace(replaceTraceDto.Title) ||
                replaceTraceDto.DueDate < LocalDate.FromDateTime(DateTime.Today))
            {
                throw new BusinessValidationException(
                    string.Format(TextConstants.UpdateTraceWithoutTitleOrFutureDateErrorMessageFormat, id));
            }

            if (!await _traceRepository.ExistsAsync(t => t.Id == id))
            {
                return false;
            }

            var trace = await _traceRepository.GetAsync(id);

            trace.Description = replaceTraceDto.Description.ValueOrDefault();
            trace.Title = replaceTraceDto.Title;
            trace.DueDate = replaceTraceDto.DueDate;

            await _traceRepository.SaveAsync();

            return true;
        }

        public async Task<bool> CompleteTraceAsync(int id)
        {
            if (!await _traceRepository.ExistsAsync(x => x.Id == id))
            {
                throw new BusinessValidationException(string.Format(TextConstants.TraceCouldNotBeFoundErrorMessageFormat, id));
            }

            var trace = await _traceRepository.GetAsync(id);

            trace.CompletedDate = SystemClock.Instance.GetCurrentInstant().InUtc().Date;
            trace.State = TraceStateEnum.Completed;

            await _traceRepository.SaveAsync();

            return true;
        }

        public async Task<bool> RevertCompleteAsync(int id)
        {
            if (!await _traceRepository.ExistsAsync(x => x.Id == id))
            {
                throw new BusinessValidationException(string.Format(TextConstants.TraceCouldNotBeFoundErrorMessageFormat, id));
            }

            var trace = await _traceRepository.GetAsync(id);

            trace.CompletedDate = null;
            trace.State = trace.DueDate < LocalDate.FromDateTime(DateTime.Today) ? TraceStateEnum.Obsolete : TraceStateEnum.Active;

            await _traceRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteTraceAsync(int id)
        {
            if (!await _traceRepository.ExistsAsync(t => t.Id == id))
            {
                throw new BusinessValidationException(string.Format(TextConstants.TraceCouldNotBeFoundErrorMessageFormat, id));
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
            CompletedDate = trace.CompletedDate?.Some() ?? Option.None<LocalDate>(),
            DueDate = trace.DueDate,
            Id = trace.Id
        };
    }
}