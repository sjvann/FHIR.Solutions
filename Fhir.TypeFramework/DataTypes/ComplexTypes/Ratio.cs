using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Ratio。</summary>
public class Ratio : ComplexTypeBase
{
    [JsonPropertyName("numerator")] public Quantity? Numerator { get; set; }
    [JsonPropertyName("denominator")] public Quantity? Denominator { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Ratio)copy;
        c.Numerator = Numerator?.DeepCopy() as Quantity;
        c.Denominator = Denominator?.DeepCopy() as Quantity;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Ratio)other;
        return ValueEquals(Numerator, o.Numerator) && ValueEquals(Denominator, o.Denominator);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Numerator, validationContext)) yield return r;
        foreach (var r in ValidateItem(Denominator, validationContext)) yield return r;
    }
}
