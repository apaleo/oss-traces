using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Traces.Common;
using Traces.Common.Constants;
using Traces.Common.Utils;

namespace Traces.Web.Services
{
    public class RequestContext : IRequestContext
    {
        private string _accessToken;
        private string _tenantId;
        private string _subjectId;

        public string TenantId => CheckInitializedAndReturn(_tenantId);

        public string SubjectId => CheckInitializedAndReturn(_subjectId);

        public bool IsInitialized { get; private set; }

        public string AccessToken => CheckInitializedAndReturn(_accessToken);

        public void InitializeFromClaims(IReadOnlyList<Claim> claims)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException(
                    $"{nameof(RequestContext)} has already been initialized with {nameof(TenantId)} {TenantId}. Another initialization isn't possible anymore.");
            }

            IsInitialized = true;

            var tenantId = claims.FirstOrDefault(c => c.Type == ApaleoClaims.AccountCode);
            var subjectId = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);

            _tenantId = Check.NotEmpty(tenantId?.Value, nameof(tenantId));
            _subjectId = Check.NotEmpty(subjectId?.Value, nameof(subjectId));
        }

        public void InitializeOrUpdateAccessToken(string accessToken) =>
            _accessToken = Check.NotEmpty(accessToken, nameof(accessToken));

        private T CheckInitializedAndReturn<T>(T val)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException(
                    $"{nameof(RequestContext)} hasn't been initialized yet and it's not possible to read the values yet.");
            }

            return val;
        }
    }
}