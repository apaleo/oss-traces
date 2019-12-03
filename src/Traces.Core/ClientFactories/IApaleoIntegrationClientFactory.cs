using Traces.ApaleoClients.Integration;

namespace Traces.Core.ClientFactories
{
    public interface IApaleoIntegrationClientFactory
    {
        IIntegrationApi CreateIntegrationApi();
    }
}