using System.Net.Http;
using IdentityModel.Client;
using Traces.ApaleoClients.Identity;
using Traces.ApaleoClients.Integration;
using Traces.Common;
using Traces.Common.Utils;

namespace Traces.Core.ClientFactories
{
    /// <summary>
    /// This is a separate factory because it is pointing to a separate base address.
    /// Further clean up will be performed with: https://github.com/apaleo/oss-traces/issues/27
    /// </summary>
    public class ApaleoIdentityClientFactory : IApaleoIdentityClientFactory
    {
        private readonly IRequestContext _requestContext;
        private readonly HttpClient _httpClient;
        private readonly object _initHttpClientLock = new object();

        private bool _httpClientIsInitialized;

        public ApaleoIdentityClientFactory(HttpClient httpClient, IRequestContext requestContext)
        {
            _httpClient = Check.NotNull(httpClient, nameof(httpClient));
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
        }

        public IIdentityApi CreateIdentityApi()
        {
            var api = new IdentityApi(GetHttpClient(), false)
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