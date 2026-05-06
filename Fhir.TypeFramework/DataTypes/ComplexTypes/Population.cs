using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Population。</summary>
public class Population : ComplexTypeBase
{
    [JsonPropertyName("ageRange")] public List<Range>? AgeRange { get; set; }
    [JsonPropertyName("age")] public List<BackboneElement>? Age { get; set; }
    [JsonPropertyName("gender")] public List<CodeableConcept>? Gender { get; set; }
    [JsonPropertyName("race")] public List<CodeableConcept>? Race { get; set; }
    [JsonPropertyName("physiologicalCondition")] public List<CodeableConcept>? PhysiologicalCondition { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Population)copy;
        c.AgeRange = DeepCopyList(AgeRange);
        c.Age = DeepCopyList(Age);
        c.Gender = DeepCopyList(Gender);
        c.Race = DeepCopyList(Race);
        c.PhysiologicalCondition = DeepCopyList(PhysiologicalCondition);
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Population)other;
        return AreListsEqual(AgeRange, o.AgeRange)
               && AreListsEqual(Age, o.Age)
               && AreListsEqual(Gender, o.Gender)
               && AreListsEqual(Race, o.Race)
               && AreListsEqual(PhysiologicalCondition, o.PhysiologicalCondition);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateList(AgeRange, validationContext)) yield return r;
        foreach (var r in ValidateList(Age, validationContext)) yield return r;
        foreach (var r in ValidateList(Gender, validationContext)) yield return r;
        foreach (var r in ValidateList(Race, validationContext)) yield return r;
        foreach (var r in ValidateList(PhysiologicalCondition, validationContext)) yield return r;
    }
}
