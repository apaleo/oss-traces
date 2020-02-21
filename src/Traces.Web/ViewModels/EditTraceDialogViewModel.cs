using System;
using System.Collections.Generic;
using System.Linq;
using Traces.Common.Constants;
using Traces.Web.Models;
using Traces.Web.Models.Files;
using Traces.Web.Utils.Converters.Files;

namespace Traces.Web.ViewModels
{
    public class EditTraceDialogViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public string SelectedRole { get; set; }

        public List<FileToUploadModel> FilesToUpload { get; set; } = new List<FileToUploadModel>();

        public List<TraceFileItemModel> TraceFiles { get; set; } = new List<TraceFileItemModel>();

        public List<string> Roles { get; } = new List<string>();

        public CreateTraceItemModel GetCreateTraceItemModel()
            => new CreateTraceItemModel
            {
                Title = Title,
                Description = Description,
                DueDate = DueDate ?? DateTime.MinValue,
                AssignedRole =
                    string.IsNullOrWhiteSpace(SelectedRole) ||
                    SelectedRole == TextConstants.TracesEditDialogNoRoleAssignedText
                        ? null
                        : SelectedRole
            };

        public CreateTraceFileItemModel[] GetCreateTraceFileItemModelArray(int traceId)
            => FilesToUpload
                .Where(file => file.IsValid)
                .ToArray()
                .ConvertToCreateTraceFileItemModelArray(traceId);

        public ReplaceTraceItemModel GetReplaceTraceItemModel()
            => new ReplaceTraceItemModel
            {
                Id = Id,
                Title = Title,
                Description = Description,
                DueDate = DueDate ?? DateTime.MinValue,
                AssignedRole =
                    string.IsNullOrWhiteSpace(SelectedRole) ||
                    SelectedRole == TextConstants.TracesEditDialogNoRoleAssignedText
                        ? null
                        : SelectedRole
            };

        public void ClearCurrentState()
        {
            Id = 0;
            Title = string.Empty;
            Description = string.Empty;
            DueDate = null;
            SelectedRole = string.Empty;

            FilesToUpload = new List<FileToUploadModel>();
            TraceFiles = new List<TraceFileItemModel>();
        }
    }
}
