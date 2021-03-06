using System;
using System.Collections.Generic;
using System.Linq;
using Traces.Common.Constants;
using Traces.Web.Enums;
using Traces.Web.Extensions.Files;
using Traces.Web.Models;
using Traces.Web.Models.Files;

namespace Traces.Web.ViewModels
{
    public class EditTraceDialogViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public string SelectedRole { get; set; }

        public List<FileToUploadModel> FilesToUpload { get; } = new List<FileToUploadModel>();

        public bool ConfirmFileContainsNoPii { get; set; }

        public IReadOnlyList<TraceFileItemModel> TraceFiles { get; set; } = new List<TraceFileItemModel>();

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
                        : SelectedRole,
                FilesToUpload = FilesToUpload.GetValidCreateTraceFileItemModels(),
                FileContainsNoPii = ConfirmFileContainsNoPii
            };

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
                        : SelectedRole,
                FilesToDelete = GetTraceFilesToDelete(),
                FilesToUpload = FilesToUpload.GetValidCreateTraceFileItemModels(),
                FileContainsNoPii = ConfirmFileContainsNoPii
            };

        public void ClearCurrentState()
        {
            Id = 0;
            Title = string.Empty;
            Description = string.Empty;
            DueDate = null;
            SelectedRole = string.Empty;

            FilesToUpload.Clear();
            TraceFiles = new List<TraceFileItemModel>();
        }

        private List<int> GetTraceFilesToDelete()
            => TraceFiles
                .Where(file => file.State == TraceFileItemModelState.ShouldDelete)
                .Select(file => file.Id)
                .ToList();
    }
}
