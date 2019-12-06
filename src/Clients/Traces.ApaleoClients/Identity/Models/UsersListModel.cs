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

    public partial class UsersListModel
    {
        /// <summary>
        /// Initializes a new instance of the UsersListModel class.
        /// </summary>
        public UsersListModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the UsersListModel class.
        /// </summary>
        /// <param name="users">A collection of users that have access to the
        /// current account</param>
        public UsersListModel(IList<UserItemModel> users)
        {
            Users = users;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets a collection of users that have access to the current
        /// account
        /// </summary>
        [JsonProperty(PropertyName = "users")]
        public IList<UserItemModel> Users { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Users == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Users");
            }
            if (Users != null)
            {
                foreach (var element in Users)
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
