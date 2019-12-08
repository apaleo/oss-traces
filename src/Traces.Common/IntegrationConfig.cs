using System;
using Optional;

namespace Traces.Common
{
    public class IntegrationConfig
    {
        public string IntegrationBaseAddress { get; set; }

        public string IntegrationIconUrl { get; set; }

        public string IntegrationLabel { get; set; }

        public string DefaultIntegrationCode { get; set; }

        public Option<Uri> IntegrationIconUri => string.IsNullOrWhiteSpace(IntegrationIconUrl)
            ? Option.None<Uri>()
            : new Uri(IntegrationIconUrl).Some();
    }
}