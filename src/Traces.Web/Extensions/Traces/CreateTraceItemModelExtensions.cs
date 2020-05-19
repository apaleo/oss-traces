using System.Collections.Generic;
using NodaTime.Extensions;
using Optional;
using Traces.Common.Utils;
using Traces.Core.Models;
using Traces.Core.Models.Files;
using Traces.Web.Extensions.Files;
using Traces.Web.Models;

namespace Traces.Web.Extensions.Traces
{
    public static class CreateTraceItemModelExtensions
    {
        public static CreateTraceDto ToCreateTraceDto(this CreateTraceItemModel itemModel)
        {
            Check.NotNull(itemModel, nameof(itemModel));

            return new CreateTraceDto
            {
                Title = itemModel.Title,
                Description = itemModel.Description.SomeNotNull(),
                DueDate = itemModel.DueDate.ToLocalDateTime().Date,
                PropertyId = itemModel.PropertyId,
                ReservationId = itemModel.ReservationId.SomeNotNull(),
                AssignedRole = itemModel.AssignedRole.SomeNotNull(),
                FilesToUpload = itemModel.FilesToUpload.ToCreateTraceFileDtoList().SomeNotNull(),
            };
        }

        public static CreateTraceDto ToCreateTraceDtoWithoutPropertyId(this CreateTraceItemModel itemModel)
        {
            Check.NotNull(itemModel, nameof(itemModel));

            return new CreateTraceDto
            {
                Title = itemModel.Title,
                Description = itemModel.Description.SomeNotNull(),
                DueDate = itemModel.DueDate.ToLocalDateTime().Date,
                ReservationId = itemModel.ReservationId.SomeNotNull(),
                AssignedRole = itemModel.AssignedRole.SomeNotNull(),
                FilesToUpload = itemModel.FilesToUpload?.ToCreateTraceFileDtoList().SomeNotNull() ?? Option.None<List<CreateTraceFileDto>>(),
            };
        }
    }
}
