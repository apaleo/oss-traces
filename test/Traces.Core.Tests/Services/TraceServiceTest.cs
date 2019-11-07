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
        private const string TestTenantId = "TEST";
        private const TaskStateEnum TestActiveTraceState = TaskStateEnum.Active;
        private readonly Instant TestActiveTraceDueDate = DateTime.UtcNow.ToInstant();
        private readonly LocalTime TestActiveTraceDueTime = new LocalTime(13, 40, 00);

        private const int TestObsoleteTraceId = 2;
        private const string TestObsoleteTraceDescription = "TestObsoleteDescription";
        private const string TestObsoleteTraceTitle = "TestObsoleteTitle";
        private const TaskStateEnum TestObsoleteTraceState = TaskStateEnum.Obsolete;
        private readonly Instant TestObsoleteTraceDueDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)).ToInstant();

        private const int TestCompletedTraceId = 3;
        private const string TestCompletedTraceDescription = "TestCompletedDescription";
        private const string TestCompletedTraceTitle = "TestCompletedTitle";
        private const string TestCompletedBy = "TestCompletedBy";
        private const TaskStateEnum TestCompletedTraceState = TaskStateEnum.Completed;
        private readonly Instant TestCompletedTraceDueDate = DateTime.UtcNow.Add(TimeSpan.FromHours(1)).ToInstant();
        private readonly Instant TestCompletedDate = DateTime.UtcNow.ToInstant();

        private readonly Mock<ITraceRepository> _traceRepositoryMock;
        private readonly Mock<IRequestContext> _requestContextMock;

        private readonly ITraceService _traceService;

        public TraceServiceTest()
        {
            _traceRepositoryMock = MockRepository.Create<ITraceRepository>();
            _requestContextMock = MockRepository.Create<IRequestContext>();

            _traceService = new TraceService(_traceRepositoryMock.Object, _requestContextMock.Object);
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
                DueDateUtc = TestActiveTraceDueDate,
                TenantId = TestTenantId,
                DueTime = TestActiveTraceDueTime
            };

            var testObsoleteTrace = new Trace
            {
                Id = TestObsoleteTraceId,
                Description = TestObsoleteTraceDescription,
                State = TestObsoleteTraceState,
                Title = TestObsoleteTraceTitle,
                DueDateUtc = TestObsoleteTraceDueDate,
                TenantId = TestTenantId
            };

            var testCompletedTrace = new Trace
            {
                Id = TestCompletedTraceId,
                Description = TestCompletedTraceDescription,
                State = TestCompletedTraceState,
                Title = TestCompletedTraceTitle,
                DueDateUtc = TestCompletedTraceDueDate,
                TenantId = TestTenantId,
                CompletedBy = TestCompletedBy,
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
            result[0].DueDate.Should().Be(TestActiveTraceDueDate.InUtc());
            result[0].DueTime.ValueOrFailure().Should().Be(TestActiveTraceDueTime);
            result[0].CompletedBy.HasValue.Should().BeFalse();
            result[0].CompletedDate.HasValue.Should().BeFalse();

            // Test second element is equivalent to trace testObsoleteTrace
            result[1].Id.Should().Be(TestObsoleteTraceId);
            result[1].Title.Should().Be(TestObsoleteTraceTitle);
            result[1].Description.ValueOrFailure().Should().Be(TestObsoleteTraceDescription);
            result[1].State.Should().Be(TestObsoleteTraceState);
            result[1].DueDate.Should().Be(TestObsoleteTraceDueDate.InUtc());
            result[1].DueTime.HasValue.Should().BeFalse();
            result[1].CompletedBy.HasValue.Should().BeFalse();
            result[1].CompletedDate.HasValue.Should().BeFalse();

            // Test second element is equivalent to trace testCompletedTrace
            result[2].Id.Should().Be(TestCompletedTraceId);
            result[2].Title.Should().Be(TestCompletedTraceTitle);
            result[2].Description.ValueOrFailure().Should().Be(TestCompletedTraceDescription);
            result[2].State.Should().Be(TestCompletedTraceState);
            result[2].DueDate.Should().Be(TestCompletedTraceDueDate.InUtc());
            result[2].DueTime.HasValue.Should().BeFalse();
            result[2].CompletedBy.ValueOrFailure().Should().Be(TestCompletedBy);
            result[2].CompletedDate.ValueOrFailure().Should().Be(TestCompletedDate.InUtc());
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
                State = TaskStateEnum.Active,
                Title = TestActiveTraceTitle,
                DueDateUtc = TestActiveTraceDueDate,
                TenantId = TestTenantId,
                DueTime = TestActiveTraceDueTime
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
            resultDto.DueDate.Should().Be(TestActiveTraceDueDate.InUtc());
            resultDto.DueTime.ValueOrFailure().Should().Be(TestActiveTraceDueTime);
            resultDto.CompletedBy.HasValue.Should().BeFalse();
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
                DueDate = TestActiveTraceDueDate.InUtc(),
                DueTime = TestActiveTraceDueTime.Some()
            };

            _traceRepositoryMock.Setup(x => x.Insert(
                It.Is<Trace>(t =>
                t.Description == TestActiveTraceDescription &&
                t.Title == TestActiveTraceTitle &&
                t.State == TaskStateEnum.Active &&
                t.DueDateUtc == TestActiveTraceDueDate &&
                t.DueTime == TestActiveTraceDueTime)));

            _traceRepositoryMock.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var result = await _traceService.CreateTraceAsync(createTraceDto);

            var resultValue = result.ValueOrFailure();

            resultValue.Title.Should().Be(TestActiveTraceTitle);
            resultValue.Description.ValueOrFailure().Should().Be(TestActiveTraceDescription);
            resultValue.State.Should().Be(TaskStateEnum.Active);
            resultValue.DueDate.Should().Be(TestActiveTraceDueDate.InUtc());
            resultValue.DueTime.ValueOrFailure().Should().Be(TestActiveTraceDueTime);
        }

        [Fact]
        public async Task ShouldFailToCreateTraceWhenDueDateIsNotSetAsync()
        {
            var createTraceDto = new CreateTraceDto
            {
                Description = TestActiveTraceDescription.Some(),
                Title = TestActiveTraceTitle,
                DueTime = TestActiveTraceDueTime.Some()
            };

            var result = await _traceService.CreateTraceAsync(createTraceDto);

            result.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldFailToCreateTraceWhenTitleIsEmptyAsync()
        {
            var createTraceDto = new CreateTraceDto
            {
                Description = TestActiveTraceDescription.Some(),
                DueDate = TestActiveTraceDueDate.InUtc(),
                DueTime = TestActiveTraceDueTime.Some()
            };

            var result = await _traceService.CreateTraceAsync(createTraceDto);

            result.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldBeAbleToReplaceTraceAsync()
        {
            var replaceTraceDto = new ReplaceTraceDto
            {
                Description = TestObsoleteTraceDescription.Some(),
                Title = TestObsoleteTraceTitle,
                DueDate = TestObsoleteTraceDueDate.InUtc()
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
                DueDate = TestObsoleteTraceDueDate.InUtc()
            };

            var result = await _traceService.ReplaceTraceAsync(TestCompletedTraceId, replaceTraceDto);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldNotReplaceTraceWhenDueDateNotProvidedAsync()
        {
            var replaceTraceDto = new ReplaceTraceDto
            {
                Title = TestActiveTraceTitle,
                Description = TestObsoleteTraceDescription.Some()
            };

            var result = await _traceService.ReplaceTraceAsync(TestObsoleteTraceId, replaceTraceDto);

            result.Should().BeFalse();
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
                    DueDateUtc = TestActiveTraceDueDate,
                    DueTime = TestActiveTraceDueTime
                });

            _traceRepositoryMock.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _requestContextMock.SetupGet(x => x.TenantId)
                .Returns(TestTenantId);

            var result = await _traceService.CompleteTraceAsync(TestActiveTraceId);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotCompleteNonExistentTraceAsync()
        {
            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(false);

            var result = await _traceService.CompleteTraceAsync(TestActiveTraceId);

            result.Should().BeFalse();
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
                    CompletedBy = TestCompletedBy,
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

            var result = await _traceService.RevertCompleteAsync(TestCompletedTraceId);
            result.Should().BeFalse();
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

            var result = await _traceService.DeleteTraceAsync(TestObsoleteTraceId);

            result.Should().BeFalse();
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