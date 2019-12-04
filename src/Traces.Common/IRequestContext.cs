using System.Collections.Generic;
using System.Security.Claims;

namespace Traces.Common
{
    public interface IRequestContext
    {
        string AccessToken { get; }

        string TenantId { get; }

        string SubjectId { get; }

        bool IsInitialized { get; }

        void InitializeFromClaims(IReadOnlyList<Claim> claimsPrincipal);

        void InitializeOrUpdateAccessToken(string accessToken);
    }
}