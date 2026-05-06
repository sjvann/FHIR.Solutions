using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype CodeableConcept。</summary>
public class CodeableConcept : ComplexTypeBase
{
    [JsonPropertyName("coding")] public List<Coding>? Coding { get; set; }
    [JsonPropertyName("text")] public FhirString? Text { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (CodeableConcept)copy;
        c.Coding = DeepCopyList(Coding);
        c.Text = Text?.DeepCopy() as FhirString;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (CodeableConcept)other;
        return AreListsEqual(Coding, o.Coding) && ValueEquals(Text, o.Text);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateList(Coding, validationContext)) yield return r;
        foreach (var r in ValidateItem(Text, validationContext)) yield return r;
    }
}
