using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Narrative。</summary>
public class Narrative : ComplexTypeBase
{
    [JsonPropertyName("status")] public FhirString? Status { get; set; }
    [JsonPropertyName("div")] public FhirXhtml? Div { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Narrative)copy;
        c.Status = Status?.DeepCopy() as FhirString;
        c.Div = Div?.DeepCopy() as FhirXhtml;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Narrative)other;
        return ValueEquals(Status, o.Status) && ValueEquals(Div, o.Div);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Status, validationContext)) yield return r;
        foreach (var r in ValidateItem(Div, validationContext)) yield return r;
    }
}
