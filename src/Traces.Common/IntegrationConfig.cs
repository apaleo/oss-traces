using System;
using Optional;

namespace Traces.Common
{
    public class IntegrationConfig
    {
        public string IntegrationBaseAddress { get; set; }

#pragma warning disable CA1056 // This uri needs to be strings because its an option loaded from the appsettings.json file

        public string IntegrationIconUrl { get; set; }

#pragma warning restore CA1056 // This uri needs to be strings because its an option loaded from the appsettings.json file

        public string IntegrationLabel { get; set; }

        public string DefaultIntegrationCode { get; set; }

        public Option<Uri> IntegrationIconUri => string.IsNullOrWhiteSpace(IntegrationIconUrl)
            ? Option.None<Uri>()
            : new Uri(IntegrationIconUrl).Some();
    }
}