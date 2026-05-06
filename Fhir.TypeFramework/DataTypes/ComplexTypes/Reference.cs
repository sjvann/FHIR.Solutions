using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Reference。</summary>
public class Reference : ComplexTypeBase
{
    [JsonPropertyName("reference")] public FhirString? ReferenceValue { get; set; }
    [JsonPropertyName("type")] public FhirUri? Type { get; set; }
    [JsonPropertyName("identifier")] public Identifier? Identifier { get; set; }
    [JsonPropertyName("display")] public FhirString? Display { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Reference)copy;
        c.ReferenceValue = ReferenceValue?.DeepCopy() as FhirString;
        c.Type = Type?.DeepCopy() as FhirUri;
        c.Identifier = Identifier?.DeepCopy() as Identifier;
        c.Display = Display?.DeepCopy() as FhirString;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Reference)other;
        return ValueEquals(ReferenceValue, o.ReferenceValue)
               && ValueEquals(Type, o.Type)
               && ValueEquals(Identifier, o.Identifier)
               && ValueEquals(Display, o.Display);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(ReferenceValue, validationContext)) yield return r;
        foreach (var r in ValidateItem(Type, validationContext)) yield return r;
        foreach (var r in ValidateItem(Identifier, validationContext)) yield return r;
        foreach (var r in ValidateItem(Display, validationContext)) yield return r;
    }
}
