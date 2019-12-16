using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NodaTime;
using NodaTime.Extensions;
using Optional;
using Optional.Unsafe;
using Traces.Common;
using Traces.Common.Enums;
using Traces.Common.Exceptions;
using Traces.Core.ClientFactories;
using Traces.Core.Models;
using Traces.Core.Repositories;
using Traces.Core.Services;
using Traces.Data.Entities;
using Traces.Testing;
using Xunit;

namespace Traces.Core.Tests.Services
{
    public class TraceServiceTest : BaseTest
    {
        private const string TestSubjectId = "TEST";
        private const int TestActiveTraceId = 1;
        private const string TestActiveTraceDescription = "TestActiveDescription";
        private const string TestActiveTraceTitle = "TestActiveTitle";
        private const TraceStateEnum TestActiveTraceState = TraceStateEnum.Active;
        private const string TestActivePropertyId = "PROA";

        private const int TestObsoleteTraceId = 2;
        private const string TestObsoleteTraceDescription = "TestObsoleteDescription";
        private const string TestObsoleteTraceTitle = "TestObsoleteTitle";
        private const TraceStateEnum TestObsoleteTraceState = TraceStateEnum.Obsolete;
        private const string TestObsoletePropertyId = "PROO";
        private const string TestObsoleteReservationId = "RESO";

        private const int TestCompletedTraceId = 3;
        private const string TestCompletedTraceDescription = "TestCompletedDescription";
        private const string TestCompletedTraceTitle = "TestCompletedTitle";
        private const TraceStateEnum TestCompletedTraceState = TraceStateEnum.Completed;
        private const string TestCompletedPropertyId = "PROC";
        private const string TestCompletedReservationId = "RESC";

