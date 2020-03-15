using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Optional.Collections;
using Traces.Common.Constants;
using Traces.Common.Utils;
using Traces.Web.Utils;

namespace Traces.Web.Services.Apaleo
{
    public class ApaleoUserClaimValidatorService : IApaleoUserClaimValidatorService
    {
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILogger _logger;

        public ApaleoUserClaimValidatorService(
            NavigationManager navigationManager,
            AuthenticationStateProvider authenticationStateProvider,
            ILogger<ApaleoUserClaimValidatorService> logger)
        {
            _navigationManager = Check.NotNull(navigationManager, nameof(navigationManager));
            _authenticationStateProvider = Check.NotNull(authenticationStateProvider, nameof(authenticationStateProvider));
            _logger = Check.NotNull(logger, nameof(logger));
        }

        public async Task AssertClaimAsync(string queryParameter, string claimType)
        {
            _logger.LogInformation($"AssertClaim called with parameters: <{queryParameter}>, <{claimType}>");

            var extractedQueryParameter = UrlQueryParameterExtractor.ExtractQueryParameterFromManager(_navigationManager, queryParameter);
            if (string.IsNullOrWhiteSpace(extractedQueryParameter))
            {
                _logger.LogInformation($"RETURN: extractedQueryParameter is null, empty or whitespace: <{extractedQueryParameter}>");
                return;
            }

            _logger.LogInformation($"extractedQueryParameter is <{extractedQueryParameter}>");

            var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authenticationState.User;
            if (!user.Identity.IsAuthenticated)
            {
                _logger.LogInformation($"RETURN: user is not authenticated");
                return;
            }

            _logger.LogInformation($"user is not null and authenticated");

            var optionClaim = user.Claims.FirstOrNone(x => x.Type == claimType);
            _logger.LogInformation($"claim hasValue: <{optionClaim.HasValue}>");

            var value = optionClaim.ValueOr(new Claim(claimType, string.Empty)).Value;
            _logger.LogInformation($"value of claim: <{value}>");

            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogInformation($"RETURN: value (<{value}>) is null, empty, or whitespace");
                return;
            }

            if (extractedQueryParameter == value)
            {
                _logger.LogInformation($"RETURN: extractedQueryParameter(<{extractedQueryParameter}>) equals value(<{value}>)");
                return;
            }

            var encodedUrl = HttpUtility.UrlEncode(_navigationManager.Uri);
            _logger.LogInformation($"encodedUrl is: <{encodedUrl}>");
            _logger.LogInformation($"Navigating to: <{AppConstants.LogoutUrlAbsolutePath}?redirectPath={encodedUrl}>");
            _navigationManager.NavigateTo($"{AppConstants.LogoutUrlAbsolutePath}?redirectPath={encodedUrl}", true);
        }
    }
}
