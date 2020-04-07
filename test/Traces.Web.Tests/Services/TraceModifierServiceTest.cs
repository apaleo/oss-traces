using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime.Extensions;
using Optional;
using Optional.Unsafe;
using Traces.Common.Enums;
using Traces.Common.Exceptions;
using Traces.Core.Models;
using Traces.Core.Services.Traces;
using Traces.Testing;
using Traces.Web.Models;
using Traces.Web.Services;
using Xunit;

namespace Traces.Web.Tests.Services
{
    public class TraceModifierServiceTest : BaseTest
    {
        private readonly Mock<ITraceService> _traceServiceMock;
        private readonly ITraceModifierService _traceModifierService;
        private readonly Mock<ILogger<TraceModifierService>> _loggerMock = new Mock<ILogger<TraceModifierService>>();

        public TraceModifierServiceTest()
        {
            _traceServiceMock = MockRepository.Create<ITraceService>();

            _traceModifierService = new TraceModifierService(_traceServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ShouldBeAbleToCompleteTraceAsync()
        {
            const int testTraceId = 1;

            _traceServiceMock.Setup(x => x.CompleteTraceAsync(It.Is<int>(v => v == testTraceId)))
                .ReturnsAsync(true);

            var completeResult = await _traceModifierService.MarkTraceAsCompleteAsync(testTraceId);

            completeResult.Should().NotBeNull();
            completeResult.Success.Should().BeTrue();
            completeResult.ErrorMessage.HasValue.Should().BeFalse();
            completeResult.Result.HasValue.Should().BeTrue();

            var modelResult = completeResult.Result.ValueOrFailure();
            modelResult.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotBeAbleToCompleteWhenValidationExceptionIsThrownAsync()
        {
            const string exceptionMessage = "Nop nop nop";
            const int testTraceId = 2;

            _traceServiceMock.Setup(x => x.CompleteTraceAsync(It.Is<int>(v => v == testTraceId)))
                .ThrowsAsync(new BusinessValidationException(exceptionMessage));

            var completeResult = await _traceModifierService.MarkTraceAsCompleteAsync(testTraceId);

            completeResult.Should().NotBeNull();
            completeResult.Success.Should().BeFalse();
            completeResult.Result.HasValue.Should().BeFalse();
            completeResult.ErrorMessage.HasValue.Should().BeTrue();

            var errorMessage = completeResult.ErrorMessage.ValueOrFailure();
            errorMessage.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task ShouldBeAbleToCreateTraceAsync()
        {
            const int createdId = 100;
            const string testTraceTitle = "TestTitle";
            const string testTraceDescription = "TestDescription";
            const string testTracePropertyId = "PROP";
            var testTraceDueDate = DateTime.Now.Add(TimeSpan.FromDays(2)).Date;

            var testCreateTrace = new CreateTraceItemModel
            {
                Title = testTraceTitle,
                Description = testTraceDescription,
                DueDate = testTraceDueDate,
                PropertyId = testTracePropertyId
            };

            var resultTraceDto = new TraceDto
            {
                Id = createdId,
                Title = testTraceTitle,
                Description = testTraceDescription.Some(),
                DueDate = testTraceDueDate.ToLocalDateTime().Date,
                State = TraceState.Active,
                PropertyId = testTracePropertyId
            };

            _traceServiceMock.Setup(x => x.CreateTraceAsync(
                It.Is<CreateTraceDto>(v =>
                v.Title == testTraceTitle &&
                v.Description.ValueOrFailure() == testTraceDescription &&
                v.DueDate.ToDateTimeUnspecified().Date == testTraceDueDate)))
                .ReturnsAsync(resultTraceDto);

            var creationResult = await _traceModifierService.CreateTraceAsync(testCreateTrace);

            creationResult.Should().NotBeNull();
            creationResult.Success.Should().BeTrue();
            creationResult.ErrorMessage.HasValue.Should().BeFalse();
            creationResult.Result.HasValue.Should().BeTrue();

            var result = creationResult.Result.ValueOrFailure();
            result.Should().NotBeNull();
            result.Id.Should().Be(createdId);
            result.Title.Should().Be(testTraceTitle);
            result.Description.Should().Be(testTraceDescription);
            result.State.Should().Be(TraceState.Active);
            result.DueDate.Should().Be(testTraceDueDate);
            result.PropertyId.Should().Be(testTracePropertyId);
        }

        [Fact]
        public async Task ShouldNotCreateWhenAValidationExceptionIsThrownAsync()
        {
            const string exceptionMessage = "iBreak";
            const string testTraceTitle = "TestTitle";
            const string testTraceDescription = "TestDescription";
            var testTraceDueDate = DateTime.Now.AddDays(2);

            var testCreateTrace = new CreateTraceItemModel
            {
                Title = testTraceTitle,
                Description = testTraceDescription,
                DueDate = testTraceDueDate
            };

            _traceServiceMock.Setup(x => x.CreateTraceAsync(
                    It.Is<CreateTraceDto>(v =>
                        v.Title == testTraceTitle &&
                        v.Description.ValueOrFailure() == testTraceDescription &&
                        v.DueDate.ToDateTimeUnspecified().Date == testTraceDueDate.Date)))
                .ThrowsAsync(new BusinessValidationException(exceptionMessage));

            var notSuccessResult = await _traceModifierService.CreateTraceAsync(testCreateTrace);

            notSuccessResult.Success.Should().BeFalse();
            notSuccessResult.Result.HasValue.Should().BeFalse();
            notSuccessResult.ErrorMessage.HasValue.Should().BeTrue();

            var errorMessage = notSuccessResult.ErrorMessage.ValueOrFailure();
            errorMessage.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task ShouldReplaceTraceAsync()
        {
            const int testReplaceTraceId = 101;
            const string testReplaceTitle = "ReplaceMe";
            const string testReplaceDescription = "#DescribeIt";
            var testReplaceDate = DateTime.Now.AddDays(3);

            var testReplaceTrace = new ReplaceTraceItemModel
            {
                Title = testReplaceTitle,
                Description = testReplaceDescription,
                DueDate = testReplaceDate,
                Id = testReplaceTraceId
            };

            _traceServiceMock.Setup(x => x.ReplaceTraceAsync(
                It.Is<int>(v => v == testReplaceTraceId),
                It.Is<ReplaceTraceDto>(v => v.Title == testReplaceTitle &&
                                            v.Description.ValueOrFailure() == testReplaceDescription &&
                                            v.DueDate.ToDateTimeUnspecified().Date == testReplaceDate.Date)))
                .ReturnsAsync(true);

            var replaceResult = await _traceModifierService.ReplaceTraceAsync(testReplaceTrace);

            replaceResult.Should().NotBeNull();
            replaceResult.Success.Should().BeTrue();
            replaceResult.Result.HasValue.Should().BeTrue();
            replaceResult.ErrorMessage.HasValue.Should().BeFalse();

            var result = replaceResult.Result.ValueOrFailure();
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotReplaceWhenValidationExceptionIsThrownAsync()
        {
            const string exceptionMessage = "Think again";
            const int testReplaceTraceId = 101;
            const string testReplaceTitle = "ReplaceMe";
            const string testReplaceDescription = "#DescribeIt";
            var testReplaceDate = DateTime.Now.AddDays(3);

            var testReplaceTrace = new ReplaceTraceItemModel
            {
                Title = testReplaceTitle,
                Description = testReplaceDescription,
                DueDate = testReplaceDate,
                Id = testReplaceTraceId
            };

            _traceServiceMock.Setup(x => x.ReplaceTraceAsync(
                    It.Is<int>(v => v == testReplaceTraceId),
                    It.Is<ReplaceTraceDto>(v => v.Title == testReplaceTitle &&
                                                v.Description.ValueOrFailure() == testReplaceDescription &&
                                                v.DueDate.ToDateTimeUnspecified().Date == testReplaceDate.Date)))
                .ThrowsAsync(new BusinessValidationException(exceptionMessage));

            var replaceResult = await _traceModifierService.ReplaceTraceAsync(testReplaceTrace);

            replaceResult.Should().NotBeNull();
            replaceResult.Success.Should().BeFalse();
            replaceResult.Result.HasValue.Should().BeFalse();
            replaceResult.ErrorMessage.HasValue.Should().BeTrue();

            var errorMessage = replaceResult.ErrorMessage.ValueOrFailure();
            errorMessage.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task ShouldBeAbleToDeleteTraceAsync()
        {
            const int testTraceId = 69;

            _traceServiceMock.Setup(x => x.DeleteTraceAsync(It.Is<int>(v => v == testTraceId)))
                .ReturnsAsync(true);

            var deleteResult = await _traceModifierService.DeleteTraceAsync(testTraceId);

            deleteResult.Should().NotBeNull();
            deleteResult.Success.Should().BeTrue();
            deleteResult.Result.HasValue.Should().BeTrue();
            deleteResult.ErrorMessage.HasValue.Should().BeFalse();

            var result = deleteResult.Result.ValueOrFailure();
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotDeleteWhenValidationExceptionIsThrownAsync()
        {
            const string exceptionMessage = "Omg omg";
            const int testTraceId = 89;

            _traceServiceMock.Setup(x => x.DeleteTraceAsync(It.Is<int>(v => v == testTraceId)))
                .ThrowsAsync(new BusinessValidationException(exceptionMessage));

            var deleteResult = await _traceModifierService.DeleteTraceAsync(testTraceId);

            deleteResult.Should().NotBeNull();
            deleteResult.Success.Should().BeFalse();
            deleteResult.Result.HasValue.Should().BeFalse();
            deleteResult.ErrorMessage.HasValue.Should().BeTrue();

            var errorMessage = deleteResult.ErrorMessage.ValueOrFailure();
            errorMessage.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task ShouldBeAbleToCreateTraceWithReservationIdAsync()
        {
            const string testTraceTitle = "ReservationTrace";
            const string testTraceDescription = "ReservationTraceDescription";
            var testTraceDueDate = DateTime.Today.AddDays(2);
            const string testReservationId = "REALRESERVATION_1";
            const int resultTraceId = 10;
            const string resultPropertyId = "PROP";

            var createTraceItemModel = new CreateTraceItemModel
            {
                Title = testTraceTitle,
                Description = testTraceDescription,
                DueDate = testTraceDueDate,
                ReservationId = testReservationId
            };

            var resultTraceDto = new TraceDto
            {
                Id = resultTraceId,
                Title = testTraceTitle,
                Description = testTraceDescription.Some(),
                State = TraceState.Active,
                DueDate = testTraceDueDate.ToLocalDateTime().Date,
                PropertyId = resultPropertyId,
                ReservationId = testReservationId.Some()
            };

            _traceServiceMock.Setup(x => x.CreateTraceFromReservationAsync(It.Is<CreateTraceDto>(
                    t => t.Title == testTraceTitle &&
                         t.Description.ValueOrFailure() == testTraceDescription &&
                         t.DueDate == testTraceDueDate.ToLocalDateTime().Date &&
                         t.ReservationId.ValueOrFailure() == testReservationId &&
                         string.IsNullOrWhiteSpace(t.PropertyId))))
                .ReturnsAsync(resultTraceDto);

            var result = await _traceModifierService.CreateTraceWithReservationIdAsync(createTraceItemModel);

            result.Success.Should().BeTrue();

            var resultContent = result.Result.ValueOrFailure();

            resultContent.Id.Should().Be(resultTraceId);
            resultContent.Title.Should().Be(testTraceTitle);
            resultContent.Description.Should().Be(testTraceDescription);
            resultContent.State.Should().Be(TraceState.Active);
            resultContent.DueDate.Should().Be(testTraceDueDate);
            resultContent.ReservationId = testReservationId;
        }

        [Fact]
        public async Task ShouldNotBeAbleToCreateTraceWithReservationWhenValidationExceptionIsThrownAsync()
        {
            const string errorMessage = "Something went wrong";

            const string testTraceTitle = "ReservationTrace";
            const string testTraceDescription = "ReservationTraceDescription";
            var testTraceDueDate = DateTime.Today.AddDays(2);
            const string testReservationId = "REALRESERVATION_1";

            var createTraceItemModel = new CreateTraceItemModel
            {
                Title = testTraceTitle,
                Description = testTraceDescription,
                DueDate = testTraceDueDate,
                ReservationId = testReservationId
            };

            _traceServiceMock.Setup(x => x.CreateTraceFromReservationAsync(It.Is<CreateTraceDto>(
                    t => t.Title == testTraceTitle &&
                         t.Description.ValueOrFailure() == testTraceDescription &&
                         t.DueDate == testTraceDueDate.ToLocalDateTime().Date &&
                         t.ReservationId.ValueOrFailure() == testReservationId &&
                         string.IsNullOrWhiteSpace(t.PropertyId))))
                .ThrowsAsync(new BusinessValidationException(errorMessage));

            var result = await _traceModifierService.CreateTraceWithReservationIdAsync(createTraceItemModel);

            result.Success.Should().BeFalse();
            result.Result.HasValue.Should().BeFalse();
            result.ErrorMessage.ValueOrFailure().Should().Be(errorMessage);
        }
    }
}