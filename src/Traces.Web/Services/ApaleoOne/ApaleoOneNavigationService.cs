using System.Threading.Tasks;
using Microsoft.JSInterop;
using Optional;
using Traces.Common.Constants;
using Traces.Common.Exceptions;
using Traces.Web.Models;

namespace Traces.Web.Services.ApaleoOne
{
    public class ApaleoOneNavigationService : BaseApaleoOneService, IApaleoOneNavigationService
    {
        public ApaleoOneNavigationService(IJSRuntime runtime)
            : base(runtime)
        {
        }

        public async Task<ResultModel<bool>> NavigateToReservationAsync(TraceItemModel traceItemModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(traceItemModel.ReservationId) ||
                    string.IsNullOrWhiteSpace(traceItemModel.PropertyId))
                {
                    throw new BusinessValidationException(TextConstants.ApaleoOneNavigationNotPossible);
                }

                var message = new ApaleoNavigationMessageModel
                {
                    Path = "reservation-details",
                    Context = traceItemModel.PropertyId,
                    Id = traceItemModel.ReservationId
                };

                var messageString = SerializeObject(message);

                await JsRuntime.InvokeVoidAsync("parent.postMessage", messageString, "*");

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