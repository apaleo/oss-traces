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

    public partial class EmbeddedUnitModel
    {
        /// <summary>
        /// Initializes a new instance of the EmbeddedUnitModel class.
        /// </summary>
        public EmbeddedUnitModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the EmbeddedUnitModel class.
        /// </summary>
        /// <param name="id">The unit id</param>
        /// <param name="description">The description for the unit</param>
        /// <param name="name">The name for the unit</param>
        /// <param name="unitGroupId">The unit group id</param>
        public EmbeddedUnitModel(string id, string description = default(string), string name = default(string), string unitGroupId = default(string))
        {
            Description = description;
            Id = id;
            Name = name;
            UnitGroupId = unitGroupId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the description for the unit
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the unit id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name for the unit
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the unit group id
        /// </summary>
        [JsonProperty(PropertyName = "unitGroupId")]
        public string UnitGroupId { get; set; }

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
