using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        public IndexViewModel(
            IApaleoSetupService apaleoSetupService,
            IHttpContextAccessor httpContextAccessor,
            IRequestContext requestContext)
            : base(httpContextAccessor, requestContext)
        {
            _apaleoSetupService = Check.NotNull(apaleoSetupService, nameof(apaleoSetupService));
        }

        public bool IsLoading { get; private set; }

        public string Title { get; private set; }

        public string Message { get; private set; }

        public string ButtonText { get; private set; }

        public bool IsSuccess { get; private set; }

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
            catch (BusinessValidationException)
            {
                IsSuccess = false;
                Title = TextConstants.ApaleoSetupErrorTitle;
                Message = TextConstants.ApaleoSetupErrorMessage;
                ButtonText = TextConstants.ApaleoSetupButtonTryAgainText;
            }

            IsLoading = false;
        }
    }
}