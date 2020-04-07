using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using Optional;
using Optional.Unsafe;
using Traces.ApaleoClients.Booking.Models;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Enums;
using Traces.Common.Exceptions;
using Traces.Common.Extensions;
using Traces.Common.Utils;
using Traces.Core.ClientFactories;
using Traces.Core.Extensions;
using Traces.Core.Models;
using Traces.Core.Repositories;
using Traces.Core.Services.Files;
using Traces.Data.Entities;

namespace Traces.Core.Services.Traces
{
    public class TraceService : ITraceService
    {
        private readonly ITraceRepository _traceRepository;
        private readonly IRequestContext _requestContext;
        private readonly IApaleoClientsFactory _apaleoClientsFactory;
        private readonly ITraceFileService _traceFileService;

        public TraceService(ITraceRepository traceRepository, IRequestContext requestContext, IApaleoClientsFactory apaleoClientsFactory, ITraceFileService traceFileService)
        {
            _traceFileService = traceFileService;
            _traceRepository = Check.NotNull(traceRepository, nameof(traceRepository));
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
            _apaleoClientsFactory = Check.NotNull(apaleoClientsFactory, nameof(apaleoClientsFactory));
        }

        public async Task<IReadOnlyList<TraceDto>> GetTracesAsync()
        {
            var traces = await _traceRepository.GetAllForTenantAsync();

            return traces.ToTraceDtoList();
        }

        public async Task<IReadOnlyList<TraceDto>> GetActiveTracesAsync(DateTime from, DateTime toDateTime)
        {
            if (from > toDateTime)
            {
                throw new BusinessValidationException(TextConstants.DateIntervalErrorMessage);
            }

            var fromLocalDate = LocalDate.FromDateTime(from);
            var toLocalDate = LocalDate.FromDateTime(toDateTime);

            var traces = await _traceRepository.GetAllTracesForTenantAsync(t =>
                t.State == TraceState.Active &&
                t.DueDate >= fromLocalDate &&
                t.DueDate <= toLocalDate);

            return traces.ToTraceDtoList();
        }

        public async Task<IReadOnlyList<TraceDto>> GetActiveTracesForPropertyAsync(string propertyId, DateTime from, DateTime toDateTime)
        {
            Check.NotEmpty(propertyId, nameof(propertyId));

            if (from > toDateTime)
            {
                throw new BusinessValidationException(TextConstants.DateIntervalErrorMessage);
            }

            var fromLocalDate = LocalDate.FromDateTime(from);
            var toLocalDate = LocalDate.FromDateTime(toDateTime);

            var propertyTraces = await _traceRepository.GetAllTracesForTenantAsync(t =>
                t.State == TraceState.Active &&
                t.PropertyId == propertyId &&
                t.DueDate >= fromLocalDate &&
                t.DueDate <= toLocalDate);

            return propertyTraces.ToTraceDtoList();
        }

        public async Task<IReadOnlyList<TraceDto>> GetOverdueTracesAsync()
        {
            var todayDate = LocalDate.FromDateTime(DateTime.Today);
            var traces = await _traceRepository.GetAllTracesForTenantAsync(t =>
                t.State == TraceState.Active &&
                t.DueDate < todayDate);

            return traces.ToTraceDtoList();
        }

        public async Task<IReadOnlyList<TraceDto>> GetOverdueTracesForPropertyAsync(string propertyId)
        {
            Check.NotEmpty(propertyId, nameof(propertyId));

            var todayDate = LocalDate.FromDateTime(DateTime.Today);
            var overdueTracesForProperty = await _traceRepository.GetAllTracesForTenantAsync(t =>
                t.State == TraceState.Active &&
                t.DueDate < todayDate &&
                t.PropertyId == propertyId);

            return overdueTracesForProperty.ToTraceDtoList();
        }

        public async Task<IReadOnlyList<TraceDto>> GetAllTracesForReservationAsync(string reservationId)
        {
            Check.NotEmpty(reservationId, nameof(reservationId));

            var allTracesForReservation = await _traceRepository.GetAllTracesForTenantAsync(t =>
                t.ReservationId == reservationId);

            return allTracesForReservation.ToTraceDtoList();
        }

        public async Task<Option<TraceDto>> GetTraceAsync(int id)
        {
            if (!await _traceRepository.ExistsAsync(t => t.Id == id))
            {
                return Option.None<TraceDto>();
            }

            var trace = await _traceRepository.GetAsync(id);

            return trace.ToTraceDto().Some();
        }

        public async Task<TraceDto> CreateTraceAsync(CreateTraceDto createTraceDto)
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
                State = TraceState.Active,
                Title = createTraceDto.Title,
                DueDate = createTraceDto.DueDate,
                PropertyId = createTraceDto.PropertyId,
                ReservationId = createTraceDto.ReservationId.ValueOrDefault(),
                AssignedRole = createTraceDto.AssignedRole.ValueOrDefault()
            };

            _traceRepository.Insert(trace);

            await _traceRepository.SaveAsync();

            return trace.ToTraceDto();
        }

        /// <summary>
        /// This function exists to request the propertyId for this specific trace based on the reservationId
        /// </summary>
        /// <param name="createTraceDto">The dto with the information to create the trace</param>
        /// <returns>Id of the new trace</returns>
        public async Task<TraceDto> CreateTraceFromReservationAsync(CreateTraceDto createTraceDto)
        {
            var reservationId = createTraceDto.ReservationId.ValueOrException(
                new BusinessValidationException(TextConstants.NoReservationIdProvidedErrorMessage));

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
            trace.AssignedRole = replaceTraceDto.AssignedRole.ValueOrDefault();

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
            trace.State = TraceState.Completed;
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
            trace.CompletedBy = null;
            trace.State = TraceState.Active;

            await _traceRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteTraceAsync(int id)
        {
            if (!await _traceRepository.ExistsAsync(t => t.Id == id))
            {
                throw new BusinessValidationException(string.Format(TextConstants.TraceCouldNotBeFoundErrorMessageFormat, id));
            }

            await _traceFileService.DeleteTraceFileRangeAsync(traceFile => traceFile.TraceId == id);

            var deleted = await _traceRepository.DeleteAsync(id);

            if (deleted)
            {
                await _traceRepository.SaveAsync();
            }

            return deleted;
        }

        private async Task<string> GetPropertyIdFromReservationIdAsync(string reservationId)
        {
            var apiClient = _apaleoClientsFactory.GetBookingApi();

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
    }
}
