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

    public partial class CreateBlockModel
    {
        /// <summary>
        /// Initializes a new instance of the CreateBlockModel class.
        /// </summary>
        public CreateBlockModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CreateBlockModel class.
        /// </summary>
        /// <param name="groupId">ID of the group that reserved the
        /// block</param>
        /// <param name="ratePlanId">The rate plan</param>
        /// <param name="fromProperty">Start date and time from which the
        /// inventory will be blocked&lt;br /&gt;Specify either a pure date or
        /// a date and time (without fractional second part) in UTC or with UTC
        /// offset as defined in &lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&gt;ISO8601:2004&lt;/a&gt;</param>
        /// <param name="to">End date and time until which the inventory will
        /// be blocked. Cannot be more than 5 years after the start date.&lt;br
        /// /&gt;Specify either a pure date or a date and time (without
        /// fractional second part) in UTC or with UTC offset as defined in
        /// &lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&gt;ISO8601:2004&lt;/a&gt;</param>
        /// <param name="timeSlices">The list of blocked units for each time
        /// slice</param>
        /// <param name="blockedUnits">Number of units to block for the defined
        /// time period</param>
        /// <param name="promoCode">The promo code associated with a certain
        /// special offer</param>
        /// <param name="corporateCode">The corporate code associated with a
        /// certain special offer</param>
        public CreateBlockModel(string groupId, string ratePlanId, string fromProperty, string to, MonetaryValueModel grossDailyRate, IList<CreateBlockTimeSliceModel> timeSlices = default(IList<CreateBlockTimeSliceModel>), int? blockedUnits = default(int?), string promoCode = default(string), string corporateCode = default(string))
        {
            GroupId = groupId;
            RatePlanId = ratePlanId;
            FromProperty = fromProperty;
            To = to;
            GrossDailyRate = grossDailyRate;
            TimeSlices = timeSlices;
            BlockedUnits = blockedUnits;
            PromoCode = promoCode;
            CorporateCode = corporateCode;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets ID of the group that reserved the block
        /// </summary>
        [JsonProperty(PropertyName = "groupId")]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the rate plan
        /// </summary>
        [JsonProperty(PropertyName = "ratePlanId")]
        public string RatePlanId { get; set; }

        /// <summary>
        /// Gets or sets start date and time from which the inventory will be
        /// blocked&amp;lt;br /&amp;gt;Specify either a pure date or a date and
        /// time (without fractional second part) in UTC or with UTC offset as
        /// defined in &amp;lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&amp;gt;ISO8601:2004&amp;lt;/a&amp;gt;
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public string FromProperty { get; set; }

        /// <summary>
        /// Gets or sets end date and time until which the inventory will be
        /// blocked. Cannot be more than 5 years after the start
        /// date.&amp;lt;br /&amp;gt;Specify either a pure date or a date and
        /// time (without fractional second part) in UTC or with UTC offset as
        /// defined in &amp;lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&amp;gt;ISO8601:2004&amp;lt;/a&amp;gt;
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "grossDailyRate")]
        public MonetaryValueModel GrossDailyRate { get; set; }

        /// <summary>
        /// Gets or sets the list of blocked units for each time slice
        /// </summary>
        [JsonProperty(PropertyName = "timeSlices")]
        public IList<CreateBlockTimeSliceModel> TimeSlices { get; set; }

        /// <summary>
        /// Gets or sets number of units to block for the defined time period
        /// </summary>
        [JsonProperty(PropertyName = "blockedUnits")]
        public int? BlockedUnits { get; set; }

        /// <summary>
        /// Gets or sets the promo code associated with a certain special offer
        /// </summary>
        [JsonProperty(PropertyName = "promoCode")]
        public string PromoCode { get; set; }

        /// <summary>
        /// Gets or sets the corporate code associated with a certain special
        /// offer
        /// </summary>
        [JsonProperty(PropertyName = "corporateCode")]
        public string CorporateCode { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (GroupId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "GroupId");
            }
            if (RatePlanId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "RatePlanId");
            }
            if (FromProperty == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "FromProperty");
            }
            if (To == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "To");
            }
            if (GrossDailyRate == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "GrossDailyRate");
            }
            if (GrossDailyRate != null)
            {
                GrossDailyRate.Validate();
            }
            if (TimeSlices != null)
            {
                foreach (var element in TimeSlices)
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
