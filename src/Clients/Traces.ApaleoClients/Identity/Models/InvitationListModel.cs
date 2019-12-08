// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Traces.ApaleoClients.Identity.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class InvitationListModel
    {
        /// <summary>
        /// Initializes a new instance of the InvitationListModel class.
        /// </summary>
        public InvitationListModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the InvitationListModel class.
        /// </summary>
        /// <param name="invitations">All invitations to the current
        /// account</param>
        public InvitationListModel(IList<InvitationModel> invitations)
        {
            Invitations = invitations;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets all invitations to the current account
        /// </summary>
        [JsonProperty(PropertyName = "invitations")]
        public IList<InvitationModel> Invitations { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Invitations == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Invitations");
            }
            if (Invitations != null)
            {
                foreach (var element in Invitations)
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
