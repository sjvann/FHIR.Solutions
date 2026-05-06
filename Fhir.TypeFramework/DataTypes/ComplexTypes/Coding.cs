using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Coding。</summary>
public class Coding : ComplexTypeBase
{
    [JsonPropertyName("system")] public FhirUri? System { get; set; }
    [JsonPropertyName("version")] public FhirString? Version { get; set; }
    [JsonPropertyName("code")] public FhirString? Code { get; set; }
    [JsonPropertyName("display")] public FhirString? Display { get; set; }
    [JsonPropertyName("userSelected")] public FhirBoolean? UserSelected { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Coding)copy;
        c.System = System?.DeepCopy() as FhirUri;
        c.Version = Version?.DeepCopy() as FhirString;
        c.Code = Code?.DeepCopy() as FhirString;
        c.Display = Display?.DeepCopy() as FhirString;
        c.UserSelected = UserSelected?.DeepCopy() as FhirBoolean;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Coding)other;
        return ValueEquals(System, o.System)
               && ValueEquals(Version, o.Version)
               && ValueEquals(Code, o.Code)
               && ValueEquals(Display, o.Display)
               && ValueEquals(UserSelected, o.UserSelected);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(System, validationContext)) yield return r;
        foreach (var r in ValidateItem(Version, validationContext)) yield return r;
        foreach (var r in ValidateItem(Code, validationContext)) yield return r;
        foreach (var r in ValidateItem(Display, validationContext)) yield return r;
        foreach (var r in ValidateItem(UserSelected, validationContext)) yield return r;
    }
}
