using System;
using Traces.Common.Constants;

namespace Traces.Web.Models
{
    public class EditTraceDialogViewModel
    {
        public string ModalTitle =>
            IsReplace ? TextConstants.ReplaceTraceModalTitle : TextConstants.CreateTraceModalTitle;

        public string CreateOrEditButtonText =>
            IsReplace ? TextConstants.ReplaceTraceButtonText : TextConstants.CreateTraceButtonText;

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsReplace { get; set; }

        public CreateTraceItemModel GetCreateTraceItemModel()
            => new CreateTraceItemModel
            {
                Title = Title,
                Description = Description,
                DueDate = DueDate.HasValue ? DueDate.Value : DateTime.MinValue
            };

        public ReplaceTraceItemModel GetReplaceTraceItemModel()
            => new ReplaceTraceItemModel
            {
                Id = Id,
                Title = Title,
                Description = Description,
                DueDate = DueDate.HasValue ? DueDate.Value : DateTime.MinValue
            };

        public void ClearCurrentState()
        {
            Id = 0;
            Title = string.Empty;
            Description = string.Empty;
            DueDate = null;
        }
    }
}