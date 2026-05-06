using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype RatioRange。</summary>
public class RatioRange : ComplexTypeBase
{
    [JsonPropertyName("lowNumerator")] public Quantity? LowNumerator { get; set; }
    [JsonPropertyName("highNumerator")] public Quantity? HighNumerator { get; set; }
    [JsonPropertyName("denominator")] public Quantity? Denominator { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (RatioRange)copy;
        c.LowNumerator = LowNumerator?.DeepCopy() as Quantity;
        c.HighNumerator = HighNumerator?.DeepCopy() as Quantity;
        c.Denominator = Denominator?.DeepCopy() as Quantity;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (RatioRange)other;
        return ValueEquals(LowNumerator, o.LowNumerator)
               && ValueEquals(HighNumerator, o.HighNumerator)
               && ValueEquals(Denominator, o.Denominator);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(LowNumerator, validationContext)) yield return r;
        foreach (var r in ValidateItem(HighNumerator, validationContext)) yield return r;
        foreach (var r in ValidateItem(Denominator, validationContext)) yield return r;
    }
}
