namespace Traces.Common
{
    public interface IRequestContext
    {
        string AccessToken { get; }

        string TenantId { get; }

        void Initialize(string tenantId, string accessToken);
    }
}