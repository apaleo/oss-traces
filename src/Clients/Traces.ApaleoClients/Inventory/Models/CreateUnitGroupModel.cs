// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Traces.ApaleoClients.Inventory.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class CreateUnitGroupModel
    {
        /// <summary>
        /// Initializes a new instance of the CreateUnitGroupModel class.
        /// </summary>
        public CreateUnitGroupModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CreateUnitGroupModel class.
        /// </summary>
        /// <param name="code">The code for the unit group that can be shown in
        /// reports and table views</param>
        /// <param name="description">The description for the unit
        /// group</param>
        /// <param name="maxPersons">Maximum number of persons for the unit
        /// group</param>
        /// <param name="name">The name for the unit group</param>
        /// <param name="propertyId">The id of the property where unit group
        /// will be created</param>
        /// <param name="rank">The unit group rank
        /// Restrictions:
        /// - Should be greater or equal to one</param>
        public CreateUnitGroupModel(string code, IDictionary<string, string> description, int maxPersons, IDictionary<string, string> name, string propertyId, int? rank = default(int?))
        {
            Code = code;
            Description = description;
            MaxPersons = maxPersons;
            Name = name;
            PropertyId = propertyId;
            Rank = rank;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the code for the unit group that can be shown in
        /// reports and table views
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the description for the unit group
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public IDictionary<string, string> Description { get; set; }

        /// <summary>
        /// Gets or sets maximum number of persons for the unit group
        /// </summary>
        [JsonProperty(PropertyName = "maxPersons")]
        public int MaxPersons { get; set; }

        /// <summary>
        /// Gets or sets the name for the unit group
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public IDictionary<string, string> Name { get; set; }

        /// <summary>
        /// Gets or sets the id of the property where unit group will be
        /// created
        /// </summary>
        [JsonProperty(PropertyName = "propertyId")]
        public string PropertyId { get; set; }

        /// <summary>
        /// Gets or sets the unit group rank
        /// Restrictions:
        /// - Should be greater or equal to one
        /// </summary>
        [JsonProperty(PropertyName = "rank")]
        public int? Rank { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Code == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Code");
            }
            if (Description == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Description");
            }
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
            if (PropertyId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "PropertyId");
            }
            if (Code != null)
            {
                if (Code.Length > 10)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "Code", 10);
                }
                if (Code.Length < 3)
                {
                    throw new ValidationException(ValidationRules.MinLength, "Code", 3);
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(Code, "^[a-zA-Z0-9_]*$"))
                {
                    throw new ValidationException(ValidationRules.Pattern, "Code", "^[a-zA-Z0-9_]*$");
                }
            }
            if (MaxPersons > 2147483647)
            {
                throw new ValidationException(ValidationRules.InclusiveMaximum, "MaxPersons", 2147483647);
            }
            if (MaxPersons < 1)
            {
                throw new ValidationException(ValidationRules.InclusiveMinimum, "MaxPersons", 1);
            }
            if (Rank > 2147483647)
            {
                throw new ValidationException(ValidationRules.InclusiveMaximum, "Rank", 2147483647);
            }
            if (Rank < 1)
            {
                throw new ValidationException(ValidationRules.InclusiveMinimum, "Rank", 1);
            }
        }
    }
}
