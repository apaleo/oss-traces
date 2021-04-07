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

    public partial class BlockTimeSliceModel
    {
        /// <summary>
        /// Initializes a new instance of the BlockTimeSliceModel class.
        /// </summary>
        public BlockTimeSliceModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the BlockTimeSliceModel class.
        /// </summary>
        /// <param name="fromProperty">Start date and time from which units
        /// will be blocked&lt;br /&gt;A date and time (without fractional
        /// second part) in UTC or with UTC offset as defined in &lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&gt;ISO8601:2004&lt;/a&gt;</param>
        /// <param name="to">End date and time until which units will be
        /// blocked&lt;br /&gt;A date and time (without fractional second part)
        /// in UTC or with UTC offset as defined in &lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&gt;ISO8601:2004&lt;/a&gt;</param>
        /// <param name="blockedUnits">Number of units blocked for this time
        /// slice</param>
        /// <param name="pickedUnits">Number of units which have picked
        /// reservations for this time slice</param>
        public BlockTimeSliceModel(System.DateTime fromProperty, System.DateTime to, int blockedUnits, int pickedUnits, AmountModel baseAmount, MonetaryValueModel totalGrossAmount)
        {
            FromProperty = fromProperty;
            To = to;
            BlockedUnits = blockedUnits;
            PickedUnits = pickedUnits;
            BaseAmount = baseAmount;
            TotalGrossAmount = totalGrossAmount;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets start date and time from which units will be
        /// blocked&amp;lt;br /&amp;gt;A date and time (without fractional
        /// second part) in UTC or with UTC offset as defined in &amp;lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&amp;gt;ISO8601:2004&amp;lt;/a&amp;gt;
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public System.DateTime FromProperty { get; set; }

        /// <summary>
        /// Gets or sets end date and time until which units will be
        /// blocked&amp;lt;br /&amp;gt;A date and time (without fractional
        /// second part) in UTC or with UTC offset as defined in &amp;lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&amp;gt;ISO8601:2004&amp;lt;/a&amp;gt;
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public System.DateTime To { get; set; }

        /// <summary>
        /// Gets or sets number of units blocked for this time slice
        /// </summary>
        [JsonProperty(PropertyName = "blockedUnits")]
        public int BlockedUnits { get; set; }

        /// <summary>
        /// Gets or sets number of units which have picked reservations for
        /// this time slice
        /// </summary>
        [JsonProperty(PropertyName = "pickedUnits")]
        public int PickedUnits { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "baseAmount")]
        public AmountModel BaseAmount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "totalGrossAmount")]
        public MonetaryValueModel TotalGrossAmount { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (BaseAmount == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "BaseAmount");
            }
            if (TotalGrossAmount == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "TotalGrossAmount");
            }
            if (BaseAmount != null)
            {
                BaseAmount.Validate();
            }
            if (TotalGrossAmount != null)
            {
                TotalGrossAmount.Validate();
            }
        }
    }
}
