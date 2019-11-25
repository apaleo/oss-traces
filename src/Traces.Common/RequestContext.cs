using System;
using Traces.Common.Utils;

namespace Traces.Common
{
    public class RequestContext : IRequestContext
    {
        private bool _isInitialized;
        private string _accessToken;
        private string _tenantId;
        private string _subjectId;

        public string TenantId => CheckInitializedAndReturn(_tenantId);

        public string SubjectId => CheckInitializedAndReturn(_subjectId);

        public string AccessToken => CheckInitializedAndReturn(_accessToken);

        public void Initialize(string tenantId, string accessToken, string subjectId)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(RequestContext)} has already been initialized with {nameof(TenantId)} {TenantId}. Another initialization with {nameof(TenantId)} {tenantId} isn't possible anymore.");
            }

            _isInitialized = true;
            _tenantId = Check.NotEmpty(tenantId, nameof(tenantId));
            _accessToken = Check.NotEmpty(accessToken, nameof(accessToken));
            _subjectId = Check.NotEmpty(subjectId, nameof(subjectId));
        }

        private T CheckInitializedAndReturn<T>(T val)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(RequestContext)} hasn't been initialized yet and it's not possible to read the values yet.");
            }

            return val;
        }
    }
}