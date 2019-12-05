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

        void Initialize(IReadOnlyList<Claim> claims, string accessToken);
    }
}