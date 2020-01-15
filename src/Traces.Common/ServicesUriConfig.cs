namespace Traces.Common
{
    public class ServicesUriConfig
    {
        #pragma warning disable CA1056 // This uri needs to be strings because its an option loaded from the appsettings.json file

        public string ApiServiceUri { get; set; }

        public string IntegrationServiceUri { get; set; }

        public string IdentityServiceUri { get; set; }

        #pragma warning restore CA1056 // This uri needs to be strings because its an option loaded from the appsettings.json file
    }
}