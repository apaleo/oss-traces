namespace Traces.Common
{
    public interface IRequestContext
    {
        string AccessToken { get; }

        string TenantId { get; }

        string SubjectId { get; }

        void Initialize(string tenantId, string accessToken, string subjectId);
    }
}