using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.JSInterop;
using Traces.Common.Constants;
using Traces.Web.Enums.ApaleoOne;

namespace Traces.Web.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IApaleoOneNotificationService _apaleoOneNotificationService;
        private readonly IToastService _toastService;
        private readonly IJSRuntime _jsRuntime;

        public NotificationService(IToastService toastService, IApaleoOneNotificationService apaleoOneNotificationService, IJSRuntime jsRuntime)
        {
            _toastService = toastService;
            _apaleoOneNotificationService = apaleoOneNotificationService;
            _jsRuntime = jsRuntime;
        }

        public async Task ShowSuccessAsync(string content, string title = TextConstants.SuccessHeaderText)
        {
            await ShowNotificationAsync(ToastLevel.Success, ApaleoNotificationType.Success, content, title);
        }

        public async Task ShowAlertAsync(string content, string title = TextConstants.AlertHeaderText)
        {
            await ShowNotificationAsync(ToastLevel.Warning, ApaleoNotificationType.Alert, content, title);
        }

        public async Task ShowErrorAsync(string content, string title = TextConstants.ErrorHeaderText)
        {
            await ShowNotificationAsync(ToastLevel.Error, ApaleoNotificationType.Error, content, title);
        }

        private async Task ShowNotificationAsync(ToastLevel toastLevel, ApaleoNotificationType apaleoNotificationType, string content, string title)
        {
            var result = await IsInIframe();

            if (result)
            {
                await _apaleoOneNotificationService.ShowNotificationAsync(apaleoNotificationType, content, title);
            }
            else
            {
                _toastService.ShowToast(toastLevel, content, title);
            }
        }

        private ValueTask<bool> IsInIframe() => _jsRuntime.InvokeAsync<bool>("blazorJsFunctions.isInIframe");
    }
}