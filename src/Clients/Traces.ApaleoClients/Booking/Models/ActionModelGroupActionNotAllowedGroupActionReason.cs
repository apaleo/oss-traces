// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Traces.ApaleoClients.Booking.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class ActionModelGroupActionNotAllowedGroupActionReason
    {
        /// <summary>
        /// Initializes a new instance of the
        /// ActionModelGroupActionNotAllowedGroupActionReason class.
        /// </summary>
        public ActionModelGroupActionNotAllowedGroupActionReason()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// ActionModelGroupActionNotAllowedGroupActionReason class.
        /// </summary>
        public ActionModelGroupActionNotAllowedGroupActionReason(bool isAllowed, IList<ActionReasonModelNotAllowedGroupActionReason> reasons = default(IList<ActionReasonModelNotAllowedGroupActionReason>))
        {
            IsAllowed = isAllowed;
            Reasons = reasons;
            CustomInit();
        }
        /// <summary>
        /// Static constructor for
        /// ActionModelGroupActionNotAllowedGroupActionReason class.
        /// </summary>
        static ActionModelGroupActionNotAllowedGroupActionReason()
        {
            Action = "Delete";
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isAllowed")]
        public bool IsAllowed { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "reasons")]
        public IList<ActionReasonModelNotAllowedGroupActionReason> Reasons { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "action")]
        public static string Action { get; private set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Reasons != null)
            {
                foreach (var element in Reasons)
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