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

    public partial class UnitGroupAvailabilityItemModel
    {
        /// <summary>
        /// Initializes a new instance of the UnitGroupAvailabilityItemModel
        /// class.
        /// </summary>
        public UnitGroupAvailabilityItemModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the UnitGroupAvailabilityItemModel
        /// class.
        /// </summary>
        /// <param name="allowedOverbookingCount">The number of units allowed
        /// for overbooking.</param>
        /// <param name="availableCount">The number of units still available.
        /// This is the house count excluding the out of order units minus
        /// the already sold units.</param>
        /// <param name="houseCount">The number of units physically existing
        /// excluding the ones which are out of inventory</param>
        /// <param name="occupancy">The percent value indicating the
        /// occupancy</param>
        /// <param name="physicalCount">The number of units physically existing
        /// on the property</param>
        /// <param name="sellableCount">The number of units available for
        /// selling. This is the minimum of the sellable units on property
        /// level
        /// and the available units of this unit group. If there are only 3
        /// units available on the property level
        /// but 5 units available for this unit group, the sellable count will
        /// be 3. This situation can occur if
        /// another category or the whole house is overbooked.</param>
        /// <param name="soldCount">The number of sold units including units
        /// picked up from blocks</param>
        public UnitGroupAvailabilityItemModel(int allowedOverbookingCount, int availableCount, BlockUnitsModel block, int houseCount, MaintenanceModel maintenance, double occupancy, int physicalCount, int sellableCount, int soldCount, EmbeddedUnitGroupModel unitGroup)
        {
            AllowedOverbookingCount = allowedOverbookingCount;
            AvailableCount = availableCount;
            Block = block;
            HouseCount = houseCount;
            Maintenance = maintenance;
            Occupancy = occupancy;
            PhysicalCount = physicalCount;
            SellableCount = sellableCount;
            SoldCount = soldCount;
            UnitGroup = unitGroup;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the number of units allowed for overbooking.
        /// </summary>
        [JsonProperty(PropertyName = "allowedOverbookingCount")]
        public int AllowedOverbookingCount { get; set; }

        /// <summary>
        /// Gets or sets the number of units still available. This is the house
        /// count excluding the out of order units minus
        /// the already sold units.
        /// </summary>
        [JsonProperty(PropertyName = "availableCount")]
        public int AvailableCount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "block")]
        public BlockUnitsModel Block { get; set; }

        /// <summary>
        /// Gets or sets the number of units physically existing excluding the
        /// ones which are out of inventory
        /// </summary>
        [JsonProperty(PropertyName = "houseCount")]
        public int HouseCount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "maintenance")]
        public MaintenanceModel Maintenance { get; set; }

        /// <summary>
        /// Gets or sets the percent value indicating the occupancy
        /// </summary>
        [JsonProperty(PropertyName = "occupancy")]
        public double Occupancy { get; set; }

        /// <summary>
        /// Gets or sets the number of units physically existing on the
        /// property
        /// </summary>
        [JsonProperty(PropertyName = "physicalCount")]
        public int PhysicalCount { get; set; }

        /// <summary>
        /// Gets or sets the number of units available for selling. This is the
        /// minimum of the sellable units on property level
        /// and the available units of this unit group. If there are only 3
        /// units available on the property level
        /// but 5 units available for this unit group, the sellable count will
        /// be 3. This situation can occur if
        /// another category or the whole house is overbooked.
        /// </summary>
        [JsonProperty(PropertyName = "sellableCount")]
        public int SellableCount { get; set; }

        /// <summary>
        /// Gets or sets the number of sold units including units picked up
        /// from blocks
        /// </summary>
        [JsonProperty(PropertyName = "soldCount")]
        public int SoldCount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "unitGroup")]
        public EmbeddedUnitGroupModel UnitGroup { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Block == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Block");
            }
            if (Maintenance == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Maintenance");
            }
            if (UnitGroup == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "UnitGroup");
            }
            if (Block != null)
            {
                Block.Validate();
            }
            if (Maintenance != null)
            {
                Maintenance.Validate();
            }
            if (UnitGroup != null)
            {
                UnitGroup.Validate();
            }
        }
    }
}
