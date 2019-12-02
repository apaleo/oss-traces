using Traces.ApaleoClients.Booking;
using Traces.ApaleoClients.Integration;
using Traces.ApaleoClients.Inventory;

namespace Traces.Core.ClientFactories
{
    public interface IApaleoClientFactory
    {
        IBookingApi CreateBookingApi();

        IInventoryApi CreateInventoryApi();

        IIntegrationApi CreateIntegrationApi();
    }
}