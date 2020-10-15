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

#pragma warning disable CA2227 // disables argument can be null
        public List<CreateTraceFileItemModel> FilesToUpload { get; set; }

        public List<int> FilesToDelete { get; set; }
#pragma warning restore CA2227

        public bool FileContainsNoPii { get; set; }
    }
}
