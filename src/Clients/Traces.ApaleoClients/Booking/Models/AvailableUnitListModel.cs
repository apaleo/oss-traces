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

    public partial class AvailableUnitListModel
    {
        /// <summary>
        /// Initializes a new instance of the AvailableUnitListModel class.
        /// </summary>
        public AvailableUnitListModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the AvailableUnitListModel class.
        /// </summary>
        /// <param name="count">Total count of items</param>
        /// <param name="units">List of units</param>
        public AvailableUnitListModel(long count, IList<AvailableUnitItemModel> units)
        {
            Count = count;
            Units = units;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets total count of items
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public long Count { get; set; }

        /// <summary>
        /// Gets or sets list of units
        /// </summary>
        [JsonProperty(PropertyName = "units")]
        public IList<AvailableUnitItemModel> Units { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Units == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Units");
            }
            if (Units != null)
            {
                foreach (var element in Units)
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
