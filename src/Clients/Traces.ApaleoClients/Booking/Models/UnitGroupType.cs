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
    /// Defines values for UnitGroupType.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UnitGroupType
    {
        [EnumMember(Value = "BedRoom")]
        BedRoom,
        [EnumMember(Value = "MeetingRoom")]
        MeetingRoom,
        [EnumMember(Value = "EventSpace")]
        EventSpace,
        [EnumMember(Value = "ParkingLot")]
        ParkingLot,
        [EnumMember(Value = "Other")]
        Other
    }
    internal static class UnitGroupTypeEnumExtension
    {
        internal static string ToSerializedValue(this UnitGroupType? value)
        {
            return value == null ? null : ((UnitGroupType)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this UnitGroupType value)
        {
            switch( value )
            {
                case UnitGroupType.BedRoom:
                    return "BedRoom";
                case UnitGroupType.MeetingRoom:
                    return "MeetingRoom";
                case UnitGroupType.EventSpace:
                    return "EventSpace";
                case UnitGroupType.ParkingLot:
                    return "ParkingLot";
                case UnitGroupType.Other:
                    return "Other";
            }
            return null;
        }

        internal static UnitGroupType? ParseUnitGroupType(this string value)
        {
            switch( value )
            {
                case "BedRoom":
                    return UnitGroupType.BedRoom;
                case "MeetingRoom":
                    return UnitGroupType.MeetingRoom;
                case "EventSpace":
                    return UnitGroupType.EventSpace;
                case "ParkingLot":
                    return UnitGroupType.ParkingLot;
                case "Other":
                    return UnitGroupType.Other;
            }
            return null;
        }
    }
}