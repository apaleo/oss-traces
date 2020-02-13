using System.Threading.Tasks;

namespace Traces.Web.Services.ApaleoOne
{
    public interface IApaleoOneNotificationService
    {
        Task ShowSuccessAsync(string title, string content = null);

        Task ShowAlertAsync(string title, string content = null);

        Task ShowErrorAsync(string title, string content = null);
    }
}