#pragma warning disable CA2227 // Can be disabled in models
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

        public List<CreateTraceFileItemModel> FilesToUpload { get; set; }
    }
}
