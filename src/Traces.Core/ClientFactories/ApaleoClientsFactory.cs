using System;
using System.Net.Http;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Traces.ApaleoClients.Booking;
using Traces.ApaleoClients.Identity;
using Traces.ApaleoClients.Integration;
using Traces.ApaleoClients.Inventory;
using Traces.Common;
using Traces.Common.Utils;

namespace Traces.Core.ClientFactories
{
    public class ApaleoClientsFactory : IApaleoClientsFactory
    {
        private readonly IRequestContext _requestContext;
        private readonly IOptions<ServicesUriConfig> _apaleoServicesUri;
        private readonly HttpClient _httpClient;
        private readonly object _initHttpClientLock = new object();

        private bool _httpClientIsInitialized;
        private IBookingApi _bookingApi;
        private IInventoryApi _inventoryApi;
        private IIdentityApi _identityApi;
        private IIntegrationApi _integrationApi;

        public ApaleoClientsFactory(IRequestContext requestContext, HttpClient httpClient, IOptions<ServicesUriConfig> apaleoServicesUri)
        {
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
            _httpClient = Check.NotNull(httpClient, nameof(httpClient));
            _apaleoServicesUri = Check.NotNull(apaleoServicesUri, nameof(apaleoServicesUri));
        }

        public IBookingApi GetBookingApi() => _bookingApi ??= CreateBookingApi();

        public IInventoryApi GetInventoryApi() => _inventoryApi ??= CreateInventoryApi();

        public IIdentityApi GetIdentityApi() => _identityApi ??= CreateIdentityApi();

        public IIntegrationApi GetIntegrationApi() => _integrationApi ??= CreateIntegrationApi();

        private IBookingApi CreateBookingApi() => new BookingApi(GetHttpClient(), false)
        {
            BaseUri = new Uri(_apaleoServicesUri.Value.ApiServiceUri)
        };

        private IInventoryApi CreateInventoryApi() => new InventoryApi(GetHttpClient(), false)
        {
            BaseUri = new Uri(_apaleoServicesUri.Value.ApiServiceUri)
        };

        private IIdentityApi CreateIdentityApi() => new IdentityApi(GetHttpClient(), false)
        {
            BaseUri = new Uri(_apaleoServicesUri.Value.IdentityServiceUri)
        };

        private IIntegrationApi CreateIntegrationApi() => new IntegrationApi(GetHttpClient(), false)
        {
            BaseUri = new Uri(_apaleoServicesUri.Value.IntegrationServiceUri)
        };

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