using Optional;

namespace Traces.Web.Models
{
    public class ResultModel<T>
    {
        public bool Success { get; set; }

        public Option<T> Result { get; set; }

        public Option<string> ErrorMessage { get; set; }
    }
}