using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Identifier。</summary>
public class Identifier : ComplexTypeBase
{
    [JsonPropertyName("use")] public FhirCode? Use { get; set; }
    [JsonPropertyName("type")] public CodeableConcept? Type { get; set; }
    [JsonPropertyName("system")] public FhirUri? System { get; set; }
    [JsonPropertyName("value")] public FhirString? Value { get; set; }
    [JsonPropertyName("period")] public Period? Period { get; set; }
    [JsonPropertyName("assigner")] public Reference? Assigner { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Identifier)copy;
        c.Use = Use?.DeepCopy() as FhirCode;
        c.Type = Type?.DeepCopy() as CodeableConcept;
        c.System = System?.DeepCopy() as FhirUri;
        c.Value = Value?.DeepCopy() as FhirString;
        c.Period = Period?.DeepCopy() as Period;
        c.Assigner = Assigner?.DeepCopy() as Reference;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Identifier)other;
        return ValueEquals(Use, o.Use)
               && ValueEquals(Type, o.Type)
               && ValueEquals(System, o.System)
               && ValueEquals(Value, o.Value)
               && ValueEquals(Period, o.Period)
               && ValueEquals(Assigner, o.Assigner);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Use, validationContext)) yield return r;
        foreach (var r in ValidateItem(Type, validationContext)) yield return r;
        foreach (var r in ValidateItem(System, validationContext)) yield return r;
        foreach (var r in ValidateItem(Value, validationContext)) yield return r;
        foreach (var r in ValidateItem(Period, validationContext)) yield return r;
        foreach (var r in ValidateItem(Assigner, validationContext)) yield return r;
    }
}
