namespace Traces.Web.Models
{
    public class ApaleoNavigationMessageModel
    {
        public string Type { get; } = "navigation";

        public string Path { get; set; }

        public string Context { get; set; }

        public string Id { get; set; }
    }
}