using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype ContactDetail。</summary>
public class ContactDetail : ComplexTypeBase
{
    [JsonPropertyName("name")] public FhirString? Name { get; set; }
    [JsonPropertyName("telecom")] public List<ContactPoint>? Telecom { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (ContactDetail)copy;
        c.Name = Name?.DeepCopy() as FhirString;
        c.Telecom = DeepCopyList(Telecom);
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (ContactDetail)other;
        return ValueEquals(Name, o.Name) && AreListsEqual(Telecom, o.Telecom);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Name, validationContext)) yield return r;
        foreach (var r in ValidateList(Telecom, validationContext)) yield return r;
    }
}
