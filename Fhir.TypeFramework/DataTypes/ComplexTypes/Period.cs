using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Period。</summary>
public class Period : ComplexTypeBase
{
    [JsonPropertyName("start")] public FhirDateTime? Start { get; set; }
    [JsonPropertyName("end")] public FhirDateTime? End { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Period)copy;
        c.Start = Start?.DeepCopy() as FhirDateTime;
        c.End = End?.DeepCopy() as FhirDateTime;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Period)other;
        return ValueEquals(Start, o.Start) && ValueEquals(End, o.End);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Start, validationContext)) yield return r;
        foreach (var r in ValidateItem(End, validationContext)) yield return r;
    }
}
