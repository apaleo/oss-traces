using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Web.Services.Apaleo;

namespace Traces.Web.Pages
{
    public partial class Index
    {
        public bool IsLoading { get; private set; }

        public string Title { get; private set; }

        public string Message { get; private set; }

        public string ButtonText { get; private set; }

        public bool IsSuccess { get; private set; }

#pragma warning disable CA1056 // We don't know how well blazor will take this as an URI

        public string ApaleoRedirectUrl => $"https://app.apaleo.com/apps/{ApaleoIntegrationConfig.Value.ClientId.ToUpperInvariant()}-{ApaleoIntegrationConfig.Value.IntegrationConfig.DefaultIntegrationCode}";

#pragma warning restore CA1056 // We don't know how well blazor will take this as an URI

        [Inject]
        private IApaleoSetupService ApaleoSetupService { get; set; }

        [Inject]
        private IOptions<ApaleoConfig> ApaleoIntegrationConfig { get; set; }

        [Inject]
        private ILogger<Index> Logger { get; set; }

        public async Task TriggerSetupAsync()
        {
            IsLoading = true;
            ButtonText = TextConstants.ApaleoSetupLoadingText;
            Title = TextConstants.ApaleoSetupLoadingTitle;
            Message = TextConstants.ApaleoSetupLoadingMessage;

            try
            {
                await ApaleoSetupService.SetupApaleoUiIntegrationsAsync();
                IsSuccess = true;
                Title = TextConstants.ApaleoSetupSuccessTitle;
                Message = TextConstants.ApaleoSetupSuccessMessage;
                ButtonText = TextConstants.ApaleoSetupButtonNavigateToApaleoText;
            }
            catch (BusinessValidationException ex)
            {
                IsSuccess = false;
                Title = TextConstants.ApaleoSetupErrorTitle;
                Message = TextConstants.ApaleoSetupErrorMessage;
                ButtonText = TextConstants.ApaleoSetupButtonTryAgainText;
                Logger.LogError(ex, $"{nameof(Index)} There was an issue while trying to setup the traces UI integrations");
            }

            IsLoading = false;
        }

        protected override async Task OnInitializedAsync()
        {
            await TriggerSetupAsync();

            await base.OnInitializedAsync();
        }
    }
}
