using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
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
            var claimValue = user.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;

            if (!extractedQueryParameter.Equals(claimValue))
            {
                var encodedUrl = HttpUtility.UrlEncode(_navigationManager.Uri);
                _navigationManager.NavigateTo($"{ApaleoOneConstants.LogoutUrlAbsolutePath}?redirectPath={encodedUrl}", true);
            }
        }
    }
}