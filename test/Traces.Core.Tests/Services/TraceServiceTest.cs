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
using Traces.Core.Models.Files;
using Traces.Core.Repositories;
using Traces.Core.Services.Files;
using Traces.Core.Services.Traces;
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
        private const TraceState TestActiveTraceState = TraceState.Active;
        private const string TestActivePropertyId = "PROA";

        private const int TestOverdueTraceId = 2;
        private const string TestOverdueTraceDescription = "TestObsoleteDescription";
        private const string TestOverdueTraceTitle = "TestObsoleteTitle";
        private const TraceState TestOverdueTraceState = TraceState.Active;
        private const string TestOverduePropertyId = "PROO";
        private const string TestOverdueReservationId = "RESO";

        private const int TestCompletedTraceId = 3;
        private const string TestCompletedTraceDescription = "TestCompletedDescription";
        private const string TestCompletedTraceTitle = "TestCompletedTitle";
        private const TraceState TestCompletedTraceState = TraceState.Completed;
        private const string TestCompletedPropertyId = "PROC";
        private const string TestCompletedReservationId = "RESC";

        private const int TestTraceFileId = 4;
        private const string TestTraceFileName = "TestTraceFileName";

        private readonly LocalDate _testActiveTraceDueDate = DateTime.UtcNow.ToLocalDateTime().Date;
        private readonly LocalDate _testOverdueTraceDueDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)).ToLocalDateTime().Date;
        private readonly LocalDate _testCompletedTraceDueDate = DateTime.UtcNow.Add(TimeSpan.FromHours(1)).ToLocalDateTime().Date;
        private readonly LocalDate _testCompletedDate = DateTime.UtcNow.ToLocalDateTime().Date;
        private readonly DateTime _testFromDate = DateTime.Today;
        private readonly DateTime _testToDate = DateTime.Today.AddDays(1);

        private readonly Mock<ITraceRepository> _traceRepositoryMock;
        private readonly Mock<IRequestContext> _requestContextMock;
        private readonly Mock<IApaleoClientsFactory> _apaleoClientFactoryMock;
        private readonly Mock<ITraceFileService> _traceFileServiceMock;
        private readonly ITraceService _traceService;

        public TraceServiceTest()
        {
            _traceRepositoryMock = MockRepository.Create<ITraceRepository>();
            _requestContextMock = MockRepository.Create<IRequestContext>();
            _apaleoClientFactoryMock = MockRepository.Create<IApaleoClientsFactory>();
            _traceFileServiceMock = MockRepository.Create<ITraceFileService>();

            _traceService = new TraceService(_traceRepositoryMock.Object, _requestContextMock.Object, _apaleoClientFactoryMock.Object, _traceFileServiceMock.Object);
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
                Id = TestOverdueTraceId,
                Description = TestOverdueTraceDescription,
                State = TestOverdueTraceState,
                Title = TestOverdueTraceTitle,
                DueDate = _testOverdueTraceDueDate,
                PropertyId = TestOverduePropertyId,
                ReservationId = TestOverdueReservationId
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
            result[1].Id.Should().Be(TestOverdueTraceId);
            result[1].Title.Should().Be(TestOverdueTraceTitle);
            result[1].Description.ValueOrFailure().Should().Be(TestOverdueTraceDescription);
            result[1].State.Should().Be(TestOverdueTraceState);
            result[1].DueDate.Should().Be(_testOverdueTraceDueDate);
            result[1].CompletedDate.HasValue.Should().BeFalse();
            result[1].PropertyId.Should().Be(TestOverduePropertyId);
            result[1].ReservationId.ValueOrFailure().Should().Be(TestOverdueReservationId);

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
                Id = TestOverdueTraceId,
                Description = TestOverdueTraceDescription,
                State = TestActiveTraceState,
                Title = TestOverdueTraceTitle,
                DueDate = _testOverdueTraceDueDate,
                PropertyId = TestOverduePropertyId,
                ReservationId = TestOverdueReservationId
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
            result[1].Id.Should().Be(TestOverdueTraceId);
            result[1].Title.Should().Be(TestOverdueTraceTitle);
            result[1].Description.ValueOrFailure().Should().Be(TestOverdueTraceDescription);
            result[1].State.Should().Be(TestActiveTraceState);
            result[1].DueDate.Should().Be(_testOverdueTraceDueDate);
            result[1].CompletedDate.HasValue.Should().BeFalse();
            result[1].PropertyId.Should().Be(TestOverduePropertyId);
            result[1].ReservationId.ValueOrFailure().Should().Be(TestOverdueReservationId);

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

            var result = await _traceService.GetAllTracesForReservationAsync(TestCompletedReservationId);

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
        public async Task ShouldGetSingleTraceAsync()
        {
            var testActiveTrace = new Trace
            {
                Id = TestActiveTraceId,
                Description = TestActiveTraceDescription,
                State = TraceState.Active,
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

            var result = await _traceService.GetTraceAsync(TestOverdueTraceId);

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
                    t.State == TraceState.Active &&
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
        public async Task ShouldCreateTraceFileAsync()
        {
            var createTraceFileDto = new CreateTraceFileDto { Name = TestTraceFileName };
            var traceFile = new TraceFile
            {
                Id = TestTraceFileId,
                Name = TestTraceFileName
            };
            var traceFiles = new List<TraceFile>();
            traceFiles.Add(traceFile);

            var createTraceDto = new CreateTraceDto
            {
                Description = TestActiveTraceDescription.Some(),
                Title = TestActiveTraceTitle,
                DueDate = _testActiveTraceDueDate,
                PropertyId = TestActivePropertyId,
                FilesToUpload = Option.Some(new List<CreateTraceFileDto> { createTraceFileDto })
            };

            _traceRepositoryMock.Setup(x => x.Insert(
                It.Is<Trace>(t => t.Files.Exists(tf => tf.Id == TestTraceFileId))));

            _traceFileServiceMock.Setup(
                    x => x.UploadStorageFilesAsync(
                        It.Is<List<CreateTraceFileDto>>(
                            l =>
                                l.Find(tf => tf.Name == TestTraceFileName) != null)))
                .ReturnsAsync(traceFiles);

            _traceRepositoryMock.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var result = await _traceService.CreateTraceAsync(createTraceDto);

            var resultFilesList = result.Files.ValueOr(new List<TraceFileDto>());
            resultFilesList.Count.Should().Be(1);
            resultFilesList[0].Id.Should().Be(TestTraceFileId);
            resultFilesList[0].Name.Should().Be(TestTraceFileName);
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
                Description = TestOverdueTraceDescription.Some(),
                Title = TestOverdueTraceTitle,
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
        public async Task ShouldBeAbleToReplaceTraceFileAsync()
        {
            var replaceTraceDto = new ReplaceTraceDto
            {
                Title = TestOverdueTraceTitle,
                DueDate = LocalDate.FromDateTime(DateTime.Today),
                FilesToUpload = Option.Some(new List<CreateTraceFileDto> { new CreateTraceFileDto { Name = TestTraceFileName } }),
                FilesToDelete = Option.Some(new List<int> { TestTraceFileId })
            };

            _traceFileServiceMock.Setup(x => x.UploadStorageFilesAsync(It.Is<List<CreateTraceFileDto>>(l => l.Exists(tf => tf.Name == TestTraceFileName))))
                .ReturnsAsync(new List<TraceFile> { new TraceFile { Name = TestTraceFileName } });

            _traceFileServiceMock.Setup(x => x.DeleteStorageFilesAsync(It.Is<List<int>>(l => l.Exists(tf => tf == TestTraceFileId))))
                .ReturnsAsync(new List<TraceFile> { new TraceFile { Name = TestTraceFileName } });

            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.GetAsync(It.Is<int>(i => i == TestActiveTraceId)))
                .ReturnsAsync(new Trace
                {
                    Title = TestActiveTraceTitle,
                    DueDate = _testActiveTraceDueDate,
                    Files = new List<TraceFile> { new TraceFile() }
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
                Description = TestOverdueTraceDescription.Some(),
                DueDate = _testOverdueTraceDueDate
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
                Description = TestOverdueTraceDescription.Some()
            };

            var result = await Assert.ThrowsAsync<BusinessValidationException>(() =>
                _traceService.ReplaceTraceAsync(TestOverdueTraceId, replaceTraceDto));

            result.Message.Should()
                .Be(
                    $"Trace with id {TestOverdueTraceId} cannot be updated, the replacement must have a title and a due date in the future.");
        }

        [Fact]
        public async Task ShouldNotReplaceWhenDueDateIsInThePastAsync()
        {
            var replaceTraceDto = new ReplaceTraceDto
            {
                Title = TestActiveTraceTitle,
                Description = TestOverdueTraceDescription.Some(),
                DueDate = LocalDate.FromDateTime(DateTime.Today.AddDays(-1))
            };

            var result = await Assert.ThrowsAsync<BusinessValidationException>(() =>
                _traceService.ReplaceTraceAsync(TestOverdueTraceId, replaceTraceDto));

            result.Message.Should()
                .Be(
                    $"Trace with id {TestOverdueTraceId} cannot be updated, the replacement must have a title and a due date in the future.");
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

            _traceRepositoryMock.Setup(x => x.GetAsync(It.Is<int>(i => i == TestOverdueTraceId)))
                .ReturnsAsync(new Trace());

            _traceRepositoryMock.Setup(x => x.DeleteAsync(It.Is<int>(i => i == TestOverdueTraceId)))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var result = await _traceService.DeleteTraceAsync(TestOverdueTraceId);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldBeAbleToDeleteTraceFileAsync()
        {
            _traceFileServiceMock.Setup(x => x.DeleteStorageFilesAsync(It.Is<List<int>>(l => l.Exists(tf => tf == TestTraceFileId))))
                .ReturnsAsync(new List<TraceFile> { new TraceFile { Name = TestTraceFileName } });

            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.GetAsync(It.Is<int>(i => i == TestOverdueTraceId)))
                .ReturnsAsync(new Trace { Files = new List<TraceFile> { new TraceFile { Id = TestTraceFileId } } });

            _traceRepositoryMock.Setup(x => x.DeleteAsync(It.Is<int>(i => i == TestOverdueTraceId)))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var result = await _traceService.DeleteTraceAsync(TestOverdueTraceId);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotBeAbleToDeleteTraceWhenDoesNotExistAsync()
        {
            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(false);

            var result =
                await Assert.ThrowsAsync<BusinessValidationException>(() =>
                    _traceService.DeleteTraceAsync(TestOverdueTraceId));

            result.Message.Should().Be($"The trace with id {TestOverdueTraceId} could not be found.");
        }

        [Fact]
        public async Task ShouldNotBeAbleToDeleteTraceWhenDeleteFailsInRepositoryAsync()
        {
            _traceRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Trace, bool>>>()))
                .ReturnsAsync(true);

            _traceRepositoryMock.Setup(x => x.GetAsync(It.Is<int>(i => i == TestActiveTraceId)))
                .ReturnsAsync(new Trace());

            _traceRepositoryMock.Setup(x => x.DeleteAsync(It.Is<int>(i => i == TestActiveTraceId)))
                .ReturnsAsync(false);

            var result = await _traceService.DeleteTraceAsync(TestActiveTraceId);

            result.Should().BeFalse();
        }
    }
}