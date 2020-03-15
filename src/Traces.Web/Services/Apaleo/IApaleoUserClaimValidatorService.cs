using System.Threading.Tasks;

namespace Traces.Web.Services.Apaleo
{
    public interface IApaleoUserClaimValidatorService
    {
        Task AssertClaimAsync(string queryParameter, string claimType);
    }
}