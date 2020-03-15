using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Web.Services.Apaleo;

namespace Traces.Web
{
    public partial class App : ComponentBase
    {
        private readonly Theme _theme = new Theme
        {
            ColorOptions = new ThemeColorOptions
            {
                Primary = StyleConstants.PrimaryColor
            }
        };

        [Parameter]
        public string AccessToken { get; set; }

        [Inject]
        private IApaleoUserClaimValidatorService UserClaimValidatorService { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        private IRequestContext RequestContext { get; set; }

        [Inject]
        private ILogger<App> Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitializeContextAsync();

            await UserClaimValidatorService.AssertClaimAsync(AppConstants.AccountCodeQueryParameter, ApaleoClaims.AccountCode);
            await UserClaimValidatorService.AssertClaimAsync(AppConstants.SubjectIdQueryParameter, ApaleoClaims.SubjectId);

            await base.OnInitializedAsync();
        }

        /// <summary>
        /// This initialization of the context is required for server side Blazor.
        /// </summary>
        private async Task InitializeContextAsync()
        {
            var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authenticationState.User;
            Logger.LogInformation($"RequestContext is init: {RequestContext.IsInitialized}");
            Logger.LogInformation($"User is authenticated: {user.Identity.IsAuthenticated}");

            if (RequestContext.IsInitialized || !user.Identity.IsAuthenticated)
            {
                return;
            }

            var accessToken = AccessToken;
            Logger.LogInformation($"Access Token: {accessToken}");
            RequestContext.Initialize(user.Claims.ToList(), accessToken);
            Logger.LogInformation($"RequestContext is ready");
        }
    }
}
