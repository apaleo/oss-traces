using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NodaTime;
using Optional;
using Optional.Unsafe;
using Traces.ApaleoClients.Booking.Models;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Enums;
using Traces.Common.Exceptions;
using Traces.Common.Utils;
using Traces.Core.ClientFactories;
using Traces.Core.Models;
using Traces.Core.Repositories;
using Traces.Data.Entities;

namespace Traces.Core.Services
{
    public class TraceService : ITraceService
    {
        private readonly ITraceRepository _traceRepository;
        private readonly IRequestContext _requestContext;
        private readonly IApaleoClientFactory _apaleoClientFactory;

        public TraceService(ITraceRepository traceRepository, IRequestContext requestContext, IApaleoClientFactory apaleoClientFactory)
        {
            _traceRepository = Check.NotNull(traceRepository, nameof(traceRepository));
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
            _apaleoClientFactory = Check.NotNull(apaleoClientFactory, nameof(apaleoClientFactory));
        }

        public async Task<IReadOnlyList<TraceDto>> GetTracesAsync()
        {
            var traces = await _traceRepository.GetAllForTenantAsync();

            return ConvertToTraceDto(traces);
        }

        public async Task<IReadOnlyList<TraceDto>> GetActiveTracesAsync()
        {
            var traces = await _traceRepository.GetAllTracesForTenantAsync(t =>
                t.State == TraceStateEnum.Active &&
                t.DueDate >= LocalDate.FromDateTime(DateTime.Today));

            return ConvertToTraceDto(traces);
        }

        public async Task<IReadOnlyList<TraceDto>> GetActiveTracesForPropertyAsync(string propertyId)
        {
            Check.NotEmpty(propertyId, nameof(propertyId));

            var propertyTraces = await _traceRepository.GetAllTracesForTenantAsync(t =>
                t.State == TraceStateEnum.Active &&
                t.PropertyId == propertyId &&
                t.DueDate >= LocalDate.FromDateTime(DateTime.Today));

            return ConvertToTraceDto(propertyTraces);
        }

        public async Task<IReadOnlyList<TraceDto>> GetActiveTracesForReservationAsync(string reservationId)
        {
            Check.NotEmpty(reservationId, nameof(reservationId));

            var reservationTraces =
                await _traceRepository.GetAllTracesForTenantAsync(t =>
                    t.State == TraceStateEnum.Active &&
                    t.ReservationId == reservationId &&
                    t.DueDate >= LocalDate.FromDateTime(DateTime.Today));

            return ConvertToTraceDto(reservationTraces);
        }

        public async Task<IReadOnlyList<TraceDto>> GetOverdueTracesAsync()
        {
            var traces = await _traceRepository.GetAllTracesForTenantAsync(t =>
                (t.State == TraceStateEnum.Active || t.State == TraceStateEnum.Obsolete) &&
                t.DueDate < LocalDate.FromDateTime(DateTime.Today));

            return ConvertToTraceDto(traces);
        }

        public async Task<IReadOnlyList<TraceDto>> GetOverdueTracesForPropertyAsync(string propertyId)
        {
            Check.NotEmpty(propertyId, nameof(propertyId));

            var overdueTracesForProperty = await _traceRepository.GetAllTracesForTenantAsync(t =>
                (t.State == TraceStateEnum.Active || t.State == TraceStateEnum.Obsolete) &&
                t.DueDate < LocalDate.FromDateTime(DateTime.Today) &&
                t.PropertyId == propertyId);

            return ConvertToTraceDto(overdueTracesForProperty);
        }

        public async Task<IReadOnlyList<TraceDto>> GetOverdueTracesForReservationAsync(string reservationId)
        {
            Check.NotEmpty(reservationId, nameof(reservationId));

            var overdueTracesForReservation = await _traceRepository.GetAllTracesForTenantAsync(t =>
                (t.State == TraceStateEnum.Active || t.State == TraceStateEnum.Obsolete) &&
                t.DueDate < LocalDate.FromDateTime(DateTime.Today) &&
                t.ReservationId == reservationId);

            return ConvertToTraceDto(overdueTracesForReservation);
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
                DueDate = createTraceDto.DueDate,
                PropertyId = createTraceDto.PropertyId,
                ReservationId = createTraceDto.ReservationId.ValueOrDefault()
            };

            _traceRepository.Insert(trace);

            await _traceRepository.SaveAsync();

            return trace.Id;
        }

        /// <summary>
        /// This function exists to request the propertyId for this specific trace based on the reservationId
        /// </summary>
        /// <param name="createTraceDto">The dto with the information to create the trace</param>
        /// <returns>Id of the new trace</returns>
        public async Task<int> CreateTraceFromReservationAsync(CreateTraceDto createTraceDto)
        {
            var reservationId = createTraceDto.ReservationId.Match(
                v => v,
                () => throw new BusinessValidationException(TextConstants.NoReservationIdProvidedErrorMessage));

            var propertyId = await GetPropertyIdFromReservationIdAsync(reservationId);

            createTraceDto.PropertyId = propertyId;

            return await CreateTraceAsync(createTraceDto);
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
            var currentSubjectId = _requestContext.SubjectId;

            trace.CompletedDate = SystemClock.Instance.GetCurrentInstant().InUtc().Date;
            trace.State = TraceStateEnum.Completed;
            trace.CompletedBy = currentSubjectId;

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

        private async Task<string> GetPropertyIdFromReservationIdAsync(string reservationId)
        {
            var apiClient = _apaleoClientFactory.CreateBookingApi();

            using (var requestResponse = await apiClient.BookingReservationsByIdGetWithHttpMessagesAsync(reservationId))
            {
                if (requestResponse.Response.IsSuccessStatusCode && requestResponse.Body is ReservationModel reservation)
                {
                    var propertyId = reservation.Property.Id;
                    if (string.IsNullOrWhiteSpace(propertyId))
                    {
                        throw new BusinessValidationException(TextConstants.FetchingDataFromApaleoForTracesErrorMessage);
                    }

                    return propertyId;
                }
                else
                {
                    throw new BusinessValidationException(TextConstants.FetchingDataFromApaleoForTracesErrorMessage);
                }
            }
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
            Id = trace.Id,
            PropertyId = trace.PropertyId,
            ReservationId = trace.ReservationId.SomeNotNull()
        };
    }
}