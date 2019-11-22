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
using Traces.Common.Enums;
using Traces.Common.Exceptions;
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
        private const int TestActiveTraceId = 1;
        private const string TestActiveTraceDescription = "TestActiveDescription";
        private const string TestActiveTraceTitle = "TestActiveTitle";
        private const TraceStateEnum TestActiveTraceState = TraceStateEnum.Active;
        private readonly LocalDate TestActiveTraceDueDate = DateTime.UtcNow.ToLocalDateTime().Date;

        private const int TestObsoleteTraceId = 2;
        private const string TestObsoleteTraceDescription = "TestObsoleteDescription";
        private const string TestObsoleteTraceTitle = "TestObsoleteTitle";
        private const TraceStateEnum TestObsoleteTraceState = TraceStateEnum.Obsolete;
        private readonly LocalDate TestObsoleteTraceDueDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)).ToLocalDateTime().Date;

        private const int TestCompletedTraceId = 3;
        private const string TestCompletedTraceDescription = "TestCompletedDescription";
        private const string TestCompletedTraceTitle = "TestCompletedTitle";
        private const TraceStateEnum TestCompletedTraceState = TraceStateEnum.Completed;
        private readonly LocalDate TestCompletedTraceDueDate = DateTime.UtcNow.Add(TimeSpan.FromHours(1)).ToLocalDateTime().Date;
        private readonly LocalDate TestCompletedDate = DateTime.UtcNow.ToLocalDateTime().Date;

        private readonly Mock<ITraceRepository> _traceRepositoryMock;
        private readonly ITraceService _traceService;

        public TraceServiceTest()
        {
            _traceRepositoryMock = MockRepository.Create<ITraceRepository>();
            _traceService = new TraceService(_traceRepositoryMock.Object);
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
                DueDateUtc = TestActiveTraceDueDate
            };

            var testObsoleteTrace = new Trace
            {
                Id = TestObsoleteTraceId,
                Description = TestObsoleteTraceDescription,
                State = TestObsoleteTraceState,
                Title = TestObsoleteTraceTitle,
                DueDateUtc = TestObsoleteTraceDueDate
            };

            var testCompletedTrace = new Trace
            {
                Id = TestCompletedTraceId,
                Description = TestCompletedTraceDescription,
                State = TestCompletedTraceState,
                Title = TestCompletedTraceTitle,
                DueDateUtc = TestCompletedTraceDueDate,
                CompletedUtc = TestCompletedDate
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
            result[0].DueDate.Should().Be(TestActiveTraceDueDate);
            result[0].CompletedDate.HasValue.Should().BeFalse();

            // Test second element is equivalent to trace testObsoleteTrace
            result[1].Id.Should().Be(TestObsoleteTraceId);
            result[1].Title.Should().Be(TestObsoleteTraceTitle);
            result[1].Description.ValueOrFailure().Should().Be(TestObsoleteTraceDescription);
            result[1].State.Should().Be(TestObsoleteTraceState);
            result[1].DueDate.Should().Be(TestObsoleteTraceDueDate);
            result[1].CompletedDate.HasValue.Should().BeFalse();

            // Test second element is equivalent to trace testCompletedTrace
            result[2].Id.Should().Be(TestCompletedTraceId);
            result[2].Title.Should().Be(TestCompletedTraceTitle);
            result[2].Description.ValueOrFailure().Should().Be(TestCompletedTraceDescription);
            result[2].State.Should().Be(TestCompletedTraceState);
            result[2].DueDate.Should().Be(TestCompletedTraceDueDate);
            result[2].CompletedDate.ValueOrFailure().Should().Be(TestCompletedDate);
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
                DueDateUtc = TestActiveTraceDueDate
            };

            var testObsoleteTrace = new Trace
            {
                Id = TestObsoleteTraceId,
                Description = TestObsoleteTraceDescription,
                State = TestActiveTraceState,
                Title = TestObsoleteTraceTitle,
                DueDateUtc = TestObsoleteTraceDueDate
            };

            var testCompletedTrace = new Trace
            {
                Id = TestCompletedTraceId,
                Description = TestCompletedTraceDescription,
                State = TestActiveTraceState,
                Title = TestCompletedTraceTitle,
                DueDateUtc = TestCompletedTraceDueDate,
                CompletedUtc = TestCompletedDate
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
            result[0].DueDate.Should().Be(TestActiveTraceDueDate);
            result[0].CompletedDate.HasValue.Should().BeFalse();

            // Test second element is equivalent to trace testObsoleteTrace
            result[1].Id.Should().Be(TestObsoleteTraceId);
            result[1].Title.Should().Be(TestObsoleteTraceTitle);
            result[1].Description.ValueOrFailure().Should().Be(TestObsoleteTraceDescription);
            result[1].State.Should().Be(TestActiveTraceState);
            result[1].DueDate.Should().Be(TestObsoleteTraceDueDate);
            result[1].CompletedDate.HasValue.Should().BeFalse();

            // Test second element is equivalent to trace testCompletedTrace
            result[2].Id.Should().Be(TestCompletedTraceId);
            result[2].Title.Should().Be(TestCompletedTraceTitle);
            result[2].Description.ValueOrFailure().Should().Be(TestCompletedTraceDescription);
            result[2].State.Should().Be(TestActiveTraceState);
            result[2].DueDate.Should().Be(TestCompletedTraceDueDate);
            result[2].CompletedDate.ValueOrFailure().Should().Be(TestCompletedDate);
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
        public async Task ShouldGetSingleTraceAsync()
        {
            var testActiveTrace = new Trace
            {
                Id = TestActiveTraceId,
                Description = TestActiveTraceDescription,
                State = TraceStateEnum.Active,
                Title = TestActiveTraceTitle,
                DueDateUtc = TestActiveTraceDueDate
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
            resultDto.DueDate.Should().Be(TestActiveTraceDueDate);
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
                DueDate = TestActiveTraceDueDate
            };

            _traceRepositoryMock.Setup(x => x.Insert(
                It.Is<Trace>(t =>
                    t.Description == TestActiveTraceDescription &&
                    t.Title == TestActiveTraceTitle &&
                    t.State == TraceStateEnum.Active &&
                    t.DueDateUtc == TestActiveTraceDueDate)));

                _traceRepositoryMock.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var result = await _traceService.CreateTraceAsync(createTraceDto);

            result.Should().Be(0);
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

            result.Message.Should().Be("The trace must have a title and a due date to be created.");
        }

        [Fact]
        public async Task ShouldFailToCreateTraceWhenTitleIsEmptyAsync()
        {
            var createTraceDto = new CreateTraceDto
            {
                Description = TestActiveTraceDescription.Some(),
                DueDate = TestActiveTraceDueDate
            };

            var result = await Assert.ThrowsAsync<BusinessValidationException>(() => _traceService.CreateTraceAsync(createTraceDto));

            result.Message.Should().Be("The trace must have a title and a due date to be created.");
        }

        [Fact]
        public async Task ShouldBeAbleToReplaceTraceAsync()
        {
            var replaceTraceDto = new ReplaceTraceDto
            {
                Description = TestObsoleteTraceDescription.Some(),
                Title = TestObsoleteTraceTitle,
                DueDate = TestObsoleteTraceDueDate
            };

            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.GetAsync(It.Is<int>(i => i == TestActiveTraceId)))
                .ReturnsAsync(new Trace
                {
                    Title = TestActiveTraceTitle,
                    DueDateUtc = TestActiveTraceDueDate,
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
                DueDate = TestObsoleteTraceDueDate
            };

            var result = await Assert.ThrowsAsync<BusinessValidationException>(() =>
                _traceService.ReplaceTraceAsync(TestCompletedTraceId, replaceTraceDto));

            result.Message.Should().Be($"Trace with id {TestCompletedTraceId} cannot be updated, the replacement must have a title and a due date.");
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
                    $"Trace with id {TestObsoleteTraceId} cannot be updated, the replacement must have a title and a due date.");
        }

        [Fact]
        public async Task ShouldCompleteTraceAsync()
        {
            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.GetAsync(It.Is<int>(t => t == TestActiveTraceId)))
                .ReturnsAsync(new Trace
                {
                    Title = TestActiveTraceTitle,
                    Description = TestActiveTraceDescription,
                    Id = TestActiveTraceId,
                    State = TestActiveTraceState,
                    DueDateUtc = TestActiveTraceDueDate
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
                    DueDateUtc = TestCompletedTraceDueDate,
                    CompletedUtc = TestCompletedDate
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