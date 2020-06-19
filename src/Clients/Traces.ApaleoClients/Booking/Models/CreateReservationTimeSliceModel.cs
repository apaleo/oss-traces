// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Traces.ApaleoClients.Booking.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class CreateReservationTimeSliceModel
    {
        /// <summary>
        /// Initializes a new instance of the CreateReservationTimeSliceModel
        /// class.
        /// </summary>
        public CreateReservationTimeSliceModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CreateReservationTimeSliceModel
        /// class.
        /// </summary>
        /// <param name="ratePlanId">The rate plan id for this time
        /// slice</param>
        public CreateReservationTimeSliceModel(string ratePlanId, MonetaryValueModel totalAmount = default(MonetaryValueModel))
        {
            RatePlanId = ratePlanId;
            TotalAmount = totalAmount;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the rate plan id for this time slice
        /// </summary>
        [JsonProperty(PropertyName = "ratePlanId")]
        public string RatePlanId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "totalAmount")]
        public MonetaryValueModel TotalAmount { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (RatePlanId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "RatePlanId");
            }
            if (TotalAmount != null)
            {
                TotalAmount.Validate();
            }
        }
    }
}
