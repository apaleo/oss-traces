using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Common.Utils;
using Traces.Web.Services;

namespace Traces.Web.ViewModels
{
    public class IndexViewModel : BaseViewModel
    {
        private readonly IApaleoSetupService _apaleoSetupService;
        private readonly IOptions<ApaleoConfig> _apaleoIntegrationConfig;
        private readonly ILogger _logger;

        public IndexViewModel(
            IApaleoSetupService apaleoSetupService,
            IHttpContextAccessor httpContextAccessor,
            IRequestContext requestContext,
            IOptions<ApaleoConfig> apaleoIntegrationConfig,
            ILogger<IndexViewModel> logger)
            : base(httpContextAccessor, requestContext)
        {
            _apaleoSetupService = Check.NotNull(apaleoSetupService, nameof(apaleoSetupService));
            _apaleoIntegrationConfig = Check.NotNull(apaleoIntegrationConfig, nameof(apaleoIntegrationConfig));
            _logger = Check.NotNull(logger, nameof(logger));
        }

        public bool IsLoading { get; private set; }

        public string Title { get; private set; }

        public string Message { get; private set; }

        public string ButtonText { get; private set; }

        public bool IsSuccess { get; private set; }

        public string ApaleoRedirectUrl => $"https://app.apaleo.com/apps/{_apaleoIntegrationConfig.Value.ClientId.ToUpperInvariant()}-{_apaleoIntegrationConfig.Value.IntegrationConfig.DefaultIntegrationCode}";

        public async Task TriggerSetupAsync()
        {
            IsLoading = true;
            ButtonText = TextConstants.ApaleoSetupLoadingText;
            Title = TextConstants.ApaleoSetupLoadingTitle;
            Message = TextConstants.ApaleoSetupLoadingMessage;

            await InitializeContextAsync();

            try
            {
                await _apaleoSetupService.SetupApaleoUiIntegrationsAsync();
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
                _logger.LogError(ex, $"{nameof(IndexViewModel)} There was an issue while trying to setup the traces UI integrations");
            }

            IsLoading = false;
        }
    }
}