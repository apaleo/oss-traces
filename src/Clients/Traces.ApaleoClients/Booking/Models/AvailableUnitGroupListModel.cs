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

    public partial class AvailableUnitGroupListModel
    {
        /// <summary>
        /// Initializes a new instance of the AvailableUnitGroupListModel
        /// class.
        /// </summary>
        public AvailableUnitGroupListModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the AvailableUnitGroupListModel
        /// class.
        /// </summary>
        /// <param name="count">Total count of items</param>
        /// <param name="timeSlices">List of time slices</param>
        public AvailableUnitGroupListModel(long count, IList<UnitGroupAvailabilityTimeSliceItemModel> timeSlices)
        {
            Count = count;
            TimeSlices = timeSlices;
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
        /// Gets or sets list of time slices
        /// </summary>
        [JsonProperty(PropertyName = "timeSlices")]
        public IList<UnitGroupAvailabilityTimeSliceItemModel> TimeSlices { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (TimeSlices == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "TimeSlices");
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