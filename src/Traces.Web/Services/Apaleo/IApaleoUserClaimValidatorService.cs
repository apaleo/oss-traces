namespace Traces.Web.Services.Apaleo
{
    public interface IApaleoUserClaimValidatorService
    {
        void AssertClaim(string queryParameter, string claimType);
    }
}