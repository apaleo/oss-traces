using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Traces.ApaleoClients.Integration.Models;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Enums;
using Traces.Common.Exceptions;
using Traces.Common.Utils;
using Traces.Core.ClientFactories;

namespace Traces.Web.Services
{
    internal class ApaleoSetupService : IApaleoSetupService
    {
        private readonly IApaleoIntegrationClientFactory _apaleoIntegrationClientFactory;
        private readonly IOptions<IntegrationConfig> _integrationConfig;

        private readonly Dictionary<string, ApaleoIntegrationTargetsEnum> apaleoIntegrationTargetDictionary =
            new Dictionary<string, ApaleoIntegrationTargetsEnum>
            {
                { ApaleoOneConstants.AccountMenuAppsIntegrationCode, ApaleoIntegrationTargetsEnum.AccountMenuApps },
                { ApaleoOneConstants.PropertyMenuAppsIntegrationCode, ApaleoIntegrationTargetsEnum.PropertyMenuApps },
                { ApaleoOneConstants.ReservationDetailsTabIntegrationCode, ApaleoIntegrationTargetsEnum.ReservationDetailsTab }
            };

        public ApaleoSetupService(IApaleoIntegrationClientFactory apaleoIntegrationClientFactory, IOptions<IntegrationConfig> integrationConfig)
        {
            _integrationConfig = Check.NotNull(integrationConfig, nameof(integrationConfig));
            _apaleoIntegrationClientFactory = Check.NotNull(apaleoIntegrationClientFactory, nameof(apaleoIntegrationClientFactory));
        }

        public async Task<bool> SetupApaleoUiIntegrationsAsync()
        {
            var nonExistentTargetKeys = await GetMissingIntegrationTargetsAsync();

            await CreateApaleoIntegrationsAsync(nonExistentTargetKeys);

            return true;
        }

        private async Task<IReadOnlyList<string>> GetMissingIntegrationTargetsAsync()
        {
            var integrationApi = _apaleoIntegrationClientFactory.CreateIntegrationApi();

            using (var requestResult = await integrationApi.IntegrationUiIntegrationsGetWithHttpMessagesAsync())
            {
                if (requestResult.Response.IsSuccessStatusCode)
                {
                    throw new BusinessValidationException($"Request to get Integrations {nameof(integrationApi.IntegrationUiIntegrationsGetWithHttpMessagesAsync)} failed with status code: {requestResult.Response.StatusCode}");
                }

                if (requestResult.Body?.UiIntegrations == null)
                {
                    return apaleoIntegrationTargetDictionary.Keys.ToList();
                }

                // Get all the existing integrations codes in uppercase invariant for comparison,
                // as when received from Integration Api they are all in uppercase
                var existingIntegrationCodes = requestResult.Body.UiIntegrations.Select(x => x.Code.ToUpperInvariant()).ToList();

                var nonExistentIntegrationCodes = apaleoIntegrationTargetDictionary.Keys.Select(k => k.ToUpperInvariant())
                    .Except(existingIntegrationCodes).ToList();

                return nonExistentIntegrationCodes;
            }
        }

        private async Task CreateApaleoIntegrationsAsync(IReadOnlyList<string> integrationKeys)
        {
            var tasks = new List<Task>();

            foreach (var integrationKey in integrationKeys)
            {
                if (apaleoIntegrationTargetDictionary.ContainsKey(integrationKey))
                {
                    tasks.Add(CreateApaleoIntegrationAsync(
                        integrationKey,
                        apaleoIntegrationTargetDictionary[integrationKey]));
                }
                else
                {
                    throw new InvalidOperationException($"Integration key: {integrationKey} does not exist in the list of due integrations to create");
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task CreateApaleoIntegrationAsync(string integrationCode, ApaleoIntegrationTargetsEnum integrationTargetEnum)
        {
            var integrationApi = _apaleoIntegrationClientFactory.CreateIntegrationApi();

            var integrationTarget = integrationTargetEnum.ToString("G");

            var integrationUrl = _integrationConfig.Value.IntegrationUrl;

            var integrationIconUrl = _integrationConfig.Value.IntegrationIconUrl;

            if (!Uri.IsWellFormedUriString(integrationUrl, UriKind.Absolute))
            {
                throw new BusinessValidationException($"Cannot create integration with invalid Url {integrationUrl}");
            }

            if (!string.IsNullOrWhiteSpace(integrationIconUrl) &&
                !Uri.IsWellFormedUriString(integrationIconUrl, UriKind.Absolute))
            {
                throw new BusinessValidationException($"Cannot create integration with invalid icon url {integrationIconUrl}");
            }

            var createUiIntegrationModel = new CreateUiIntegrationModel
            {
                Code = integrationCode,
                Label = _integrationConfig.Value.IntegrationLabel,
                IconSource = _integrationConfig.Value.IntegrationIconUrl,
                SourceType = ApaleoOneConstants.IntegrationSourceType,
                SourceUrl = _integrationConfig.Value.IntegrationUrl
            };

            using (var responseResult = await integrationApi.IntegrationUiIntegrationsByTargetPostWithHttpMessagesAsync(integrationTarget, createUiIntegrationModel))
            {
                if (!responseResult.Response.IsSuccessStatusCode)
                {
                    var content = await responseResult.Response.Content.ReadAsStringAsync();
                    throw new BusinessValidationException(
                        $"Failed to create integration with {nameof(integrationApi.IntegrationUiIntegrationsByTargetPostWithHttpMessagesAsync)} with status code: {responseResult.Response.StatusCode} and content: {content}");
                }
            }
        }
    }
}