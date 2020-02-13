using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Traces.Common.Utils;

namespace Traces.Web.Services.ApaleoOne
{
    public class BaseApaleoOneService
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        protected BaseApaleoOneService(IJSRuntime runtime)
        {
            JsRuntime = Check.NotNull(runtime, nameof(runtime));
        }

        protected IJSRuntime JsRuntime { get; }

        protected string SerializeObject(object message) => JsonConvert.SerializeObject(message, _jsonSerializerSettings);
    }
}