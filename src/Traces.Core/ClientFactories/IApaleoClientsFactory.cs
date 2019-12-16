using Traces.ApaleoClients.Booking;
using Traces.ApaleoClients.Identity;
using Traces.ApaleoClients.Integration;
using Traces.ApaleoClients.Inventory;

namespace Traces.Core.ClientFactories
{
    public interface IApaleoClientsFactory
    {
        IBookingApi GetBookingApi();

        IInventoryApi GetInventoryApi();

        IIdentityApi GetIdentityApi();

        IIntegrationApi GetIntegrationApi();
    }
}