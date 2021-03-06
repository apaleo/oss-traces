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

    public partial class ServiceModel
    {
        /// <summary>
        /// Initializes a new instance of the ServiceModel class.
        /// </summary>
        public ServiceModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ServiceModel class.
        /// </summary>
        /// <param name="id">The service id</param>
        /// <param name="code">The code for the service</param>
        /// <param name="name">The name for the service</param>
        /// <param name="description">The description for the service</param>
        /// <param name="pricingUnit">Defines the granularity (room, person)
        /// for which this item is offered and priced. Possible values include:
        /// 'Room', 'Person'</param>
        public ServiceModel(string id, string code, string name, string description, PricedUnit pricingUnit, MonetaryValueModel defaultGrossPrice)
        {
            Id = id;
            Code = code;
            Name = name;
            Description = description;
            PricingUnit = pricingUnit;
            DefaultGrossPrice = defaultGrossPrice;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the service id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the code for the service
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name for the service
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for the service
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets defines the granularity (room, person) for which this
        /// item is offered and priced. Possible values include: 'Room',
        /// 'Person'
        /// </summary>
        [JsonProperty(PropertyName = "pricingUnit")]
        public PricedUnit PricingUnit { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "defaultGrossPrice")]
        public MonetaryValueModel DefaultGrossPrice { get; set; }

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
            if (Code == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Code");
            }
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
            if (Description == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Description");
            }
            if (DefaultGrossPrice == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "DefaultGrossPrice");
            }
            if (DefaultGrossPrice != null)
            {
                DefaultGrossPrice.Validate();
            }
        }
    }
}
