using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype RelatedArtifact。</summary>
public class RelatedArtifact : ComplexTypeBase
{
    [JsonPropertyName("type")] public FhirCode? Type { get; set; }
    [JsonPropertyName("label")] public FhirString? Label { get; set; }
    [JsonPropertyName("display")] public FhirString? Display { get; set; }
    [JsonPropertyName("citation")] public FhirString? Citation { get; set; }
    [JsonPropertyName("url")] public FhirUrl? Url { get; set; }
    [JsonPropertyName("resource")] public FhirCanonical? Resource { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (RelatedArtifact)copy;
        c.Type = Type?.DeepCopy() as FhirCode;
        c.Label = Label?.DeepCopy() as FhirString;
        c.Display = Display?.DeepCopy() as FhirString;
        c.Citation = Citation?.DeepCopy() as FhirString;
        c.Url = Url?.DeepCopy() as FhirUrl;
        c.Resource = Resource?.DeepCopy() as FhirCanonical;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (RelatedArtifact)other;
        return ValueEquals(Type, o.Type)
               && ValueEquals(Label, o.Label)
               && ValueEquals(Display, o.Display)
               && ValueEquals(Citation, o.Citation)
               && ValueEquals(Url, o.Url)
               && ValueEquals(Resource, o.Resource);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Type, validationContext)) yield return r;
        foreach (var r in ValidateItem(Label, validationContext)) yield return r;
        foreach (var r in ValidateItem(Display, validationContext)) yield return r;
        foreach (var r in ValidateItem(Citation, validationContext)) yield return r;
        foreach (var r in ValidateItem(Url, validationContext)) yield return r;
        foreach (var r in ValidateItem(Resource, validationContext)) yield return r;
    }
}
