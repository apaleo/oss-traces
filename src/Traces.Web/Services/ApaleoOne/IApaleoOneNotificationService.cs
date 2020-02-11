using System.Threading.Tasks;
using Traces.Web.Enums.ApaleoOne;

namespace Traces.Web.Services
{
    public interface IApaleoOneNotificationService
    {
        Task ShowNotificationAsync(ApaleoNotificationType type, string content, string title);
    }
}