using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Traces.Common;
using Traces.Common.Constants;

namespace Traces.Web.AutoRefresh
{
    public class AutoRefreshCookieEvents : CookieAuthenticationEvents
    {
        private readonly ISystemClock _clock;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly IOptionsSnapshot<OpenIdConnectOptions> _oidcOptions;
        private readonly AutoRefreshOptions _refreshOptions;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IRequestContext _requestContext;

        public AutoRefreshCookieEvents(
            IOptions<AutoRefreshOptions> refreshOptions,
            IOptionsSnapshot<OpenIdConnectOptions> oidcOptions,
            IAuthenticationSchemeProvider schemeProvider,
            IHttpClientFactory httpClientFactory,
            IRequestContext requestContext,
            ILogger<AutoRefreshCookieEvents> logger,
            ISystemClock clock)
        {
            _refreshOptions = refreshOptions.Value;
            _oidcOptions = oidcOptions;
            _schemeProvider = schemeProvider;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _clock = clock;
            _requestContext = requestContext;
        }

        // important: this is just a POC at this point - it misses any thread synchronization. Will
        // add later.
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var tokens = context.Properties.GetTokens();
            if (tokens == null || !tokens.Any())
            {
                _logger.LogDebug("No tokens found in cookie properties. SaveTokens must be enabled for automatic token refresh.");
                return;
            }

            var refreshToken = tokens.SingleOrDefault(t => t.Name == OpenIdConnectParameterNames.RefreshToken);
            if (refreshToken == null)
            {
                _logger.LogWarning("No refresh token found in cookie properties. A refresh token must be requested and SaveTokens must be enabled.");
                return;
            }

            var expiresAt = tokens.SingleOrDefault(t => t.Name == "expires_at");
            if (expiresAt == null)
            {
                _logger.LogWarning("No expires_at value found in cookie properties.");
                return;
            }

            var dtExpires = DateTimeOffset.Parse(expiresAt.Value, CultureInfo.InvariantCulture);
            var dtRefresh = dtExpires.Subtract(_refreshOptions.RefreshBeforeExpiration);

            if (dtRefresh < _clock.UtcNow)
            {
                var oidcOptions = await GetOidcOptionsAsync();
                var configuration = await oidcOptions.ConfigurationManager.GetConfigurationAsync(default(CancellationToken));

                var tokenClient = _httpClientFactory.CreateClient("tokenClient");

                using (var request = new RefreshTokenRequest
                {
                    Address = configuration.TokenEndpoint,
                    ClientId = oidcOptions.ClientId,
                    ClientSecret = oidcOptions.ClientSecret,
                    RefreshToken = refreshToken.Value
                })
                {
                    var response = await tokenClient.RequestRefreshTokenAsync(request);

                    if (response.IsError)
                    {
                        _logger.LogWarning("Error refreshing token: {error}", response.Error);
                        return;
                    }

                    context.Properties.UpdateTokenValue(SecurityConstants.AccessToken, response.AccessToken);
                    context.Properties.UpdateTokenValue("refresh_token", response.RefreshToken);

                    var newExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);
                    context.Properties.UpdateTokenValue("expires_at", newExpiresAt.ToString("o", CultureInfo.InvariantCulture));
                }

                await context.HttpContext.SignInAsync(context.Principal, context.Properties);
            }

            InitializeRequestContext(context.Principal, context.Properties.GetTokens());
        }

        private async Task<OpenIdConnectOptions> GetOidcOptionsAsync()
        {
            if (string.IsNullOrEmpty(_refreshOptions.Scheme))
            {
                var scheme = await _schemeProvider.GetDefaultChallengeSchemeAsync();
                return _oidcOptions.Get(scheme.Name);
            }
            else
            {
                return _oidcOptions.Get(_refreshOptions.Scheme);
            }
        }

        private void InitializeRequestContext(ClaimsPrincipal claimsPrincipal, IEnumerable<AuthenticationToken> authenticationToken)
        {
            var accessToken = authenticationToken.FirstOrDefault(t => t.Name == SecurityConstants.AccessToken);

            _requestContext.InitializeOrUpdateAccessToken(accessToken.Value);

            if (_requestContext.IsInitialized || !claimsPrincipal.Identity.IsAuthenticated)
            {
                return;
            }

            var tenantId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ApaleoClaims.AccountCode);
            var subjectId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);

            _requestContext.Initialize(tenantId?.Value, subjectId?.Value);
        }
    }
}