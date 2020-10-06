using System;
using System.Collections.Generic;
using Traces.Web.Models.Files;

namespace Traces.Web.Models
{
    public class CreateTraceItemModel
    {
        public CreateTraceItemModel()
        {
            DueDate = DateTime.Now;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public string ReservationId { get; set; }

        public string PropertyId { get; set; }

        public string AssignedRole { get; set; }

#pragma warning disable CA2227 // disables argument can be null
        public List<CreateTraceFileItemModel> FilesToUpload { get; set; }
#pragma warning restore CA2227

        public bool FileContainsNoPii { get; set; }
    }
}
