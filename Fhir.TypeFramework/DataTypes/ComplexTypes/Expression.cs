using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Expression。</summary>
public class Expression : ComplexTypeBase
{
    [JsonPropertyName("description")] public FhirString? Description { get; set; }
    [JsonPropertyName("name")] public FhirId? Name { get; set; }
    [JsonPropertyName("language")] public FhirString? Language { get; set; }
    /// <summary>FHIR JSON 欄位名為 <c>expression</c>。</summary>
    [JsonPropertyName("expression")] public FhirString? Value { get; set; }
    [JsonPropertyName("reference")] public FhirUri? Reference { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Expression)copy;
        c.Description = Description?.DeepCopy() as FhirString;
        c.Name = Name?.DeepCopy() as FhirId;
        c.Language = Language?.DeepCopy() as FhirString;
        c.Value = Value?.DeepCopy() as FhirString;
        c.Reference = Reference?.DeepCopy() as FhirUri;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Expression)other;
        return ValueEquals(Description, o.Description)
               && ValueEquals(Name, o.Name)
               && ValueEquals(Language, o.Language)
               && ValueEquals(Value, o.Value)
               && ValueEquals(Reference, o.Reference);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Description, validationContext)) yield return r;
        foreach (var r in ValidateItem(Name, validationContext)) yield return r;
        foreach (var r in ValidateItem(Language, validationContext)) yield return r;
        foreach (var r in ValidateItem(Value, validationContext)) yield return r;
        foreach (var r in ValidateItem(Reference, validationContext)) yield return r;
    }
}
