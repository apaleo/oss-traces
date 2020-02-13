using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using NodaTime.Extensions;
using Optional;
using Optional.Unsafe;
using Traces.Common.Enums;
using Traces.Common.Exceptions;
using Traces.Core.Models;
using Traces.Core.Services;
using Traces.Testing;
using Traces.Web.Services;
using Xunit;

namespace Traces.Web.Tests.Services
{
    public class TracesCollectorServiceTest : BaseTest
    {
        private const string PropertyId = "PROP";
        private const string ReservationId = "THEREALDEAL-1";
        private const int FirstTraceId = 1;
        private const string FirstTraceTitle = "FirstTraceTitle";
        private const string FirstTraceDescription = "FirstTraceDescription";
        private const TraceState FirstTraceState = TraceState.Active;

        private const int SecondTraceId = 2;
        private const string SecondTraceTitle = "SecondTraceTitle";
        private const TraceState SecondTraceState = TraceState.Active;

        private const int ThirdTraceId = 3;
        private const string ThirdTraceTitle = "ThirdTraceTitle";
        private const string ThirdTraceDescription = "ThirdTraceDescription";
        private const TraceState ThirdTraceState = TraceState.Completed;

        private readonly LocalDate _firstTraceDueDate = DateTime.UtcNow.ToLocalDateTime().Date;
        private readonly LocalDate _secondTraceDueDate = DateTime.UtcNow.ToLocalDateTime().Date;
        private readonly LocalDate _thirdTraceDueDate = DateTime.UtcNow.ToLocalDateTime().Date;

        private readonly DateTime _testFromDate = DateTime.Today;
        private readonly DateTime _testToDate = DateTime.Today.AddDays(1);

        private readonly Mock<ITraceService> _traceServiceMock;
        private readonly Mock<ILogger<TracesCollectorService>> _loggerMock = new Mock<ILogger<TracesCollectorService>>();
        private readonly ITracesCollectorService _tracesCollectorService;

