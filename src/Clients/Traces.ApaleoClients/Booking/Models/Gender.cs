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
    /// Defines values for Gender.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Gender
    {
        [EnumMember(Value = "Female")]
        Female,
        [EnumMember(Value = "Male")]
        Male,
        [EnumMember(Value = "Other")]
        Other
    }
    internal static class GenderEnumExtension
    {
        internal static string ToSerializedValue(this Gender? value)
        {
            return value == null ? null : ((Gender)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this Gender value)
        {
            switch( value )
            {
                case Gender.Female:
                    return "Female";
                case Gender.Male:
                    return "Male";
                case Gender.Other:
                    return "Other";
            }
            return null;
        }

        internal static Gender? ParseGender(this string value)
        {
            switch( value )
            {
                case "Female":
                    return Gender.Female;
                case "Male":
                    return Gender.Male;
                case "Other":
                    return Gender.Other;
            }
            return null;
        }
    }
}
