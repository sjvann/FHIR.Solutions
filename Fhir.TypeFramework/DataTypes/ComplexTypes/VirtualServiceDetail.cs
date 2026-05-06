using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype VirtualServiceDetail。</summary>
public class VirtualServiceDetail : ComplexTypeBase
{
    [JsonPropertyName("channel")] public List<Coding>? Channel { get; set; }
    [JsonPropertyName("addressString")] public FhirString? AddressString { get; set; }
    [JsonPropertyName("addressUrl")] public FhirUrl? AddressUrl { get; set; }
    [JsonPropertyName("addressContactPoint")] public ContactPoint? AddressContactPoint { get; set; }
    [JsonPropertyName("addressReference")] public Reference? AddressReference { get; set; }
    [JsonPropertyName("sessionKey")] public FhirString? SessionKey { get; set; }
    [JsonPropertyName("additionalInfo")] public FhirString? AdditionalInfo { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (VirtualServiceDetail)copy;
        c.Channel = DeepCopyList(Channel);
        c.AddressString = AddressString?.DeepCopy() as FhirString;
        c.AddressUrl = AddressUrl?.DeepCopy() as FhirUrl;
        c.AddressContactPoint = AddressContactPoint?.DeepCopy() as ContactPoint;
        c.AddressReference = AddressReference?.DeepCopy() as Reference;
        c.SessionKey = SessionKey?.DeepCopy() as FhirString;
        c.AdditionalInfo = AdditionalInfo?.DeepCopy() as FhirString;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (VirtualServiceDetail)other;
        return AreListsEqual(Channel, o.Channel)
               && ValueEquals(AddressString, o.AddressString)
               && ValueEquals(AddressUrl, o.AddressUrl)
               && ValueEquals(AddressContactPoint, o.AddressContactPoint)
               && ValueEquals(AddressReference, o.AddressReference)
               && ValueEquals(SessionKey, o.SessionKey)
               && ValueEquals(AdditionalInfo, o.AdditionalInfo);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateList(Channel, validationContext)) yield return r;
        foreach (var r in ValidateItem(AddressString, validationContext)) yield return r;
        foreach (var r in ValidateItem(AddressUrl, validationContext)) yield return r;
        foreach (var r in ValidateItem(AddressContactPoint, validationContext)) yield return r;
        foreach (var r in ValidateItem(AddressReference, validationContext)) yield return r;
        foreach (var r in ValidateItem(SessionKey, validationContext)) yield return r;
        foreach (var r in ValidateItem(AdditionalInfo, validationContext)) yield return r;
    }
}
