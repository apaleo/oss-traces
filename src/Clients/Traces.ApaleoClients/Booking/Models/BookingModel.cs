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
    /// A booking holds all shared metadata for a set of reservations
    /// </summary>
    public partial class BookingModel
    {
        /// <summary>
        /// Initializes a new instance of the BookingModel class.
        /// </summary>
        public BookingModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the BookingModel class.
        /// </summary>
        /// <param name="id">Booking id</param>
        /// <param name="created">Date of creation&lt;br /&gt;A date and time
        /// (without fractional second part) in UTC or with UTC offset as
        /// defined in &lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&gt;ISO8601:2004&lt;/a&gt;</param>
        /// <param name="modified">Date of last modification&lt;br /&gt;A date
        /// and time (without fractional second part) in UTC or with UTC offset
        /// as defined in &lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&gt;ISO8601:2004&lt;/a&gt;</param>
        /// <param name="groupId">Group id</param>
        /// <param name="comment">Additional information and comments</param>
        /// <param name="bookerComment">Additional information and comment by
        /// the booker</param>
        /// <param name="propertyValues">Property specific values like total
        /// amount and balance</param>
        /// <param name="reservations">Reservations within this booking</param>
        public BookingModel(string id, System.DateTime created, System.DateTime modified, string groupId = default(string), BookerModel booker = default(BookerModel), PaymentAccountModel paymentAccount = default(PaymentAccountModel), string comment = default(string), string bookerComment = default(string), IList<PropertyValueModel> propertyValues = default(IList<PropertyValueModel>), IList<BookingReservationModel> reservations = default(IList<BookingReservationModel>))
        {
            Id = id;
            GroupId = groupId;
            Booker = booker;
            PaymentAccount = paymentAccount;
            Comment = comment;
            BookerComment = bookerComment;
            Created = created;
            Modified = modified;
            PropertyValues = propertyValues;
            Reservations = reservations;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets booking id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets group id
        /// </summary>
        [JsonProperty(PropertyName = "groupId")]
        public string GroupId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "booker")]
        public BookerModel Booker { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "paymentAccount")]
        public PaymentAccountModel PaymentAccount { get; set; }

        /// <summary>
        /// Gets or sets additional information and comments
        /// </summary>
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets additional information and comment by the booker
        /// </summary>
        [JsonProperty(PropertyName = "bookerComment")]
        public string BookerComment { get; set; }

        /// <summary>
        /// Gets or sets date of creation&amp;lt;br /&amp;gt;A date and time
        /// (without fractional second part) in UTC or with UTC offset as
        /// defined in &amp;lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&amp;gt;ISO8601:2004&amp;lt;/a&amp;gt;
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public System.DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets date of last modification&amp;lt;br /&amp;gt;A date
        /// and time (without fractional second part) in UTC or with UTC offset
        /// as defined in &amp;lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&amp;gt;ISO8601:2004&amp;lt;/a&amp;gt;
        /// </summary>
        [JsonProperty(PropertyName = "modified")]
        public System.DateTime Modified { get; set; }

        /// <summary>
        /// Gets or sets property specific values like total amount and balance
        /// </summary>
        [JsonProperty(PropertyName = "propertyValues")]
        public IList<PropertyValueModel> PropertyValues { get; set; }

        /// <summary>
        /// Gets or sets reservations within this booking
        /// </summary>
        [JsonProperty(PropertyName = "reservations")]
        public IList<BookingReservationModel> Reservations { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Id == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Id");
            }
            if (Booker != null)
            {
                Booker.Validate();
            }
            if (PaymentAccount != null)
            {
                PaymentAccount.Validate();
            }
            if (PropertyValues != null)
            {
                foreach (var element in PropertyValues)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
            if (Reservations != null)
            {
                foreach (var element1 in Reservations)
                {
                    if (element1 != null)
                    {
                        element1.Validate();
                    }
                }
            }
        }
    }
}
