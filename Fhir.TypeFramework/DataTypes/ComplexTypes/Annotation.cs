using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Annotation。</summary>
public class Annotation : ComplexTypeBase
{
    [JsonPropertyName("authorReference")] public Reference? AuthorReference { get; set; }
    [JsonPropertyName("authorString")] public FhirString? AuthorString { get; set; }
    [JsonPropertyName("time")] public FhirDateTime? Time { get; set; }
    [JsonPropertyName("text")] public FhirString? Text { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Annotation)copy;
        c.AuthorReference = AuthorReference?.DeepCopy() as Reference;
        c.AuthorString = AuthorString?.DeepCopy() as FhirString;
        c.Time = Time?.DeepCopy() as FhirDateTime;
        c.Text = Text?.DeepCopy() as FhirString;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Annotation)other;
        return ValueEquals(AuthorReference, o.AuthorReference)
               && ValueEquals(AuthorString, o.AuthorString)
               && ValueEquals(Time, o.Time)
               && ValueEquals(Text, o.Text);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(AuthorReference, validationContext)) yield return r;
        foreach (var r in ValidateItem(AuthorString, validationContext)) yield return r;
        foreach (var r in ValidateItem(Time, validationContext)) yield return r;
        foreach (var r in ValidateItem(Text, validationContext)) yield return r;
    }
}
