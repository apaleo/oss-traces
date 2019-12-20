using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Traces.Common.Utils;
using Traces.Core.ClientFactories;

namespace Traces.Web.Services
{
    public class ApaleoRolesCollectorService : IApaleoRolesCollectorService
    {
        private readonly IApaleoClientsFactory _apaleoClientsFactory;
        private readonly ILogger _logger;

        public ApaleoRolesCollectorService(IApaleoClientsFactory apaleoClientsFactory, ILogger<ApaleoRolesCollectorService> logger)
        {
            _logger = Check.NotNull(logger, nameof(logger));
            _apaleoClientsFactory = Check.NotNull(apaleoClientsFactory, nameof(apaleoClientsFactory));
        }

        public async Task<IReadOnlyList<string>> GetRolesAsync()
        {
            var identityClient = _apaleoClientsFactory.GetIdentityApi();

            using (var requestResponse = await identityClient.ApiRolesGetWithHttpMessagesAsync())
            {
                if (requestResponse.Response.IsSuccessStatusCode)
                {
                    return requestResponse.Body.Roles.ToList();
                }
                else
                {
                    _logger.LogError($"Requesting roles to Identity service was not successful with status code: {requestResponse.Response.StatusCode}");
                    return new List<string>();
                }
            }
        }
    }
}