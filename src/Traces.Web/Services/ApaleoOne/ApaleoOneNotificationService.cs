using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Traces.Web.Enums.ApaleoOne;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public class ApaleoOneNotificationService : BaseApaleoOneService, IApaleoOneNotificationService
    {
        public ApaleoOneNotificationService(IJSRuntime jsRuntime)
            : base(jsRuntime)
        {
        }

        public async Task ShowNotificationAsync(ApaleoNotificationType type, string content, string title)
        {
            Console.WriteLine("====== Show notification async");

            var message = new ApaleoNotificationMessageModel
            {
                Title = title,
                Content = content,
                NotificationType = type
            };

            var messageString = SerializeObject(message);

            await JsRuntime.InvokeVoidAsync("parent.postMessage", messageString, "*");
        }
    }
}