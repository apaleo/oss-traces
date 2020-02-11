using System.Threading.Tasks;
using Traces.Common.Constants;

namespace Traces.Web.Services
{
    public interface INotificationService
    {
        Task ShowSuccessAsync(string content, string title = TextConstants.SuccessHeaderText);

        Task ShowAlertAsync(string content, string title = TextConstants.AlertHeaderText);

        Task ShowErrorAsync(string content, string title = TextConstants.ErrorHeaderText);
    }
}