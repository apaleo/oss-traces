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

    public partial class ActionModelBlockActionNotAllowedBlockActionReason
    {
        /// <summary>
        /// Initializes a new instance of the
        /// ActionModelBlockActionNotAllowedBlockActionReason class.
        /// </summary>
        public ActionModelBlockActionNotAllowedBlockActionReason()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// ActionModelBlockActionNotAllowedBlockActionReason class.
        /// </summary>
        /// <param name="action">Possible values include: 'Delete', 'Confirm',
        /// 'Release', 'Cancel', 'Pickup', 'Modify', 'Wash'</param>
        public ActionModelBlockActionNotAllowedBlockActionReason(BlockAction action, bool isAllowed, IList<ActionReasonModelNotAllowedBlockActionReason> reasons = default(IList<ActionReasonModelNotAllowedBlockActionReason>))
        {
            Action = action;
            IsAllowed = isAllowed;
            Reasons = reasons;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'Delete', 'Confirm',
        /// 'Release', 'Cancel', 'Pickup', 'Modify', 'Wash'
        /// </summary>
        [JsonProperty(PropertyName = "action")]
        public BlockAction Action { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isAllowed")]
        public bool IsAllowed { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "reasons")]
        public IList<ActionReasonModelNotAllowedBlockActionReason> Reasons { get; set; }

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