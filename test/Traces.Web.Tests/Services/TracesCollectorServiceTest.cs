using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NodaTime;
using NodaTime.Extensions;
using Optional;
using Optional.Unsafe;
using Traces.Common.Enums;
using Traces.Core.Models;
using Traces.Core.Services;
using Traces.Testing;
using Traces.Web.Services;
using Xunit;

namespace Traces.Web.Tests.Services
{
    public class TracesCollectorServiceTest : BaseTest
    {
        private const int FirstTraceId = 1;
        private const string FirstTraceTitle = "FirstTraceTitle";
        private const string FirstTraceDescription = "FirstTraceDescription";
        private const TraceStateEnum FirstTraceState = TraceStateEnum.Active;
        private readonly LocalDate FirstTraceDueDate = DateTime.UtcNow.ToLocalDateTime().Date;

        private const int SecondTraceId = 2;
        private const string SecondTraceTitle = "SecondTraceTitle";
        private const TraceStateEnum SecondTraceState = TraceStateEnum.Active;
        private readonly LocalDate SecondTraceDueDate = DateTime.UtcNow.ToLocalDateTime().Date;

        private const int ThirdTraceId = 3;
        private const string ThirdTraceTitle = "ThirdTraceTitle";
        private const string ThirdTraceDescription = "ThirdTraceDescription";
        private const TraceStateEnum ThirdTraceState = TraceStateEnum.Obsolete;
        private readonly LocalDate ThirdTraceDueDate = DateTime.UtcNow.ToLocalDateTime().Date;

        private readonly Mock<ITraceService> _traceServiceMock;
        private readonly ITracesCollectorService _tracesCollectorService;

        public TracesCollectorServiceTest()
        {
            _traceServiceMock = MockRepository.Create<ITraceService>();
            _tracesCollectorService = new TracesCollectorService(_traceServiceMock.Object);
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
                    DueDate = FirstTraceDueDate
                },
                new TraceDto
                {
                    Id = SecondTraceId,
                    Title = SecondTraceTitle,
                    State = SecondTraceState,
                    DueDate = SecondTraceDueDate
                },
                new TraceDto
                {
                    Id = ThirdTraceId,
                    Title = ThirdTraceTitle,
                    Description = ThirdTraceDescription.Some(),
                    State = ThirdTraceState,
                    DueDate = ThirdTraceDueDate
                }
            };

            _traceServiceMock.Setup(x => x.GetTracesAsync())
                .ReturnsAsync(testTraces);

            var result = await _tracesCollectorService.GetTracesAsync();

            result.Should().NotBeNull();

            result.Success.Should().BeTrue();

            var resultTraces = result.Result.ValueOrFailure();

            resultTraces.Should().HaveCount(3);

            resultTraces[0].Id.Should().Be(FirstTraceId);
            resultTraces[0].Title.Should().Be(FirstTraceTitle);
            resultTraces[0].Description.Should().Be(FirstTraceDescription);
            resultTraces[0].DueDate.Should().Be(FirstTraceDueDate.ToDateTimeUnspecified());
            resultTraces[0].State.Should().Be(FirstTraceState);

            resultTraces[1].Id.Should().Be(SecondTraceId);
            resultTraces[1].Title.Should().Be(SecondTraceTitle);
            resultTraces[1].Description.Should().BeEmpty();
            resultTraces[1].DueDate.Should().Be(SecondTraceDueDate.ToDateTimeUnspecified());
            resultTraces[1].State.Should().Be(SecondTraceState);

            resultTraces[2].Id.Should().Be(ThirdTraceId);
            resultTraces[2].Title.Should().Be(ThirdTraceTitle);
            resultTraces[2].Description.Should().Be(ThirdTraceDescription);
            resultTraces[2].DueDate.Should().Be(ThirdTraceDueDate.ToDateTimeUnspecified());
            resultTraces[2].State.Should().Be(ThirdTraceState);
        }
    }
}