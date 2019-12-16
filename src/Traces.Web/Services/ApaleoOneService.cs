using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Optional;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public class ApaleoOneService : IApaleoOneService
    {
        private readonly IJSRuntime _jsRuntime;

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public ApaleoOneService(IJSRuntime runtime)
        {
            _jsRuntime = runtime;
        }

        public async Task<ResultModel<bool>> NavigateToReservationAsync(TraceItemModel traceItemModel)
        {
            try
            {
                if (string.IsNullOrEmpty(traceItemModel.ReservationId) ||
                    string.IsNullOrEmpty(traceItemModel.PropertyId))
                {
                    throw new BusinessValidationException(TextConstants.ApaleoOneMessageItemIncomplete);
                }

                var message = new ApaleoNavigationMessageModel
                {
                    Path = "reservation-details",
                    Context = traceItemModel.PropertyId,
                    Id = traceItemModel.ReservationId
                };

                var messageString = JsonConvert.SerializeObject(message, _jsonSerializerSettings);

                await _jsRuntime.InvokeVoidAsync("parent.postMessage", messageString, "*");

                return new ResultModel<bool>
                {
                    Result = true.Some(),
                    Success = true
                };
            }
            catch (BusinessValidationException e)
            {
                return new ResultModel<bool>
                {
                    Success = false,
                    ErrorMessage = e.Message.Some()
                };
            }
        }
    }
}