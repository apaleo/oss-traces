using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Traces.ApaleoClients.Integration.Models;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Enums;
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
                requestResult.Response.EnsureSuccessStatusCode();

                if (requestResult.Body?.UiIntegrations == null)
                {
                    return new List<string>();
                }

                var existingIntegrationCodes = requestResult.Body.UiIntegrations.Select(x => x.Code).ToList();

                var nonExistentIntegrationCodes = apaleoIntegrationTargetDictionary.Keys.ToList()
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
                    tasks.Add(CreateApaleoIntegrationAsync(integrationKey,
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

            var createUiIntegrationModel = new CreateUiIntegrationModel
            {
                Code = integrationCode,
                Label = _integrationConfig.Value.IntegrationLabel,
                IconSource = _integrationConfig.Value.IntegrationIconUrl,
                SourceType = ApaleoOneConstants.IntegrationSourceType,
                SourceUrl = _integrationConfig.Value.IntegrationUrl
            };

            using (var response = await integrationApi.IntegrationUiIntegrationsByTargetPostWithHttpMessagesAsync(integrationTarget, createUiIntegrationModel))
            {
                // TODO
            }
        }
    }
}