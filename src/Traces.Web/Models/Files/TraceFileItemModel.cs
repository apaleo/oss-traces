using System;
using Traces.Web.Enums;

namespace Traces.Web.Models.Files
{
    public class TraceFileItemModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string MimeType { get; set; }

        public long Size { get; set; }

        public Guid PublicId { get; set; }

        public string Path { get; set; }

        public string CreatedBy { get; set; }

        public int TraceId { get; set; }

        public TraceFileItemModelState State { get; set; } = TraceFileItemModelState.NoChanges;

        public bool IsStateChanged => State != TraceFileItemModelState.NoChanges;
    }
}
