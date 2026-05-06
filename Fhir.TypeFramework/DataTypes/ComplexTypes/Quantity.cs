using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Quantity。</summary>
public class Quantity : ComplexTypeBase
{
    [JsonPropertyName("value")] public FhirDecimal? Value { get; set; }
    [JsonPropertyName("comparator")] public FhirCode? Comparator { get; set; }
    [JsonPropertyName("unit")] public FhirString? Unit { get; set; }
    [JsonPropertyName("system")] public FhirUri? System { get; set; }
    [JsonPropertyName("code")] public FhirCode? Code { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Quantity)copy;
        c.Value = Value?.DeepCopy() as FhirDecimal;
        c.Comparator = Comparator?.DeepCopy() as FhirCode;
        c.Unit = Unit?.DeepCopy() as FhirString;
        c.System = System?.DeepCopy() as FhirUri;
        c.Code = Code?.DeepCopy() as FhirCode;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Quantity)other;
        return ValueEquals(Value, o.Value)
               && ValueEquals(Comparator, o.Comparator)
               && ValueEquals(Unit, o.Unit)
               && ValueEquals(System, o.System)
               && ValueEquals(Code, o.Code);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Value, validationContext)) yield return r;
        foreach (var r in ValidateItem(Comparator, validationContext)) yield return r;
        foreach (var r in ValidateItem(Unit, validationContext)) yield return r;
        foreach (var r in ValidateItem(System, validationContext)) yield return r;
        foreach (var r in ValidateItem(Code, validationContext)) yield return r;
    }
}
