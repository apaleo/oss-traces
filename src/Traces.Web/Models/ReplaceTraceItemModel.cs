using System;

namespace Traces.Web.Models
{
    public class ReplaceTraceItemModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public string AssignedRole { get; set; }
    }
}