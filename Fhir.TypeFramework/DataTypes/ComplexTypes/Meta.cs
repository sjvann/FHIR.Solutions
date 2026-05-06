using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Meta。</summary>
public class Meta : ComplexTypeBase
{
    [JsonPropertyName("versionId")]
    public FhirId? VersionId { get; set; }

    [JsonPropertyName("lastUpdated")]
    public FhirInstant? LastUpdated { get; set; }

    [JsonPropertyName("source")]
    public FhirUri? Source { get; set; }

    [JsonPropertyName("profile")]
    public List<FhirCanonical>? Profile { get; set; }

    [JsonPropertyName("security")]
    public List<Coding>? Security { get; set; }

    [JsonPropertyName("tag")]
    public List<Coding>? Tag { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Meta)copy;
        c.VersionId = VersionId?.DeepCopy() as FhirId;
        c.LastUpdated = LastUpdated?.DeepCopy() as FhirInstant;
        c.Source = Source?.DeepCopy() as FhirUri;
        c.Profile = DeepCopyList(Profile);
        c.Security = DeepCopyList(Security);
        c.Tag = DeepCopyList(Tag);
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Meta)other;
        return ValueEquals(VersionId, o.VersionId)
               && ValueEquals(LastUpdated, o.LastUpdated)
               && ValueEquals(Source, o.Source)
               && AreListsEqual(Profile, o.Profile)
               && AreListsEqual(Security, o.Security)
               && AreListsEqual(Tag, o.Tag);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(VersionId, validationContext)) yield return r;
        foreach (var r in ValidateItem(LastUpdated, validationContext)) yield return r;
        foreach (var r in ValidateItem(Source, validationContext)) yield return r;
        foreach (var r in ValidateList(Profile, validationContext)) yield return r;
        foreach (var r in ValidateList(Security, validationContext)) yield return r;
        foreach (var r in ValidateList(Tag, validationContext)) yield return r;
    }
}
