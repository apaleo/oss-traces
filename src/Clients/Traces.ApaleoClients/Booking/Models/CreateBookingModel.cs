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
    /// With this request you can create a booking
    /// </summary>
    public partial class CreateBookingModel
    {
        /// <summary>
        /// Initializes a new instance of the CreateBookingModel class.
        /// </summary>
        public CreateBookingModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CreateBookingModel class.
        /// </summary>
        /// <param name="reservations">List of reservations to create</param>
        /// <param name="bookerComment">Additional information and comments by
        /// the booker</param>
        /// <param name="comment">Additional information and comments</param>
        public CreateBookingModel(BookerModel booker, IList<CreateReservationModel> reservations, string bookerComment = default(string), string comment = default(string), CreatePaymentAccountModel paymentAccount = default(CreatePaymentAccountModel))
        {
            Booker = booker;
            BookerComment = bookerComment;
            Comment = comment;
            PaymentAccount = paymentAccount;
            Reservations = reservations;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "booker")]
        public BookerModel Booker { get; set; }

        /// <summary>
        /// Gets or sets additional information and comments by the booker
        /// </summary>
        [JsonProperty(PropertyName = "bookerComment")]
        public string BookerComment { get; set; }

        /// <summary>
        /// Gets or sets additional information and comments
        /// </summary>
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "paymentAccount")]
        public CreatePaymentAccountModel PaymentAccount { get; set; }

        /// <summary>
        /// Gets or sets list of reservations to create
        /// </summary>
        [JsonProperty(PropertyName = "reservations")]
        public IList<CreateReservationModel> Reservations { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Booker == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Booker");
            }
            if (Reservations == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Reservations");
            }
            if (Booker != null)
            {
                Booker.Validate();
            }
            if (PaymentAccount != null)
            {
                PaymentAccount.Validate();
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
