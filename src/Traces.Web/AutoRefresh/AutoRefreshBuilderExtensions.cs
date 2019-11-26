using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Traces.Web.AutoRefresh
{
    public static class AutoRefreshBuilderExtensions
    {
        public static AuthenticationBuilder AddAutomaticTokenRefresh(this AuthenticationBuilder builder)
        {
            builder.Services.AddTransient<AutoRefreshCookieEvents>();
            builder.Services.AddSingleton<IConfigureOptions<CookieAuthenticationOptions>, AutoRefreshConfigureCookieOptions>();

            builder.Services.AddHttpClient("tokenClient");

            return builder;
        }
    }
}