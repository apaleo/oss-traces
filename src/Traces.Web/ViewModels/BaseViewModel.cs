using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Utils;

namespace Traces.Web.ViewModels
{
    public class BaseViewModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestContext _requestContext;
        private bool _isInitialized;

        protected BaseViewModel(IHttpContextAccessor httpContextAccessor, IRequestContext requestContext)
        {
            _httpContextAccessor = Check.NotNull(httpContextAccessor, nameof(httpContextAccessor));
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
        }

        /// <summary>
        /// This initialization of the context is required for server side Blazor.
        /// </summary>
        protected async Task<bool> InitializeContextAsync()
        {
            var httpContextUser = _httpContextAccessor.HttpContext.User;
            if (_isInitialized || !httpContextUser.Identity.IsAuthenticated)
            {
                return _isInitialized;
            }

            var tenantId = httpContextUser.Claims.FirstOrDefault(c => c.Type == ApaleoClaims.AccountCode);
            var subjectId = httpContextUser.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(SecurityConstants.AccessToken);
            _requestContext.Initialize(tenantId?.Value, accessToken, subjectId?.Value);

            _isInitialized = true;
            return true;
        }
    }
}