using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Money。</summary>
public class Money : ComplexTypeBase
{
    [JsonPropertyName("value")] public FhirDecimal? Value { get; set; }
    [JsonPropertyName("currency")] public FhirCode? Currency { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Money)copy;
        c.Value = Value?.DeepCopy() as FhirDecimal;
        c.Currency = Currency?.DeepCopy() as FhirCode;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Money)other;
        return ValueEquals(Value, o.Value) && ValueEquals(Currency, o.Currency);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Value, validationContext)) yield return r;
        foreach (var r in ValidateItem(Currency, validationContext)) yield return r;
    }
}
