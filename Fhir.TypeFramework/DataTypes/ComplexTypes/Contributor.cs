using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Contributor。</summary>
public class Contributor : ComplexTypeBase
{
    [JsonPropertyName("type")] public FhirCode? Type { get; set; }
    [JsonPropertyName("name")] public FhirString? Name { get; set; }
    [JsonPropertyName("contact")] public List<ContactDetail>? Contact { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Contributor)copy;
        c.Type = Type?.DeepCopy() as FhirCode;
        c.Name = Name?.DeepCopy() as FhirString;
        c.Contact = DeepCopyList(Contact);
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Contributor)other;
        return ValueEquals(Type, o.Type) && ValueEquals(Name, o.Name) && AreListsEqual(Contact, o.Contact);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Type, validationContext)) yield return r;
        foreach (var r in ValidateItem(Name, validationContext)) yield return r;
        foreach (var r in ValidateList(Contact, validationContext)) yield return r;
    }
}
