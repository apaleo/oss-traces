using System.Net.Http;
using IdentityModel.Client;
using Traces.ApaleoClients.Booking;
using Traces.ApaleoClients.Integration;
using Traces.ApaleoClients.Inventory;
using Traces.Common;
using Traces.Common.Utils;

namespace Traces.Core.ClientFactories
{
    public class ApaleoClientFactory : IApaleoClientFactory
    {
        private readonly IRequestContext _requestContext;
        private readonly HttpClient _httpClient;
        private readonly object _initHttpClientLock = new object();

        private bool _httpClientIsInitialized;

        public ApaleoClientFactory(IRequestContext requestContext, HttpClient httpClient)
        {
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
            _httpClient = Check.NotNull(httpClient, nameof(httpClient));
        }

        public IBookingApi CreateBookingApi()
        {
            IBookingApi api = new BookingApi(GetHttpClient(), false)
            {
                BaseUri = _httpClient.BaseAddress
            };

            return api;
        }

        public IInventoryApi CreateInventoryApi()
        {
            IInventoryApi api = new InventoryApi(GetHttpClient(), false)
            {
                BaseUri = _httpClient.BaseAddress
            };

            return api;
        }

        public IIntegrationApi CreateIntegrationApi()
        {
            IIntegrationApi api = new IntegrationApi(GetHttpClient(), false)
            {
                BaseUri = _httpClient.BaseAddress
            };

            return api;
        }

        private HttpClient GetHttpClient()
        {
            if (_httpClientIsInitialized)
            {
                return _httpClient;
            }

            lock (_initHttpClientLock)
            {
                if (!_httpClientIsInitialized)
                {
                    _httpClient.SetBearerToken(_requestContext.AccessToken);
                }
            }

            _httpClientIsInitialized = true;

            return _httpClient;
        }
    }
}