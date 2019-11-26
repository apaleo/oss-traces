using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Traces.Common;
using Traces.Common.Constants;

namespace Traces.Web.Helpers
{
    public class ContextFilter : IAsyncActionFilter, IAsyncPageFilter
    {
        private readonly IRequestContext _context;

        private bool _contextInitialized;

        public ContextFilter(IRequestContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await InitializeContextIfNeededAsync(context);

            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            await next();
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            await InitializeContextIfNeededAsync(context);

            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            await next();
        }

        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            await InitializeContextIfNeededAsync(context);
        }

        private async Task InitializeContextIfNeededAsync(ActionContext context)
        {
            if (_contextInitialized)
            {
                return;
            }

            var accountCode = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ApaleoClaims.AccountCode);
            var subjectId = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);

            _context.Initialize(
                tenantId: accountCode?.Value,
                accessToken: await context.HttpContext.GetTokenAsync(SecurityConstants.AccessToken),
                subjectId: subjectId?.Value);

            _contextInitialized = true;
        }
    }
}