using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype CodeableReference。</summary>
public class CodeableReference : ComplexTypeBase
{
    [JsonPropertyName("concept")] public CodeableConcept? Concept { get; set; }
    [JsonPropertyName("reference")] public Reference? Reference { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (CodeableReference)copy;
        c.Concept = Concept?.DeepCopy() as CodeableConcept;
        c.Reference = Reference?.DeepCopy() as Reference;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (CodeableReference)other;
        return ValueEquals(Concept, o.Concept) && ValueEquals(Reference, o.Reference);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Concept, validationContext)) yield return r;
        foreach (var r in ValidateItem(Reference, validationContext)) yield return r;
    }
}
