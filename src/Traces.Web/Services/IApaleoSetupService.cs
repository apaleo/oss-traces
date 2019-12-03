using System.Threading.Tasks;

namespace Traces.Web.Services
{
    public interface IApaleoSetupService
    {
        Task<bool> SetupApaleoUiIntegrationsAsync();
    }
}