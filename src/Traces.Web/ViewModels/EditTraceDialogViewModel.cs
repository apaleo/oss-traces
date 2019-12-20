using System;
using System.Collections.Generic;
using Traces.Common.Constants;

namespace Traces.Web.Models
{
    public class EditTraceDialogViewModel
    {
        public EditTraceDialogViewModel()
        {
            Roles = new List<string>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public string SelectedRole { get; set; }

        public List<string> Roles { get; }

        public CreateTraceItemModel GetCreateTraceItemModel()
            => new CreateTraceItemModel
            {
                Title = Title,
                Description = Description,
                DueDate = DueDate ?? DateTime.MinValue,
                AssignedRole = SelectedRole == TextConstants.TracesEditDialogNoRoleAssignedText ? null : SelectedRole
            };

        public ReplaceTraceItemModel GetReplaceTraceItemModel()
            => new ReplaceTraceItemModel
            {
                Id = Id,
                Title = Title,
                Description = Description,
                DueDate = DueDate ?? DateTime.MinValue,
                AssignedRole = SelectedRole == TextConstants.TracesEditDialogNoRoleAssignedText ? null : SelectedRole
            };

        public void ClearCurrentState()
        {
            Id = 0;
            Title = string.Empty;
            Description = string.Empty;
            DueDate = null;
            SelectedRole = string.Empty;
        }
    }
}