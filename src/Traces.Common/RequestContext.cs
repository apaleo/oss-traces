using System;
using Traces.Common.Utils;

namespace Traces.Common
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

        public void Initialize(string tenantId, string subjectId)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException($"{nameof(RequestContext)} has already been initialized with {nameof(TenantId)} {TenantId}. Another initialization with {nameof(TenantId)} {tenantId} isn't possible anymore.");
            }

            IsInitialized = true;
            _tenantId = Check.NotEmpty(tenantId, nameof(tenantId));
            _subjectId = Check.NotEmpty(subjectId, nameof(subjectId));
        }

        public void InitializeOrUpdateAccessToken(string accessToken) =>
            _accessToken = Check.NotEmpty(accessToken, nameof(accessToken));

        private T CheckInitializedAndReturn<T>(T val)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException($"{nameof(RequestContext)} hasn't been initialized yet and it's not possible to read the values yet.");
            }

            return val;
        }
    }
}