using Traces.Common.Utils;
using Traces.Core.Models;
using Traces.Web.Models;

namespace Traces.Web.Utils
{
    public static class ExtensionMethods
    {
        public static TraceItemModel ConvertToTraceItemModel(this TraceDto dto)
        {
            Check.NotNull(dto, nameof(dto));

            return new TraceItemModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description.ValueOr(string.Empty),
                State = dto.State,
                DueDate = dto.DueDate.ToDateTimeUnspecified(),
                PropertyId = dto.PropertyId,
                ReservationId = dto.ReservationId.ValueOr(string.Empty),
                AssignedRole = dto.AssignedRole.ValueOr(string.Empty)
            };
        }
    }
}