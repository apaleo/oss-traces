using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Utils;

namespace Traces.Web.ViewModels
{
    public class BasePageModel : PageModel
    {
        private readonly IRequestContext _requestContext;

        protected BasePageModel(IRequestContext requestContext)
        {
            _requestContext = Check.NotNull(requestContext, nameof(requestContext));
        }

        /// <summary>
        /// This initialization of the context is required for server side Blazor.
        /// </summary>
        protected async Task InitializeContextAsync()
        {
            var httpContextUser = HttpContext.User;
            if (_requestContext.IsInitialized || !httpContextUser.Identity.IsAuthenticated)
            {
                return;
            }

            var accessToken = await HttpContext.GetTokenAsync(SecurityConstants.AccessToken);
            _requestContext.Initialize(httpContextUser.Claims.ToList(), accessToken);
        }
    }
}
