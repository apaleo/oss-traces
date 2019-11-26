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

    public partial class EmbeddedServiceModel
    {
        /// <summary>
        /// Initializes a new instance of the EmbeddedServiceModel class.
        /// </summary>
        public EmbeddedServiceModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the EmbeddedServiceModel class.
        /// </summary>
        /// <param name="id">The service id</param>
        /// <param name="code">The code for the service</param>
        /// <param name="description">The description for the service</param>
        /// <param name="name">The name for the service</param>
        public EmbeddedServiceModel(string id, string code = default(string), string description = default(string), string name = default(string))
        {
            Code = code;
            Description = description;
            Id = id;
            Name = name;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the code for the service
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the description for the service
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the service id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name for the service
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Id == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Id");
            }
        }
    }
}
