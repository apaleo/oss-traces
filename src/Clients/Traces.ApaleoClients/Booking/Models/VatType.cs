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
    /// Defines values for VatType.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VatType
    {
        [EnumMember(Value = "Null")]
        Null,
        [EnumMember(Value = "VeryReduced")]
        VeryReduced,
        [EnumMember(Value = "Reduced")]
        Reduced,
        [EnumMember(Value = "Normal")]
        Normal,
        [EnumMember(Value = "Without")]
        Without,
        [EnumMember(Value = "Special")]
        Special,
        [EnumMember(Value = "ReducedCovid19")]
        ReducedCovid19,
        [EnumMember(Value = "NormalCovid19")]
        NormalCovid19
    }
    internal static class VatTypeEnumExtension
    {
        internal static string ToSerializedValue(this VatType? value)
        {
            return value == null ? null : ((VatType)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this VatType value)
        {
            switch( value )
            {
                case VatType.Null:
                    return "Null";
                case VatType.VeryReduced:
                    return "VeryReduced";
                case VatType.Reduced:
                    return "Reduced";
                case VatType.Normal:
                    return "Normal";
                case VatType.Without:
                    return "Without";
                case VatType.Special:
                    return "Special";
                case VatType.ReducedCovid19:
                    return "ReducedCovid19";
                case VatType.NormalCovid19:
                    return "NormalCovid19";
            }
            return null;
        }

        internal static VatType? ParseVatType(this string value)
        {
            switch( value )
            {
                case "Null":
                    return VatType.Null;
                case "VeryReduced":
                    return VatType.VeryReduced;
                case "Reduced":
                    return VatType.Reduced;
                case "Normal":
                    return VatType.Normal;
                case "Without":
                    return VatType.Without;
                case "Special":
                    return VatType.Special;
                case "ReducedCovid19":
                    return VatType.ReducedCovid19;
                case "NormalCovid19":
                    return VatType.NormalCovid19;
            }
            return null;
        }
    }
}
