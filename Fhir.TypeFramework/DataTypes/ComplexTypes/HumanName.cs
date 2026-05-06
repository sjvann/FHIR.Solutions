using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype HumanName。</summary>
public class HumanName : ComplexTypeBase
{
    [JsonPropertyName("use")] public FhirCode? Use { get; set; }
    [JsonPropertyName("text")] public FhirString? Text { get; set; }
    [JsonPropertyName("family")] public FhirString? Family { get; set; }
    [JsonPropertyName("given")] public List<FhirString>? Given { get; set; }
    [JsonPropertyName("prefix")] public List<FhirString>? Prefix { get; set; }
    [JsonPropertyName("suffix")] public List<FhirString>? Suffix { get; set; }
    [JsonPropertyName("period")] public Period? Period { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (HumanName)copy;
        c.Use = Use?.DeepCopy() as FhirCode;
        c.Text = Text?.DeepCopy() as FhirString;
        c.Family = Family?.DeepCopy() as FhirString;
        c.Given = DeepCopyList(Given);
        c.Prefix = DeepCopyList(Prefix);
        c.Suffix = DeepCopyList(Suffix);
        c.Period = Period?.DeepCopy() as Period;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (HumanName)other;
        return ValueEquals(Use, o.Use)
               && ValueEquals(Text, o.Text)
               && ValueEquals(Family, o.Family)
               && AreListsEqual(Given, o.Given)
               && AreListsEqual(Prefix, o.Prefix)
               && AreListsEqual(Suffix, o.Suffix)
               && ValueEquals(Period, o.Period);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Use, validationContext)) yield return r;
        foreach (var r in ValidateItem(Text, validationContext)) yield return r;
        foreach (var r in ValidateItem(Family, validationContext)) yield return r;
        foreach (var r in ValidateList(Given, validationContext)) yield return r;
        foreach (var r in ValidateList(Prefix, validationContext)) yield return r;
        foreach (var r in ValidateList(Suffix, validationContext)) yield return r;
        foreach (var r in ValidateItem(Period, validationContext)) yield return r;
    }
}