        private readonly LocalDate _testActiveTraceDueDate = DateTime.UtcNow.ToLocalDateTime().Date;
        private readonly LocalDate _testObsoleteTraceDueDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)).ToLocalDateTime().Date;
        private readonly LocalDate _testCompletedTraceDueDate = DateTime.UtcNow.Add(TimeSpan.FromHours(1)).ToLocalDateTime().Date;
        private readonly LocalDate _testCompletedDate = DateTime.UtcNow.ToLocalDateTime().Date;
        private readonly DateTime _testFromDate = DateTime.Today;
        private readonly DateTime _testToDate = DateTime.Today.AddDays(1);

        private readonly Mock<ITraceRepository> _traceRepositoryMock;
        private readonly Mock<IRequestContext> _requestContextMock;
        private readonly Mock<IApaleoClientsFactory> _apaleoClientFactoryMock;
        private readonly ITraceService _traceService;

        public TraceServiceTest()
        {
            _traceRepositoryMock = MockRepository.Create<ITraceRepository>();
            _requestContextMock = MockRepository.Create<IRequestContext>();
            _apaleoClientFactoryMock = MockRepository.Create<IApaleoClientsFactory>();
            _traceService = new TraceService(_traceRepositoryMock.Object, _requestContextMock.Object, _apaleoClientFactoryMock.Object);
        }

        [Fact]
        public async Task ShouldGetAllTracesAsync()
        {
            var testActiveTrace = new Trace
            {
                Id = TestActiveTraceId,
                Description = TestActiveTraceDescription,
                State = TestActiveTraceState,
                Title = TestActiveTraceTitle,
                DueDate = _testActiveTraceDueDate,
                PropertyId = TestActivePropertyId
            };

            var testObsoleteTrace = new Trace
            {
                Id = TestObsoleteTraceId,
                Description = TestObsoleteTraceDescription,
                State = TestObsoleteTraceState,
                Title = TestObsoleteTraceTitle,
                DueDate = _testObsoleteTraceDueDate,
                PropertyId = TestObsoletePropertyId,
                ReservationId = TestObsoleteReservationId
            };

            var testCompletedTrace = new Trace
            {
                Id = TestCompletedTraceId,
                Description = TestCompletedTraceDescription,
                State = TestCompletedTraceState,
                Title = TestCompletedTraceTitle,
                DueDate = _testCompletedTraceDueDate,
                CompletedDate = _testCompletedDate,
                PropertyId = TestCompletedPropertyId,
                ReservationId = TestCompletedReservationId
            };

            var testTraces = new List<Trace>
            {
                testActiveTrace,
                testObsoleteTrace,
                testCompletedTrace
            };

            _traceRepositoryMock.Setup(x => x.GetAllForTenantAsync())
                .ReturnsAsync(testTraces);

            var result = await _traceService.GetTracesAsync();

            result.Should().NotBeEmpty();
            result.Should().HaveCount(3);

            // Test first element is equivalent to trace testActiveTrace
            result[0].Id.Should().Be(TestActiveTraceId);
            result[0].Title.Should().Be(TestActiveTraceTitle);
            result[0].Description.ValueOrFailure().Should().Be(TestActiveTraceDescription);
            result[0].State.Should().Be(TestActiveTraceState);
            result[0].DueDate.Should().Be(_testActiveTraceDueDate);
            result[0].CompletedDate.HasValue.Should().BeFalse();
            result[0].PropertyId.Should().Be(TestActivePropertyId);
            result[0].ReservationId.HasValue.Should().BeFalse();

            // Test second element is equivalent to trace testObsoleteTrace
            result[1].Id.Should().Be(TestObsoleteTraceId);
            result[1].Title.Should().Be(TestObsoleteTraceTitle);
            result[1].Description.ValueOrFailure().Should().Be(TestObsoleteTraceDescription);
            result[1].State.Should().Be(TestObsoleteTraceState);
            result[1].DueDate.Should().Be(_testObsoleteTraceDueDate);
            result[1].CompletedDate.HasValue.Should().BeFalse();
            result[1].PropertyId.Should().Be(TestObsoletePropertyId);
            result[1].ReservationId.ValueOrFailure().Should().Be(TestObsoleteReservationId);

            // Test second element is equivalent to trace testCompletedTrace
            result[2].Id.Should().Be(TestCompletedTraceId);
            result[2].Title.Should().Be(TestCompletedTraceTitle);
            result[2].Description.ValueOrFailure().Should().Be(TestCompletedTraceDescription);
            result[2].State.Should().Be(TestCompletedTraceState);
            result[2].DueDate.Should().Be(_testCompletedTraceDueDate);
            result[2].CompletedDate.ValueOrFailure().Should().Be(_testCompletedDate);
            result[2].PropertyId.Should().Be(TestCompletedPropertyId);
            result[2].ReservationId.ValueOrFailure().Should().Be(TestCompletedReservationId);
        }

        [Fact]
        public async Task ShouldGetAllActiveTracesAsync()
        {
            var testActiveTrace = new Trace
            {
                Id = TestActiveTraceId,
                Description = TestActiveTraceDescription,
                State = TestActiveTraceState,
                Title = TestActiveTraceTitle,
                DueDate = _testActiveTraceDueDate,
                PropertyId = TestActivePropertyId
            };

            var testObsoleteTrace = new Trace
            {
                Id = TestObsoleteTraceId,
                Description = TestObsoleteTraceDescription,
                State = TestActiveTraceState,
                Title = TestObsoleteTraceTitle,
                DueDate = _testObsoleteTraceDueDate,
                PropertyId = TestObsoletePropertyId,
                ReservationId = TestObsoleteReservationId
            };

            var testCompletedTrace = new Trace
            {
                Id = TestCompletedTraceId,
                Description = TestCompletedTraceDescription,
                State = TestActiveTraceState,
                Title = TestCompletedTraceTitle,
                DueDate = _testCompletedTraceDueDate,
                CompletedDate = _testCompletedDate,
                PropertyId = TestCompletedPropertyId,
                ReservationId = TestCompletedReservationId
            };

            var testTraces = new List<Trace>
            {
                testActiveTrace,
                testObsoleteTrace,
                testCompletedTrace
            };

            _traceRepositoryMock.Setup(x => x.GetAllForTenantAsync())
                .ReturnsAsync(testTraces);

            var result = await _traceService.GetTracesAsync();

            result.Should().NotBeEmpty();
            result.Should().HaveCount(3);

            // Test first element is equivalent to trace testActiveTrace
            result[0].Id.Should().Be(TestActiveTraceId);
            result[0].Title.Should().Be(TestActiveTraceTitle);
            result[0].Description.ValueOrFailure().Should().Be(TestActiveTraceDescription);
            result[0].State.Should().Be(TestActiveTraceState);
            result[0].DueDate.Should().Be(_testActiveTraceDueDate);
            result[0].CompletedDate.HasValue.Should().BeFalse();
            result[0].PropertyId.Should().Be(TestActivePropertyId);
            result[0].ReservationId.HasValue.Should().BeFalse();

            // Test second element is equivalent to trace testObsoleteTrace
            result[1].Id.Should().Be(TestObsoleteTraceId);
            result[1].Title.Should().Be(TestObsoleteTraceTitle);
            result[1].Description.ValueOrFailure().Should().Be(TestObsoleteTraceDescription);
            result[1].State.Should().Be(TestActiveTraceState);
            result[1].DueDate.Should().Be(_testObsoleteTraceDueDate);
            result[1].CompletedDate.HasValue.Should().BeFalse();
            result[1].PropertyId.Should().Be(TestObsoletePropertyId);
            result[1].ReservationId.ValueOrFailure().Should().Be(TestObsoleteReservationId);

            // Test second element is equivalent to trace testCompletedTrace
            result[2].Id.Should().Be(TestCompletedTraceId);
            result[2].Title.Should().Be(TestCompletedTraceTitle);
            result[2].Description.ValueOrFailure().Should().Be(TestCompletedTraceDescription);
            result[2].State.Should().Be(TestActiveTraceState);
            result[2].DueDate.Should().Be(_testCompletedTraceDueDate);
            result[2].CompletedDate.ValueOrFailure().Should().Be(_testCompletedDate);
            result[2].PropertyId.Should().Be(TestCompletedPropertyId);
            result[2].ReservationId.ValueOrFailure().Should().Be(TestCompletedReservationId);
        }

        [Fact]
        public async Task ShouldGetEmptyListWhenNoTracesExistAsync()
        {
            _traceRepositoryMock.Setup(x => x.GetAllForTenantAsync())
                .ReturnsAsync(new List<Trace>());

            var result = await _traceService.GetTracesAsync();

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldFailToGetTracesWhenInvalidDateIntervalProvidedAsync()
        {
            var result = await Assert.ThrowsAsync<BusinessValidationException>(() =>
                _traceService.GetActiveTracesAsync(_testToDate, _testFromDate));

            result.Message.Should()
                .Be(
                    "The provided date interval is invalid. The end of the interval must be greater than the beginning");
        }

        [Fact]
        public async Task ShouldGetAllTracesForPropertyAsync()
        {
            var testActiveTrace = new Trace
            {
                Id = TestActiveTraceId,
                Description = TestActiveTraceDescription,
                State = TestActiveTraceState,
                Title = TestActiveTraceTitle,
                DueDate = _testActiveTraceDueDate,
                PropertyId = TestActivePropertyId
            };

            _traceRepositoryMock.Setup(x => x.GetAllTracesForTenantAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(new List<Trace>
                {
                    testActiveTrace
                });

            var result = await _traceService.GetActiveTracesForPropertyAsync(TestActivePropertyId, _testFromDate, _testToDate);

            result.Should().NotBeEmpty();
            result.Should().HaveCount(1);

            result[0].Id.Should().Be(TestActiveTraceId);
            result[0].Title.Should().Be(TestActiveTraceTitle);
            result[0].Description.ValueOrFailure().Should().Be(TestActiveTraceDescription);
            result[0].State.Should().Be(TestActiveTraceState);
            result[0].DueDate.Should().Be(_testActiveTraceDueDate);
            result[0].CompletedDate.HasValue.Should().BeFalse();
            result[0].PropertyId.Should().Be(TestActivePropertyId);
        }

        [Fact]
        public async Task ShouldBeAbleToGetTracesRelatedToPropertyAsync()
        {
            var result = await Assert.ThrowsAsync<BusinessValidationException>(() =>
                _traceService.GetActiveTracesForPropertyAsync(TestActivePropertyId, _testToDate, _testFromDate));

            result.Message.Should()
                .Be(
                    "The provided date interval is invalid. The end of the interval must be greater than the beginning");
        }

        [Fact]
        public async Task ShouldBeAbleToGetTracesRelatedToAReservationAsync()
        {
            var testCompletedTrace = new Trace
            {
                Id = TestCompletedTraceId,
                Description = TestCompletedTraceDescription,
                State = TestCompletedTraceState,
                Title = TestCompletedTraceTitle,
                DueDate = _testCompletedTraceDueDate,
                CompletedDate = _testCompletedDate,
                PropertyId = TestCompletedPropertyId,
                ReservationId = TestCompletedReservationId
            };

            _traceRepositoryMock.Setup(x => x.GetAllTracesForTenantAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(new List<Trace>
                {
                    testCompletedTrace
                });

            var result = await _traceService.GetActiveTracesForReservationAsync(TestCompletedReservationId, _testFromDate, _testToDate);

            result.Should().HaveCount(1);

            result[0].Id.Should().Be(TestCompletedTraceId);
            result[0].Title.Should().Be(TestCompletedTraceTitle);
            result[0].Description.ValueOrFailure().Should().Be(TestCompletedTraceDescription);
            result[0].State.Should().Be(TestCompletedTraceState);
            result[0].DueDate.Should().Be(_testCompletedTraceDueDate);
            result[0].CompletedDate.ValueOrFailure().Should().Be(_testCompletedDate);
            result[0].PropertyId.Should().Be(TestCompletedPropertyId);
            result[0].ReservationId.ValueOrFailure().Should().Be(TestCompletedReservationId);
        }

        [Fact]
        public async Task ShouldFailToGetReservationTracesWhenDateIntervalIsInvalidAsync()
        {
            var result = await Assert.ThrowsAsync<BusinessValidationException>(() =>
                _traceService.GetActiveTracesForReservationAsync(TestCompletedReservationId, _testToDate, _testFromDate));

            result.Message.Should()
                .Be(
                    "The provided date interval is invalid. The end of the interval must be greater than the beginning");
        }

        [Fact]
        public async Task ShouldGetSingleTraceAsync()
        {
            var testActiveTrace = new Trace
            {
                Id = TestActiveTraceId,
                Description = TestActiveTraceDescription,
                State = TraceStateEnum.Active,
                Title = TestActiveTraceTitle,
                DueDate = _testActiveTraceDueDate
            };

            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.GetAsync(It.Is<int>(v => v == TestActiveTraceId)))
                .ReturnsAsync(testActiveTrace);

            var result = await _traceService.GetTraceAsync(TestActiveTraceId);

            result.Should().NotBeNull();
            var resultDto = result.ValueOrFailure();

            resultDto.Id.Should().Be(TestActiveTraceId);
            resultDto.Title.Should().Be(TestActiveTraceTitle);
            resultDto.Description.ValueOrFailure().Should().Be(TestActiveTraceDescription);
            resultDto.State.Should().Be(TestActiveTraceState);
            resultDto.DueDate.Should().Be(_testActiveTraceDueDate);
            resultDto.CompletedDate.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldNotReturnTraceWhenDoesNotExistAsync()
        {
            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(false);

            var result = await _traceService.GetTraceAsync(TestObsoleteTraceId);

            result.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldCreateTraceAsync()
        {
            var createTraceDto = new CreateTraceDto
            {
                Description = TestActiveTraceDescription.Some(),
                Title = TestActiveTraceTitle,
                DueDate = _testActiveTraceDueDate,
                PropertyId = TestActivePropertyId
            };

            _traceRepositoryMock.Setup(x => x.Insert(
                It.Is<Trace>(t =>
                    t.Description == TestActiveTraceDescription &&
                    t.Title == TestActiveTraceTitle &&
                    t.State == TraceStateEnum.Active &&
                    t.DueDate == _testActiveTraceDueDate &&
                    t.PropertyId == TestActivePropertyId)));

            _traceRepositoryMock.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var result = await _traceService.CreateTraceAsync(createTraceDto);

            result.Id.Should().Be(0);
            result.Title.Should().Be(TestActiveTraceTitle);
            result.Description.ValueOrFailure().Should().Be(TestActiveTraceDescription);
            result.DueDate.Should().Be(_testActiveTraceDueDate);
            result.PropertyId.Should().Be(TestActivePropertyId);
            result.ReservationId.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldFailToCreateTraceWhenDueDateIsNotSetAsync()
        {
            var createTraceDto = new CreateTraceDto
            {
                Description = TestActiveTraceDescription.Some(),
                Title = TestActiveTraceTitle
            };

            var result = await Assert.ThrowsAsync<BusinessValidationException>(() => _traceService.CreateTraceAsync(createTraceDto));

            result.Message.Should().Be("The trace must have a title and a due date in the future to be created.");
        }

        [Fact]
        public async Task ShouldFailToCreateTraceWhenTitleIsEmptyAsync()
        {
            var createTraceDto = new CreateTraceDto
            {
                Description = TestActiveTraceDescription.Some(),
                DueDate = _testActiveTraceDueDate
            };

            var result = await Assert.ThrowsAsync<BusinessValidationException>(() => _traceService.CreateTraceAsync(createTraceDto));

            result.Message.Should().Be("The trace must have a title and a due date in the future to be created.");
        }

        [Fact]
        public async Task ShouldFailToCreateTraceWhenDueDateIsInThePastAsync()
        {
            var createTraceDto = new CreateTraceDto
            {
                Title = TestActiveTraceTitle,
                Description = TestActiveTraceDescription.Some(),
                DueDate = LocalDate.FromDateTime(DateTime.Today.AddDays(-1))
            };

            var result = await Assert.ThrowsAsync<BusinessValidationException>(() => _traceService.CreateTraceAsync(createTraceDto));

            result.Message.Should().Be("The trace must have a title and a due date in the future to be created.");
        }

        [Fact]
        public async Task ShouldBeAbleToReplaceTraceAsync()
        {
            var replaceTraceDto = new ReplaceTraceDto
            {
                Description = TestObsoleteTraceDescription.Some(),
                Title = TestObsoleteTraceTitle,
                DueDate = LocalDate.FromDateTime(DateTime.Today)
            };

            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.GetAsync(It.Is<int>(i => i == TestActiveTraceId)))
                .ReturnsAsync(new Trace
                {
                    Title = TestActiveTraceTitle,
                    DueDate = _testActiveTraceDueDate,
                    Description = TestActiveTraceDescription
                });

            _traceRepositoryMock.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var result = await _traceService.ReplaceTraceAsync(TestActiveTraceId, replaceTraceDto);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotReplaceTraceWhenTitleIsEmptyAsync()
        {
            var replaceTraceDto = new ReplaceTraceDto
            {
                Description = TestObsoleteTraceDescription.Some(),
                DueDate = _testObsoleteTraceDueDate
            };

            var result = await Assert.ThrowsAsync<BusinessValidationException>(() =>
                _traceService.ReplaceTraceAsync(TestCompletedTraceId, replaceTraceDto));

            result.Message.Should().Be($"Trace with id {TestCompletedTraceId} cannot be updated, the replacement must have a title and a due date in the future.");
        }

        [Fact]
        public async Task ShouldNotReplaceTraceWhenDueDateNotProvidedAsync()
        {
            var replaceTraceDto = new ReplaceTraceDto
            {
                Title = TestActiveTraceTitle,
                Description = TestObsoleteTraceDescription.Some()
            };

            var result = await Assert.ThrowsAsync<BusinessValidationException>(() =>
                _traceService.ReplaceTraceAsync(TestObsoleteTraceId, replaceTraceDto));

            result.Message.Should()
                .Be(
                    $"Trace with id {TestObsoleteTraceId} cannot be updated, the replacement must have a title and a due date in the future.");
        }

        [Fact]
        public async Task ShouldNotReplaceWhenDueDateIsInThePastAsync()
        {
            var replaceTraceDto = new ReplaceTraceDto
            {
                Title = TestActiveTraceTitle,
                Description = TestObsoleteTraceDescription.Some(),
                DueDate = LocalDate.FromDateTime(DateTime.Today.AddDays(-1))
            };

            var result = await Assert.ThrowsAsync<BusinessValidationException>(() =>
                _traceService.ReplaceTraceAsync(TestObsoleteTraceId, replaceTraceDto));

            result.Message.Should()
                .Be(
                    $"Trace with id {TestObsoleteTraceId} cannot be updated, the replacement must have a title and a due date in the future.");
        }

        [Fact]
        public async Task ShouldCompleteTraceAsync()
        {
            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(true);

            _requestContextMock.SetupGet(x => x.SubjectId)
                .Returns(TestSubjectId);

            _traceRepositoryMock.Setup(x => x.GetAsync(It.Is<int>(t => t == TestActiveTraceId)))
                .ReturnsAsync(new Trace
                {
                    Title = TestActiveTraceTitle,
                    Description = TestActiveTraceDescription,
                    Id = TestActiveTraceId,
                    State = TestActiveTraceState,
                    DueDate = _testActiveTraceDueDate
                });

            _traceRepositoryMock.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var result = await _traceService.CompleteTraceAsync(TestActiveTraceId);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotCompleteNonExistentTraceAsync()
        {
            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(false);

            var result =
                await Assert.ThrowsAsync<BusinessValidationException>(() =>
                    _traceService.CompleteTraceAsync(TestActiveTraceId));

            result.Message.Should().Be($"The trace with id {TestActiveTraceId} could not be found.");
        }

        [Fact]
        public async Task ShouldRevertCompletedTraceStateAsync()
        {
            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.GetAsync(It.Is<int>(t => t == TestCompletedTraceId)))
                .ReturnsAsync(new Trace
                {
                    Title = TestCompletedTraceTitle,
                    Description = TestCompletedTraceDescription,
                    Id = TestCompletedTraceId,
                    State = TestCompletedTraceState,
                    DueDate = _testCompletedTraceDueDate,
                    CompletedDate = _testCompletedDate
                });

            _traceRepositoryMock.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var result = await _traceService.RevertCompleteAsync(TestCompletedTraceId);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotBeAbleToRevertTraceCompletionWhenNotExistAsync()
        {
            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(false);

            var result = await Assert.ThrowsAsync<BusinessValidationException>(() => _traceService.RevertCompleteAsync(TestCompletedTraceId));
            result.Message.Should().Be($"The trace with id {TestCompletedTraceId} could not be found.");
        }

        [Fact]
        public async Task ShouldBeAbleToDeleteTraceAsync()
        {
            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.DeleteAsync(It.Is<int>(i => i == TestObsoleteTraceId)))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var result = await _traceService.DeleteTraceAsync(TestObsoleteTraceId);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotBeAbleToDeleteTraceWhenDoesNotExistAsync()
        {
            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(false);

            var result =
                await Assert.ThrowsAsync<BusinessValidationException>(() =>
                    _traceService.DeleteTraceAsync(TestObsoleteTraceId));

            result.Message.Should().Be($"The trace with id {TestObsoleteTraceId} could not be found.");
        }

        [Fact]
        public async Task ShouldNotBeAbleToDeleteTraceWhenDeleteFailsInRepositoryAsync()
        {
            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.DeleteAsync(It.Is<int>(i => i == TestActiveTraceId)))
                .ReturnsAsync(false);

            var result = await _traceService.DeleteTraceAsync(TestActiveTraceId);

            result.Should().BeFalse();
        }
    }
}