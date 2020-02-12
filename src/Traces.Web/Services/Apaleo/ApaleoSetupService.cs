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
    public class ApaleoSetupService : IApaleoSetupService
    {
        private readonly IApaleoClientsFactory _apaleoClientsFactory;
        private readonly IOptions<IntegrationConfig> _integrationConfig;

        private readonly Dictionary<ApaleoIntegrationTarget, Uri> _apaleoIntegrationTargetsUrlDictionary;

        public ApaleoSetupService(IApaleoClientsFactory apaleoIntegrationClientsFactory, IOptions<IntegrationConfig> integrationConfig)
        {
            _integrationConfig = Check.NotNull(integrationConfig, nameof(integrationConfig));
            _apaleoClientsFactory = Check.NotNull(apaleoIntegrationClientsFactory, nameof(apaleoIntegrationClientsFactory));
            _apaleoIntegrationTargetsUrlDictionary = new Dictionary<ApaleoIntegrationTarget, Uri>
            {
                {
                    ApaleoIntegrationTarget.AccountMenuApps,
                    new Uri($"{_integrationConfig.Value.IntegrationBaseAddress}{AppConstants.AccountLevelUrlAbsolutePath}")
                },
                {
                    ApaleoIntegrationTarget.PropertyMenuApps,
                    new Uri($"{_integrationConfig.Value.IntegrationBaseAddress}{AppConstants.PropertyLevelUrlAbsolutePath}")
                },
                {
                    ApaleoIntegrationTarget.ReservationDetailsTab,
                    new Uri($"{_integrationConfig.Value.IntegrationBaseAddress}{AppConstants.ReservationLevelUrlAbsolutePath}")
                }
            };
        }

        public async Task SetupApaleoUiIntegrationsAsync()
        {
            var nonExistentTargetKeys = await GetMissingIntegrationTargetsAsync();

            await CreateApaleoIntegrationsAsync(nonExistentTargetKeys);
        }

        private async Task<IReadOnlyList<ApaleoIntegrationTarget>> GetMissingIntegrationTargetsAsync()
        {
            var integrationApi = _apaleoClientsFactory.GetIntegrationApi();

            using (var requestResult = await integrationApi.IntegrationUiIntegrationsGetWithHttpMessagesAsync())
            {
                if (!requestResult.Response.IsSuccessStatusCode)
                {
                    throw new BusinessValidationException($"Request to get Integrations {nameof(integrationApi.IntegrationUiIntegrationsGetWithHttpMessagesAsync)} failed with status code: {requestResult.Response.StatusCode}");
                }

                var expectedTargets = _apaleoIntegrationTargetsUrlDictionary.Keys.ToList();

                if (requestResult.Body?.UiIntegrations == null)
                {
                    return expectedTargets;
                }

                // Get all the existing integrations codes in uppercase invariant for comparison,
                // as when received from Integration Api they are all in uppercase
                var existingIntegrationTargets = requestResult.Body.UiIntegrations.Where(x => x.Code == _integrationConfig.Value.DefaultIntegrationCode.ToUpperInvariant()).Select(x => x.Target).ToList();

                var nonExistentIntegrationCodes = expectedTargets.Where(target => !existingIntegrationTargets.Exists(t => t == target.ToString("G"))).ToList();

                return nonExistentIntegrationCodes;
            }
        }

        private async Task CreateApaleoIntegrationsAsync(IReadOnlyList<ApaleoIntegrationTarget> integrationTargets)
        {
            var tasks = integrationTargets.Select(CreateApaleoIntegrationAsync).ToList();

            await Task.WhenAll(tasks);
        }

        private async Task CreateApaleoIntegrationAsync(ApaleoIntegrationTarget integrationTarget)
        {
            var integrationApi = _apaleoClientsFactory.GetIntegrationApi();

            var integrationUrl = _apaleoIntegrationTargetsUrlDictionary[integrationTarget].ToString();

            var integrationIconUrl =
                _integrationConfig.Value.IntegrationIconUri.Match(
                    v => v.ToString(),
                    () => string.Empty);

            var createUiIntegrationModel = new CreateUiIntegrationModel
            {
                Code = _integrationConfig.Value.DefaultIntegrationCode,
                Label = _integrationConfig.Value.IntegrationLabel,
                IconSource = integrationIconUrl,
                SourceType = AppConstants.IntegrationSourceType,
                SourceUrl = integrationUrl
            };

            using (var responseResult = await integrationApi.IntegrationUiIntegrationsByTargetPostWithHttpMessagesAsync(integrationTarget.ToString("G"), createUiIntegrationModel))
            {
                if (!responseResult.Response.IsSuccessStatusCode && !await IntegrationExistsAsync(integrationTarget))
                {
                    var content = await responseResult.Response.Content.ReadAsStringAsync();
                    throw new BusinessValidationException(
                        $"Failed to create integration with {nameof(integrationApi.IntegrationUiIntegrationsByTargetPostWithHttpMessagesAsync)} with status code: {responseResult.Response.StatusCode} and content: {content}");
                }
            }
        }

        private async Task<bool> IntegrationExistsAsync(ApaleoIntegrationTarget target)
        {
            var nonExistingIntegrations = await GetMissingIntegrationTargetsAsync();

            return !nonExistingIntegrations.Contains(target);
        }
    }
}