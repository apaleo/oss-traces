using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Optional.Collections;
using Traces.Common.Constants;
using Traces.Web.Utils;

namespace Traces.Web.Services.Apaleo
{
    public class ApaleoUserClaimValidatorService : IApaleoUserClaimValidatorService
    {
        private readonly NavigationManager _navigationManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApaleoUserClaimValidatorService(
            NavigationManager navigationManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _navigationManager = navigationManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public void AssertClaim(string queryParameter, string claimType)
        {
            var extractedQueryParameter = UrlQueryParameterExtractor.ExtractQueryParameterFromManager(_navigationManager, queryParameter);
            if (string.IsNullOrWhiteSpace(extractedQueryParameter))
            {
                return;
            }

            var user = _httpContextAccessor.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return;
            }

            var optionClaim = user.Claims.FirstOrNone(x => x.Type == claimType);
            var value = optionClaim.ValueOr(new Claim(claimType, string.Empty)).Value;

            if (string.IsNullOrWhiteSpace(value) || extractedQueryParameter.Equals(value))
            {
                return;
            }

            var encodedUrl = HttpUtility.UrlEncode(_navigationManager.Uri);
            _navigationManager.NavigateTo($"{AppConstants.LogoutUrlAbsolutePath}?redirectPath={encodedUrl}", true);
        }
    }
}
