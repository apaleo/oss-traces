using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Traces.Common;
using Traces.Web.Services;

namespace Traces.Web.ViewModels
{
    public class ApaleoBaseComponent : ComponentBase
    {
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        private IRequestContext RequestContext { get; set; }

        [Inject]
        private ITokenStorageService TokenStorageService { get; set; }

        [Inject]
        private ILogger<ApaleoBaseComponent> Logger { get; set; }

        /// <summary>
        /// This initialization of the context is required for server side Blazor.
        /// </summary>
        protected async Task InitializeContextAsync()
        {
            var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authenticationState.User;
            Logger.LogInformation($"RequestContext is init: {RequestContext.IsInitialized}. User is authenticated: {user.Identity.IsAuthenticated}. Access Token is null or whitespace: {string.IsNullOrWhiteSpace(TokenStorageService.AccessToken)}");

            if (RequestContext.IsInitialized || !user.Identity.IsAuthenticated)
            {
                return;
            }

            RequestContext.Initialize(user.Claims.ToList(), TokenStorageService.AccessToken);
            Logger.LogInformation($"RequestContext is ready");
        }
    }
}
