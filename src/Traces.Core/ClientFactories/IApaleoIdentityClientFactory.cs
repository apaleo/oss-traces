using Traces.ApaleoClients.Identity;

namespace Traces.Core.ClientFactories
{
    public interface IApaleoIdentityClientFactory
    {
        IIdentityApi CreateIdentityApi();
    }
}