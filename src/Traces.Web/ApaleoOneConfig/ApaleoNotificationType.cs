namespace Traces.Web.ApaleoOneConfig
{
    public sealed class ApaleoNotificationType
    {
        public static readonly ApaleoNotificationType Success = new ApaleoNotificationType("success");
        public static readonly ApaleoNotificationType Alert = new ApaleoNotificationType("alert");
        public static readonly ApaleoNotificationType Error = new ApaleoNotificationType("error");

        private ApaleoNotificationType(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static implicit operator string(ApaleoNotificationType type) => type.Value;

        public override string ToString() => Value;
    }
}