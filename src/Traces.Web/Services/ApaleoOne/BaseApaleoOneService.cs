using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Traces.Web.Services
{
    public class BaseApaleoOneService
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        protected BaseApaleoOneService(IJSRuntime runtime)
        {
            JsRuntime = runtime;
        }

        protected IJSRuntime JsRuntime { get; }

        protected string SerializeObject(object message) => JsonConvert.SerializeObject(message, _jsonSerializerSettings);
    }
}