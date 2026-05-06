using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Range。</summary>
public class Range : ComplexTypeBase
{
    [JsonPropertyName("low")] public Quantity? Low { get; set; }
    [JsonPropertyName("high")] public Quantity? High { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Range)copy;
        c.Low = Low?.DeepCopy() as Quantity;
        c.High = High?.DeepCopy() as Quantity;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Range)other;
        return ValueEquals(Low, o.Low) && ValueEquals(High, o.High);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Low, validationContext)) yield return r;
        foreach (var r in ValidateItem(High, validationContext)) yield return r;
    }
}
