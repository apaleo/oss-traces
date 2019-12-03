using Traces.ApaleoClients.Booking;
using Traces.ApaleoClients.Inventory;

namespace Traces.Core.ClientFactories
{
    public interface IApaleoClientFactory
    {
        IBookingApi CreateBookingApi();

        IInventoryApi CreateInventoryApi();
    }
}