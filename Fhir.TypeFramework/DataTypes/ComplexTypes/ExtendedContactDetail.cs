using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype ExtendedContactDetail。</summary>
public class ExtendedContactDetail : ComplexTypeBase
{
    [JsonPropertyName("purpose")] public CodeableConcept? Purpose { get; set; }
    [JsonPropertyName("name")] public List<HumanName>? Name { get; set; }
    [JsonPropertyName("telecom")] public List<ContactPoint>? Telecom { get; set; }
    [JsonPropertyName("address")] public Address? Address { get; set; }
    [JsonPropertyName("organization")] public Reference? Organization { get; set; }
    [JsonPropertyName("period")] public Period? Period { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (ExtendedContactDetail)copy;
        c.Purpose = Purpose?.DeepCopy() as CodeableConcept;
        c.Name = DeepCopyList(Name);
        c.Telecom = DeepCopyList(Telecom);
        c.Address = Address?.DeepCopy() as Address;
        c.Organization = Organization?.DeepCopy() as Reference;
        c.Period = Period?.DeepCopy() as Period;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (ExtendedContactDetail)other;
        return ValueEquals(Purpose, o.Purpose)
               && AreListsEqual(Name, o.Name)
               && AreListsEqual(Telecom, o.Telecom)
               && ValueEquals(Address, o.Address)
               && ValueEquals(Organization, o.Organization)
               && ValueEquals(Period, o.Period);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Purpose, validationContext)) yield return r;
        foreach (var r in ValidateList(Name, validationContext)) yield return r;
        foreach (var r in ValidateList(Telecom, validationContext)) yield return r;
        foreach (var r in ValidateItem(Address, validationContext)) yield return r;
        foreach (var r in ValidateItem(Organization, validationContext)) yield return r;
        foreach (var r in ValidateItem(Period, validationContext)) yield return r;
    }
}
