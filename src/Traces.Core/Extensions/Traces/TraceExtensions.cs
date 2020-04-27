using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Optional;
using Traces.Core.Extensions.Files;
using Traces.Core.Models;
using Traces.Core.Models.Files;
using Traces.Data.Entities;

namespace Traces.Core.Extensions.Traces
{
    public static class TraceExtensions
    {
        public static TraceDto ToTraceDto(this Trace trace) => new TraceDto
        {
            Description = trace.Description.SomeNotNull(),
            State = trace.State,
            Title = trace.Title,
            CompletedDate = trace.CompletedDate?.Some() ?? Option.None<LocalDate>(),
            DueDate = trace.DueDate,
            Id = trace.Id,
            PropertyId = trace.PropertyId,
            ReservationId = trace.ReservationId.SomeNotNull(),
            AssignedRole = trace.AssignedRole.SomeNotNull(),
            Files = trace.Files?.ToTraceFileDtoList().SomeNotNull() ?? Option.None<IReadOnlyList<TraceFileDto>>()
        };

        public static IReadOnlyList<TraceDto> ToTraceDtoList(this IReadOnlyList<Trace> traces) => traces.Select(ToTraceDto).ToList();
    }
}
