using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public ApaleoUserClaimValidatorService(
            NavigationManager navigationManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ApaleoUserClaimValidatorService> logger)
        {
            _navigationManager = Check.NotNull(navigationManager, nameof(navigationManager));
            _httpContextAccessor = Check.NotNull(httpContextAccessor, nameof(httpContextAccessor));
            _logger = Check.NotNull(logger, nameof(logger));
        }

        public void AssertClaim(string queryParameter, string claimType)
        {
            _logger.LogInformation("================== Tracing ==================");
            _logger.LogInformation($"AssertClaim called with parameters: <{queryParameter}>, <{claimType}>");

            var extractedQueryParameter = UrlQueryParameterExtractor.ExtractQueryParameterFromManager(_navigationManager, queryParameter);
            if (string.IsNullOrWhiteSpace(extractedQueryParameter))
            {
                _logger.LogInformation($"RETURN: extractedQueryParameter is null, empty or whitespace: <{extractedQueryParameter}>");
                return;
            }

            _logger.LogInformation($"extractedQueryParameter is <{extractedQueryParameter}>");

            var user = _httpContextAccessor.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                _logger.LogInformation($"RETURN: user is null or unauthenticated");
                return;
            }

            _logger.LogInformation($"user is not null and authenticated");

            var optionClaim = user.Claims.FirstOrNone(x => x.Type == claimType);
            _logger.LogInformation($"claim hasValue: <{optionClaim.HasValue}>");

            var value = optionClaim.ValueOr(new Claim(claimType, string.Empty)).Value;
            _logger.LogInformation($"value of claim: <{value}>");

            if (string.IsNullOrWhiteSpace(value) || extractedQueryParameter.Equals(value))
            {
                _logger.LogInformation($"RETURN: value (<{value}>) is null or whitespace or extractedQueryParameter(<{extractedQueryParameter}>) equals value(<{value}>)");
                return;
            }

            var encodedUrl = HttpUtility.UrlEncode(_navigationManager.Uri);
            _logger.LogInformation($"encodedUrl is: <{encodedUrl}>");
            _logger.LogInformation($"Navigating to: <{AppConstants.LogoutUrlAbsolutePath}?redirectPath={encodedUrl}>");
            _navigationManager.NavigateTo($"{AppConstants.LogoutUrlAbsolutePath}?redirectPath={encodedUrl}", true);
        }
    }
}
