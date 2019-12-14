using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Optional;
using Traces.Common.Constants;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public class ApaleoOneService : IApaleoOneService
    {
        private readonly IJSRuntime _jsRuntime;

        public ApaleoOneService(IJSRuntime runtime)
        {
            _jsRuntime = runtime;
        }

        public async Task<ResultModel<bool>> NavigateToReservation(TraceItemModel traceItemModel)
        {
            try
            {
                if (string.IsNullOrEmpty(traceItemModel.ReservationId) ||
                    string.IsNullOrEmpty(traceItemModel.PropertyId))
                {
                    throw new ArgumentException(TextConstants.ApaleoOneMessageItemIncomplete);
                }

                var message = new ApaleoNavigationMessageModel
                {
                    Path = "reservation-details",
                    Context = traceItemModel.PropertyId,
                    Id = traceItemModel.ReservationId
                };

                await _jsRuntime.InvokeVoidAsync("parent.postMessage", message, "*");

                return new ResultModel<bool>
                {
                    Result = Option.Some(true),
                    Success = true
                };
            }
            catch (ArgumentException e)
            {
                return new ResultModel<bool>
                {
                    Result = Option.None<bool>(),
                    Success = false,
                    ErrorMessage = e.Message.Some()
                };
            }
        }
    }
}