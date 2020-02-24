using System;
using System.Collections.Generic;
using System.Linq;
using Traces.Common.Constants;
using Traces.Web.Converters.Files;
using Traces.Web.Enums;
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

        public IReadOnlyList<FileToUploadModel> FilesToUpload { get; set; } = new List<FileToUploadModel>();

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
                        : SelectedRole
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
                        : SelectedRole
            };

        public IReadOnlyList<CreateTraceFileItemModel> GetCreateTraceFileItemModels(int traceId)
            => FilesToUpload
                .Where(file => file.IsValid)
                .ToList()
                .ConvertToCreateTraceFileItemModel(traceId);

        public IReadOnlyList<int> GetTraceFilesToDelete()
            => TraceFiles
                .Where(file => file.State == TraceFileItemModelState.ShouldDelete)
                .Select(file => file.Id)
                .ToList();

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
