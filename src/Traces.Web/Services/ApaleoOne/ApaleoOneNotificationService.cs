using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Common.Utils;
using Traces.Web.ApaleoOneConfig;
using Traces.Web.Models;

namespace Traces.Web.Services.ApaleoOne
{
    public class ApaleoOneNotificationService : BaseApaleoOneService, IApaleoOneNotificationService
    {
        private readonly ILogger _logger;

        public ApaleoOneNotificationService(IJSRuntime jsRuntime, ILogger<ApaleoOneNotificationService> logger)
            : base(jsRuntime)
        {
            _logger = Check.NotNull(logger, nameof(logger));
        }

        public async Task ShowSuccessAsync(string title, string content = null)
        {
            await ShowNotificationAsync(ApaleoNotificationType.Success, title, content);
        }

        public async Task ShowAlertAsync(string title, string content = null)
        {
            await ShowNotificationAsync(ApaleoNotificationType.Alert, title, content);
        }

        public async Task ShowErrorAsync(string title, string content = null)
        {
            await ShowNotificationAsync(ApaleoNotificationType.Error, title, content);
        }

        private async Task ShowNotificationAsync(ApaleoNotificationType type, string title, string content)
        {
            try
            {
                if (string.IsNullOrEmpty(title))
                {
                    throw new BusinessValidationException(TextConstants.ApaleoOneNotificationNotPossible);
                }

                var message = new ApaleoNotificationMessageModel
                {
                    Title = title,
                    Content = content,
                    NotificationType = type
                };

                var messageString = SerializeObject(message);

                await JsRuntime.InvokeVoidAsync("parent.postMessage", messageString, "*");
            }
            catch (BusinessValidationException ex)
            {
                _logger.LogWarning(ex, $"{nameof(ApaleoOneNotificationService)}.{nameof(ShowNotificationAsync)} - Exception while showing toast message");
            }
        }
    }
}