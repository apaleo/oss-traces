using System.Linq;
using System.Threading.Tasks;
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

        protected BaseViewModel(IHttpContextAccessor httpContextAccessor, IRequestContext requestContext)
        {
            _httpContextAccessor = Check.NotNull(httpContextAccessor, nameof(httpContextAccessor));
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
        }

        /// <summary>
        /// This initialization of the context is required for server side Blazor.
        /// </summary>
        protected async Task InitializeContextAsync()
        {
            var httpContextUser = _httpContextAccessor.HttpContext.User;
            if (_requestContext.IsInitialized || !httpContextUser.Identity.IsAuthenticated)
            {
                return;
            }

            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(SecurityConstants.AccessToken);
            _requestContext.Initialize(httpContextUser.Claims.ToList(), accessToken);
        }
    }
}