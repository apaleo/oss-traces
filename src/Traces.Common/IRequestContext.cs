namespace Traces.Common
{
    public interface IRequestContext
    {
        string AccessToken { get; }

        string TenantId { get; }

        string SubjectId { get; }

        bool IsInitialized { get; }

        void Initialize(string tenantId, string subjectId);

        void InitializeOrUpdateAccessToken(string accessToken);
    }
}