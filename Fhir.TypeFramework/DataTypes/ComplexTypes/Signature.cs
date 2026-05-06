using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Signature。</summary>
public class Signature : ComplexTypeBase
{
    [JsonPropertyName("type")] public List<Coding>? Type { get; set; }
    [JsonPropertyName("when")] public FhirInstant? When { get; set; }
    [JsonPropertyName("who")] public Reference? Who { get; set; }
    [JsonPropertyName("onBehalfOf")] public Reference? OnBehalfOf { get; set; }
    [JsonPropertyName("targetFormat")] public FhirCode? TargetFormat { get; set; }
    [JsonPropertyName("sigFormat")] public FhirCode? SigFormat { get; set; }
    [JsonPropertyName("data")] public FhirBase64Binary? Data { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Signature)copy;
        c.Type = DeepCopyList(Type);
        c.When = When?.DeepCopy() as FhirInstant;
        c.Who = Who?.DeepCopy() as Reference;
        c.OnBehalfOf = OnBehalfOf?.DeepCopy() as Reference;
        c.TargetFormat = TargetFormat?.DeepCopy() as FhirCode;
        c.SigFormat = SigFormat?.DeepCopy() as FhirCode;
        c.Data = Data?.DeepCopy() as FhirBase64Binary;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Signature)other;
        return AreListsEqual(Type, o.Type)
               && ValueEquals(When, o.When)
               && ValueEquals(Who, o.Who)
               && ValueEquals(OnBehalfOf, o.OnBehalfOf)
               && ValueEquals(TargetFormat, o.TargetFormat)
               && ValueEquals(SigFormat, o.SigFormat)
               && ValueEquals(Data, o.Data);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateList(Type, validationContext)) yield return r;
        foreach (var r in ValidateItem(When, validationContext)) yield return r;
        foreach (var r in ValidateItem(Who, validationContext)) yield return r;
        foreach (var r in ValidateItem(OnBehalfOf, validationContext)) yield return r;
        foreach (var r in ValidateItem(TargetFormat, validationContext)) yield return r;
        foreach (var r in ValidateItem(SigFormat, validationContext)) yield return r;
        foreach (var r in ValidateItem(Data, validationContext)) yield return r;
    }
}