        public TracesCollectorServiceTest()
        {
            _traceServiceMock = MockRepository.Create<ITraceService>();
            _tracesCollectorService = new TracesCollectorService(_traceServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ShouldBeAbleToGetTracesAsync()
        {
            var testTraces = new List<TraceDto>
            {
                new TraceDto
                {
                    Id = FirstTraceId,
                    Title = FirstTraceTitle,
                    Description = FirstTraceDescription.Some(),
                    State = FirstTraceState,
                    DueDate = _firstTraceDueDate
                },
                new TraceDto
                {
                    Id = SecondTraceId,
                    Title = SecondTraceTitle,
                    State = SecondTraceState,
                    DueDate = _secondTraceDueDate
                },
                new TraceDto
                {
                    Id = ThirdTraceId,
                    Title = ThirdTraceTitle,
                    Description = ThirdTraceDescription.Some(),
                    State = ThirdTraceState,
                    DueDate = _thirdTraceDueDate
                }
            };

            _traceServiceMock.Setup(x => x.GetActiveTracesAsync(
                    It.Is<DateTime>(dt => dt == _testFromDate),
                    It.Is<DateTime>(dt => dt == _testToDate)))
                .ReturnsAsync(testTraces);

            var result = await _tracesCollectorService.GetActiveTracesAsync(_testFromDate, _testToDate);

            result.Should().NotBeNull();

            result.Success.Should().BeTrue();
            result.ErrorMessage.HasValue.Should().BeFalse();

            var resultTraces = result.Result.ValueOrFailure();

            resultTraces.Should().HaveCount(3);

            resultTraces[0].Id.Should().Be(FirstTraceId);
            resultTraces[0].Title.Should().Be(FirstTraceTitle);
            resultTraces[0].Description.Should().Be(FirstTraceDescription);
            resultTraces[0].DueDate.Should().Be(_firstTraceDueDate.ToDateTimeUnspecified());
            resultTraces[0].State.Should().Be(FirstTraceState);

            resultTraces[1].Id.Should().Be(SecondTraceId);
            resultTraces[1].Title.Should().Be(SecondTraceTitle);
            resultTraces[1].Description.Should().BeEmpty();
            resultTraces[1].DueDate.Should().Be(_secondTraceDueDate.ToDateTimeUnspecified());
            resultTraces[1].State.Should().Be(SecondTraceState);

            resultTraces[2].Id.Should().Be(ThirdTraceId);
            resultTraces[2].Title.Should().Be(ThirdTraceTitle);
            resultTraces[2].Description.Should().Be(ThirdTraceDescription);
            resultTraces[2].DueDate.Should().Be(_thirdTraceDueDate.ToDateTimeUnspecified());
            resultTraces[2].State.Should().Be(ThirdTraceState);
        }

        [Fact]
        public async Task ShouldCatchValidationExceptionAndReturnUnsuccessfulResultAsync()
        {
            const string exceptionMessage = "Traces do not exist";

            _traceServiceMock.Setup(x => x.GetActiveTracesAsync(
                    It.Is<DateTime>(dt => dt == _testFromDate),
                    It.Is<DateTime>(dt => dt == _testToDate)))
                .ThrowsAsync(new BusinessValidationException(exceptionMessage));

            var collectorResult = await _tracesCollectorService.GetActiveTracesAsync(_testFromDate, _testToDate);

            collectorResult.Success.Should().BeFalse();
            collectorResult.Result.HasValue.Should().BeFalse();
            collectorResult.ErrorMessage.HasValue.Should().BeTrue();

            var errorMessage = collectorResult.ErrorMessage.ValueOrFailure();

            errorMessage.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task ShouldGetTracesForPropertyAsync()
        {
            var testTraces = new List<TraceDto>
            {
                new TraceDto
                {
                    Id = FirstTraceId,
                    Title = FirstTraceTitle,
                    Description = FirstTraceDescription.Some(),
                    State = FirstTraceState,
                    DueDate = _firstTraceDueDate,
                    PropertyId = PropertyId
                },
                new TraceDto
                {
                    Id = ThirdTraceId,
                    Title = ThirdTraceTitle,
                    Description = ThirdTraceDescription.Some(),
                    State = ThirdTraceState,
                    DueDate = _thirdTraceDueDate,
                    PropertyId = PropertyId
                }
            };

            var testTraceFromDate = DateTime.Today;
            var testTraceToDate = DateTime.Today.AddDays(1);

            _traceServiceMock.Setup(x => x.GetActiveTracesForPropertyAsync(
                    It.Is<string>(v => v == PropertyId),
                    It.Is<DateTime>(v => v == testTraceFromDate),
                    It.Is<DateTime>(v => v == testTraceToDate)))
                .ReturnsAsync(testTraces);

            var result =
                await _tracesCollectorService.GetActiveTracesForPropertyAsync(PropertyId, testTraceFromDate, testTraceToDate);

            result.Success.Should().BeTrue();

            var resultContent = result.Result.ValueOrFailure();

            resultContent.Should().HaveCount(2);

            resultContent[0].Id.Should().Be(FirstTraceId);
            resultContent[0].Title.Should().Be(FirstTraceTitle);
            resultContent[0].Description.Should().Be(FirstTraceDescription);
            resultContent[0].DueDate.Should().Be(_firstTraceDueDate.ToDateTimeUnspecified());
            resultContent[0].State.Should().Be(FirstTraceState);
            resultContent[0].PropertyId.Should().Be(PropertyId);

            resultContent[1].Id.Should().Be(ThirdTraceId);
            resultContent[1].Title.Should().Be(ThirdTraceTitle);
            resultContent[1].Description.Should().Be(ThirdTraceDescription);
            resultContent[1].DueDate.Should().Be(_thirdTraceDueDate.ToDateTimeUnspecified());
            resultContent[1].State.Should().Be(ThirdTraceState);
            resultContent[1].PropertyId.Should().Be(PropertyId);
        }

        [Fact]
        public async Task ShouldBeAbleToGetTracesForReservationAsync()
        {
            var testTraces = new List<TraceDto>
            {
                new TraceDto
                {
                    Id = FirstTraceId,
                    Title = FirstTraceTitle,
                    Description = FirstTraceDescription.Some(),
                    State = FirstTraceState,
                    DueDate = _firstTraceDueDate,
                    PropertyId = PropertyId,
                    ReservationId = ReservationId.Some()
                }
            };

            _traceServiceMock.Setup(x => x.GetActiveTracesForReservationAsync(
                    It.Is<string>(v => v == ReservationId)))
                .ReturnsAsync(testTraces);

            var result =
                await _tracesCollectorService.GetActiveTracesForReservationAsync(
                    ReservationId);

            result.Success.Should().BeTrue();

            var resultContent = result.Result.ValueOrFailure();

            resultContent.Should().HaveCount(1);

            resultContent[0].Id.Should().Be(FirstTraceId);
            resultContent[0].Title.Should().Be(FirstTraceTitle);
            resultContent[0].Description.Should().Be(FirstTraceDescription);
            resultContent[0].DueDate.Should().Be(_firstTraceDueDate.ToDateTimeUnspecified());
            resultContent[0].State.Should().Be(FirstTraceState);
            resultContent[0].PropertyId.Should().Be(PropertyId);
            resultContent[0].ReservationId.Should().Be(ReservationId);
        }

        [Fact]
        public async Task ShouldBeAbleToGetOverdueTracesAsync()
        {
            var testTraces = new List<TraceDto>
            {
                new TraceDto
                {
                    Id = ThirdTraceId,
                    Title = ThirdTraceTitle,
                    Description = ThirdTraceDescription.Some(),
                    State = ThirdTraceState,
                    DueDate = _thirdTraceDueDate,
                    PropertyId = PropertyId
                }
            };

            _traceServiceMock.Setup(x => x.GetOverdueTracesAsync())
                .ReturnsAsync(testTraces);

            var result = await _tracesCollectorService.GetOverdueTracesAsync();

            result.Success.Should().BeTrue();

            var resultContent = result.Result.ValueOrFailure();
            resultContent[0].Id.Should().Be(ThirdTraceId);
            resultContent[0].Title.Should().Be(ThirdTraceTitle);
            resultContent[0].Description.Should().Be(ThirdTraceDescription);
            resultContent[0].DueDate.Should().Be(_thirdTraceDueDate.ToDateTimeUnspecified());
            resultContent[0].State.Should().Be(ThirdTraceState);
            resultContent[0].PropertyId.Should().Be(PropertyId);
        }

        [Fact]
        public async Task ShouldBeAbleToGetOverdueTracesForPropertyAsync()
        {
            var testTraces = new List<TraceDto>
            {
                new TraceDto
                {
                    Id = FirstTraceId,
                    Title = FirstTraceTitle,
                    Description = FirstTraceDescription.Some(),
                    State = FirstTraceState,
                    DueDate = _firstTraceDueDate,
                    PropertyId = PropertyId,
                    ReservationId = ReservationId.Some()
                },
                new TraceDto
                {
                    Id = SecondTraceId,
                    Title = SecondTraceTitle,
                    State = SecondTraceState,
                    DueDate = _secondTraceDueDate,
                    PropertyId = PropertyId
                },
                new TraceDto
                {
                    Id = ThirdTraceId,
                    Title = ThirdTraceTitle,
                    Description = ThirdTraceDescription.Some(),
                    State = ThirdTraceState,
                    DueDate = _thirdTraceDueDate,
                    PropertyId = PropertyId
                }
            };

            _traceServiceMock.Setup(x => x.GetOverdueTracesForPropertyAsync(
                    It.Is<string>(v => v == PropertyId)))
                .ReturnsAsync(testTraces);

            var result = await _tracesCollectorService.GetOverdueTracesForPropertyAsync(PropertyId);

            result.Success.Should().BeTrue();

            var resultContent = result.Result.ValueOrFailure();

            resultContent[0].Id.Should().Be(FirstTraceId);
            resultContent[0].Title.Should().Be(FirstTraceTitle);
            resultContent[0].Description.Should().Be(FirstTraceDescription);
            resultContent[0].DueDate.Should().Be(_firstTraceDueDate.ToDateTimeUnspecified());
            resultContent[0].State.Should().Be(FirstTraceState);
            resultContent[0].PropertyId.Should().Be(PropertyId);
            resultContent[0].ReservationId.Should().Be(ReservationId);

            resultContent[1].Id.Should().Be(SecondTraceId);
            resultContent[1].Title.Should().Be(SecondTraceTitle);
            resultContent[1].Description.Should().BeEmpty();
            resultContent[1].DueDate.Should().Be(_secondTraceDueDate.ToDateTimeUnspecified());
            resultContent[1].State.Should().Be(SecondTraceState);
            resultContent[1].PropertyId.Should().Be(PropertyId);

            resultContent[2].Id.Should().Be(ThirdTraceId);
            resultContent[2].Title.Should().Be(ThirdTraceTitle);
            resultContent[2].Description.Should().Be(ThirdTraceDescription);
            resultContent[2].DueDate.Should().Be(_thirdTraceDueDate.ToDateTimeUnspecified());
            resultContent[2].State.Should().Be(ThirdTraceState);
            resultContent[2].PropertyId.Should().Be(PropertyId);
        }

        [Fact]
        public async Task ShouldBeAbleToGetOverdueTracesForReservationAsync()
        {
            var testTraces = new List<TraceDto>
            {
                new TraceDto
                {
                    Id = SecondTraceId,
                    Title = SecondTraceTitle,
                    State = SecondTraceState,
                    DueDate = _secondTraceDueDate,
                    PropertyId = PropertyId,
                    ReservationId = ReservationId.Some()
                },
                new TraceDto
                {
                    Id = ThirdTraceId,
                    Title = ThirdTraceTitle,
                    Description = ThirdTraceDescription.Some(),
                    State = ThirdTraceState,
                    DueDate = _thirdTraceDueDate,
                    PropertyId = PropertyId,
                    ReservationId = ReservationId.Some()
                }
            };

            _traceServiceMock.Setup(x => x.GetOverdueTracesForReservationAsync(
                    It.Is<string>(v => v == ReservationId)))
                .ReturnsAsync(testTraces);

            var result = await _tracesCollectorService.GetOverdueTracesForReservationAsync(ReservationId);

            result.Success.Should().BeTrue();

            var resultContent = result.Result.ValueOrFailure();

            resultContent.Should().HaveCount(2);

            resultContent[0].Id.Should().Be(SecondTraceId);
            resultContent[0].Title.Should().Be(SecondTraceTitle);
            resultContent[0].Description.Should().BeEmpty();
            resultContent[0].DueDate.Should().Be(_secondTraceDueDate.ToDateTimeUnspecified());
            resultContent[0].State.Should().Be(SecondTraceState);
            resultContent[0].PropertyId.Should().Be(PropertyId);

            resultContent[1].Id.Should().Be(ThirdTraceId);
            resultContent[1].Title.Should().Be(ThirdTraceTitle);
            resultContent[1].Description.Should().Be(ThirdTraceDescription);
            resultContent[1].DueDate.Should().Be(_thirdTraceDueDate.ToDateTimeUnspecified());
            resultContent[1].State.Should().Be(ThirdTraceState);
            resultContent[1].PropertyId.Should().Be(PropertyId);
        }
    }
}