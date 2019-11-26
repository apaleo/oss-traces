// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Traces.ApaleoClients.Booking.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// With this request you can pick up reservations to an existing group
    /// booking
    /// </summary>
    public partial class PickUpReservationsModel
    {
        /// <summary>
        /// Initializes a new instance of the PickUpReservationsModel class.
        /// </summary>
        public PickUpReservationsModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the PickUpReservationsModel class.
        /// </summary>
        /// <param name="reservations">List of reservations to pick up to the
        /// existing group booking</param>
        public PickUpReservationsModel(IList<PickUpReservationModel> reservations)
        {
            Reservations = reservations;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets list of reservations to pick up to the existing group
        /// booking
        /// </summary>
        [JsonProperty(PropertyName = "reservations")]
        public IList<PickUpReservationModel> Reservations { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Reservations == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Reservations");
            }
            if (Reservations != null)
            {
                foreach (var element in Reservations)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}
