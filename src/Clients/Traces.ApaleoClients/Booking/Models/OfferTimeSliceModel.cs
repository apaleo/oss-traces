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

    public partial class OfferTimeSliceModel
    {
        /// <summary>
        /// Initializes a new instance of the OfferTimeSliceModel class.
        /// </summary>
        public OfferTimeSliceModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OfferTimeSliceModel class.
        /// </summary>
        /// <param name="availableUnits">The number of available units for that
        /// time slice</param>
        /// <param name="fromProperty">The start date and time for this time
        /// slice&lt;br /&gt;A date and time (without fractional second part)
        /// in UTC or with UTC offset as defined in &lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&gt;ISO8601:2004&lt;/a&gt;</param>
        /// <param name="to">The end date and time for this time slice&lt;br
        /// /&gt;A date and time (without fractional second part) in UTC or
        /// with UTC offset as defined in &lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&gt;ISO8601:2004&lt;/a&gt;</param>
        /// <param name="includedServices">The breakdown for services included
        /// in the offer</param>
        public OfferTimeSliceModel(int availableUnits, AmountModel baseAmount, System.DateTime fromProperty, System.DateTime to, MonetaryValueModel totalGrossAmount, IList<OfferServiceModel> includedServices = default(IList<OfferServiceModel>))
        {
            AvailableUnits = availableUnits;
            BaseAmount = baseAmount;
            FromProperty = fromProperty;
            IncludedServices = includedServices;
            To = to;
            TotalGrossAmount = totalGrossAmount;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the number of available units for that time slice
        /// </summary>
        [JsonProperty(PropertyName = "availableUnits")]
        public int AvailableUnits { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "baseAmount")]
        public AmountModel BaseAmount { get; set; }

        /// <summary>
        /// Gets or sets the start date and time for this time slice&amp;lt;br
        /// /&amp;gt;A date and time (without fractional second part) in UTC or
        /// with UTC offset as defined in &amp;lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&amp;gt;ISO8601:2004&amp;lt;/a&amp;gt;
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public System.DateTime FromProperty { get; set; }

        /// <summary>
        /// Gets or sets the breakdown for services included in the offer
        /// </summary>
        [JsonProperty(PropertyName = "includedServices")]
        public IList<OfferServiceModel> IncludedServices { get; set; }

        /// <summary>
        /// Gets or sets the end date and time for this time slice&amp;lt;br
        /// /&amp;gt;A date and time (without fractional second part) in UTC or
        /// with UTC offset as defined in &amp;lt;a
        /// href="https://en.wikipedia.org/wiki/ISO_8601"&amp;gt;ISO8601:2004&amp;lt;/a&amp;gt;
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public System.DateTime To { get; set; }

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
            if (IncludedServices != null)
            {
                foreach (var element in IncludedServices)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
            if (TotalGrossAmount != null)
            {
                TotalGrossAmount.Validate();
            }
        }
    }
}
