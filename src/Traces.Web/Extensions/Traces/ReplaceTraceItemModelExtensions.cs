using NodaTime.Extensions;
using Optional;
using Traces.Common.Utils;
using Traces.Core.Models;
using Traces.Web.Extensions.Files;
using Traces.Web.Models;

namespace Traces.Web.Extensions.Traces
{
    public static class ReplaceTraceItemModelExtensions
    {
        public static ReplaceTraceDto ToReplaceTraceDto(this ReplaceTraceItemModel itemModel)
        {
            Check.NotNull(itemModel, nameof(itemModel));

            return new ReplaceTraceDto
            {
                Title = itemModel.Title,
                Description = itemModel.Description.SomeWhen(t => !string.IsNullOrWhiteSpace(t)),
                DueDate = itemModel.DueDate.ToLocalDateTime().Date,
                AssignedRole = itemModel.AssignedRole.SomeNotNull(),
                FilesToDelete = itemModel.FilesToDelete.SomeNotNull(),
                FilesToUpload = itemModel.FilesToUpload.ToCreateTraceFileDtoList().SomeNotNull(),
                FileContainsNoPii = itemModel.FileContainsNoPii
            };
        }
    }
}
