using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype ParameterDefinition。</summary>
public class ParameterDefinition : ComplexTypeBase
{
    [JsonPropertyName("name")] public FhirCode? Name { get; set; }
    [JsonPropertyName("use")] public FhirCode? Use { get; set; }
    [JsonPropertyName("min")] public FhirInteger? Min { get; set; }
    [JsonPropertyName("max")] public FhirString? Max { get; set; }
    [JsonPropertyName("documentation")] public FhirString? Documentation { get; set; }
    [JsonPropertyName("type")] public FhirString? Type { get; set; }
    [JsonPropertyName("profile")] public FhirCanonical? Profile { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (ParameterDefinition)copy;
        c.Name = Name?.DeepCopy() as FhirCode;
        c.Use = Use?.DeepCopy() as FhirCode;
        c.Min = Min?.DeepCopy() as FhirInteger;
        c.Max = Max?.DeepCopy() as FhirString;
        c.Documentation = Documentation?.DeepCopy() as FhirString;
        c.Type = Type?.DeepCopy() as FhirString;
        c.Profile = Profile?.DeepCopy() as FhirCanonical;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (ParameterDefinition)other;
        return ValueEquals(Name, o.Name)
               && ValueEquals(Use, o.Use)
               && ValueEquals(Min, o.Min)
               && ValueEquals(Max, o.Max)
               && ValueEquals(Documentation, o.Documentation)
               && ValueEquals(Type, o.Type)
               && ValueEquals(Profile, o.Profile);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Name, validationContext)) yield return r;
        foreach (var r in ValidateItem(Use, validationContext)) yield return r;
        foreach (var r in ValidateItem(Min, validationContext)) yield return r;
        foreach (var r in ValidateItem(Max, validationContext)) yield return r;
        foreach (var r in ValidateItem(Documentation, validationContext)) yield return r;
        foreach (var r in ValidateItem(Type, validationContext)) yield return r;
        foreach (var r in ValidateItem(Profile, validationContext)) yield return r;
    }
}
