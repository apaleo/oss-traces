using System.Threading.Tasks;

namespace Traces.Web.Services.Apaleo
{
    public interface IApaleoSetupService
    {
        Task SetupApaleoUiIntegrationsAsync();
    }
}