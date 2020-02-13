namespace Traces.Web.Models
{
    public class ApaleoNotificationMessageModel
    {
        public string Type { get; } = "notification";

        public string Title { get; set; }

        public string Content { get; set; }

        public string NotificationType { get; set; }
    }
}