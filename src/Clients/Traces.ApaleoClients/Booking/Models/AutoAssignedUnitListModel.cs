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

    public partial class AutoAssignedUnitListModel
    {
        /// <summary>
        /// Initializes a new instance of the AutoAssignedUnitListModel class.
        /// </summary>
        public AutoAssignedUnitListModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the AutoAssignedUnitListModel class.
        /// </summary>
        /// <param name="timeSlices">The list of time slices with the
        /// respective assigned unit</param>
        public AutoAssignedUnitListModel(IList<AutoAssignedUnitItemModel> timeSlices)
        {
            TimeSlices = timeSlices;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the list of time slices with the respective assigned
        /// unit
        /// </summary>
        [JsonProperty(PropertyName = "timeSlices")]
        public IList<AutoAssignedUnitItemModel> TimeSlices { get; set; }

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
