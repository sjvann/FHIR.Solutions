using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype ContactPoint。</summary>
public class ContactPoint : ComplexTypeBase
{
    [JsonPropertyName("system")] public FhirCode? System { get; set; }
    [JsonPropertyName("value")] public FhirString? Value { get; set; }
    [JsonPropertyName("use")] public FhirCode? Use { get; set; }
    [JsonPropertyName("rank")] public FhirPositiveInt? Rank { get; set; }
    [JsonPropertyName("period")] public Period? Period { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (ContactPoint)copy;
        c.System = System?.DeepCopy() as FhirCode;
        c.Value = Value?.DeepCopy() as FhirString;
        c.Use = Use?.DeepCopy() as FhirCode;
        c.Rank = Rank?.DeepCopy() as FhirPositiveInt;
        c.Period = Period?.DeepCopy() as Period;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (ContactPoint)other;
        return ValueEquals(System, o.System)
               && ValueEquals(Value, o.Value)
               && ValueEquals(Use, o.Use)
               && ValueEquals(Rank, o.Rank)
               && ValueEquals(Period, o.Period);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(System, validationContext)) yield return r;
        foreach (var r in ValidateItem(Value, validationContext)) yield return r;
        foreach (var r in ValidateItem(Use, validationContext)) yield return r;
        foreach (var r in ValidateItem(Rank, validationContext)) yield return r;
        foreach (var r in ValidateItem(Period, validationContext)) yield return r;
    }
}
