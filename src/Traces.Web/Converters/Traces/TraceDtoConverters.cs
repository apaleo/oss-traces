using System.Collections.Generic;
using Traces.Common.Utils;
using Traces.Core.Models;
using Traces.Core.Models.Files;
using Traces.Web.Converters.Files;
using Traces.Web.Models;

namespace Traces.Web.Converters.Traces
{
    public static class ModelExtensionMethods
    {
        public static TraceItemModel ConvertToItemModel(this TraceDto dto)
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
                AssignedRole = dto.AssignedRole.ValueOr(string.Empty),
                Files = dto.Files.ValueOr(new List<TraceFileDto>()).ConvertToItemModel()
            };
        }
    }
}
