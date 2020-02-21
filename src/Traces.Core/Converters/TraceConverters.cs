using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Optional;
using Traces.Core.Models;
using Traces.Data.Entities;

namespace Traces.Core.Converters
{
    public static class TraceConverters
    {
        public static TraceDto ConvertToDto(this Trace trace) => new TraceDto
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
            Files = trace.Files.ConvertToDto().SomeNotNull()
        };

        public static IReadOnlyList<TraceDto> ConvertToDto(this IReadOnlyList<Trace> traces) => traces.Select(ConvertToDto).ToList();
    }
}
