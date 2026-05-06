using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype DataRequirement。</summary>
public class DataRequirement : ComplexTypeBase
{
    [JsonPropertyName("type")] public FhirCode? Type { get; set; }
    [JsonPropertyName("profile")] public List<FhirCanonical>? Profile { get; set; }
    [JsonPropertyName("codeFilter")] public List<BackboneElement>? CodeFilter { get; set; }
    [JsonPropertyName("dateFilter")] public List<BackboneElement>? DateFilter { get; set; }
    [JsonPropertyName("sort")] public FhirString? Sort { get; set; }
    [JsonPropertyName("limit")] public FhirInteger? Limit { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (DataRequirement)copy;
        c.Type = Type?.DeepCopy() as FhirCode;
        c.Profile = DeepCopyList(Profile);
        c.CodeFilter = DeepCopyList(CodeFilter);
        c.DateFilter = DeepCopyList(DateFilter);
        c.Sort = Sort?.DeepCopy() as FhirString;
        c.Limit = Limit?.DeepCopy() as FhirInteger;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (DataRequirement)other;
        return ValueEquals(Type, o.Type)
               && AreListsEqual(Profile, o.Profile)
               && AreListsEqual(CodeFilter, o.CodeFilter)
               && AreListsEqual(DateFilter, o.DateFilter)
               && ValueEquals(Sort, o.Sort)
               && ValueEquals(Limit, o.Limit);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Type, validationContext)) yield return r;
        foreach (var r in ValidateList(Profile, validationContext)) yield return r;
        foreach (var r in ValidateList(CodeFilter, validationContext)) yield return r;
        foreach (var r in ValidateList(DateFilter, validationContext)) yield return r;
        foreach (var r in ValidateItem(Sort, validationContext)) yield return r;
        foreach (var r in ValidateItem(Limit, validationContext)) yield return r;
    }
}
