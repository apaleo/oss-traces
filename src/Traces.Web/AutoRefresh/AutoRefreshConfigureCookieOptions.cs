using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Traces.Web.AutoRefresh
{
    public class AutoRefreshConfigureCookieOptions : IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        private readonly AuthenticationScheme _signInScheme;

        public AutoRefreshConfigureCookieOptions(IAuthenticationSchemeProvider provider)
        {
            _signInScheme = provider.GetDefaultSignInSchemeAsync().GetAwaiter().GetResult();
        }

        public void Configure(CookieAuthenticationOptions options)
        {
            // Just copy/paste
        }

        public void Configure(string name, CookieAuthenticationOptions options)
        {
            if (name == _signInScheme.Name)
            {
                options.EventsType = typeof(AutoRefreshCookieEvents);
            }
        }
    }
}