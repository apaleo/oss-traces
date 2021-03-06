// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Traces.ApaleoClients.Booking.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for NotAllowedReservationTimeSliceActionReason.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotAllowedReservationTimeSliceActionReason
    {
        [EnumMember(Value = "AmendNotAllowedWhenTimeSliceIsInThePast")]
        AmendNotAllowedWhenTimeSliceIsInThePast,
        [EnumMember(Value = "AmendNotAllowedWhenTimeSliceIsAlreadyPosted")]
        AmendNotAllowedWhenTimeSliceIsAlreadyPosted,
        [EnumMember(Value = "AmendNotAllowedForReservationInFinalStatus")]
        AmendNotAllowedForReservationInFinalStatus
    }
    internal static class NotAllowedReservationTimeSliceActionReasonEnumExtension
    {
        internal static string ToSerializedValue(this NotAllowedReservationTimeSliceActionReason? value)
        {
            return value == null ? null : ((NotAllowedReservationTimeSliceActionReason)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this NotAllowedReservationTimeSliceActionReason value)
        {
            switch( value )
            {
                case NotAllowedReservationTimeSliceActionReason.AmendNotAllowedWhenTimeSliceIsInThePast:
                    return "AmendNotAllowedWhenTimeSliceIsInThePast";
                case NotAllowedReservationTimeSliceActionReason.AmendNotAllowedWhenTimeSliceIsAlreadyPosted:
                    return "AmendNotAllowedWhenTimeSliceIsAlreadyPosted";
                case NotAllowedReservationTimeSliceActionReason.AmendNotAllowedForReservationInFinalStatus:
                    return "AmendNotAllowedForReservationInFinalStatus";
            }
            return null;
        }

        internal static NotAllowedReservationTimeSliceActionReason? ParseNotAllowedReservationTimeSliceActionReason(this string value)
        {
            switch( value )
            {
                case "AmendNotAllowedWhenTimeSliceIsInThePast":
                    return NotAllowedReservationTimeSliceActionReason.AmendNotAllowedWhenTimeSliceIsInThePast;
                case "AmendNotAllowedWhenTimeSliceIsAlreadyPosted":
                    return NotAllowedReservationTimeSliceActionReason.AmendNotAllowedWhenTimeSliceIsAlreadyPosted;
                case "AmendNotAllowedForReservationInFinalStatus":
                    return NotAllowedReservationTimeSliceActionReason.AmendNotAllowedForReservationInFinalStatus;
            }
            return null;
        }
    }
}
