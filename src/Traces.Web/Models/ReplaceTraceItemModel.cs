#pragma warning disable CA2227 // Can be disabled in models
using System;
using System.Collections.Generic;
using Traces.Web.Models.Files;

namespace Traces.Web.Models
{
    public class ReplaceTraceItemModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public string AssignedRole { get; set; }

        public List<CreateTraceFileItemModel> FilesToUpload { get; set; }

        public List<int> FilesToDelete { get; set; }
    }
}
