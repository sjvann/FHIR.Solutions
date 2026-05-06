using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype UsageContext。</summary>
public class UsageContext : ComplexTypeBase
{
    [JsonPropertyName("code")] public Coding? Code { get; set; }
    [JsonPropertyName("valueCodeableConcept")] public CodeableConcept? ValueCodeableConcept { get; set; }
    [JsonPropertyName("valueQuantity")] public Quantity? ValueQuantity { get; set; }
    [JsonPropertyName("valueRange")] public Range? ValueRange { get; set; }
    [JsonPropertyName("valueReference")] public Reference? ValueReference { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (UsageContext)copy;
        c.Code = Code?.DeepCopy() as Coding;
        c.ValueCodeableConcept = ValueCodeableConcept?.DeepCopy() as CodeableConcept;
        c.ValueQuantity = ValueQuantity?.DeepCopy() as Quantity;
        c.ValueRange = ValueRange?.DeepCopy() as Range;
        c.ValueReference = ValueReference?.DeepCopy() as Reference;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (UsageContext)other;
        return ValueEquals(Code, o.Code)
               && ValueEquals(ValueCodeableConcept, o.ValueCodeableConcept)
               && ValueEquals(ValueQuantity, o.ValueQuantity)
               && ValueEquals(ValueRange, o.ValueRange)
               && ValueEquals(ValueReference, o.ValueReference);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Code, validationContext)) yield return r;
        foreach (var r in ValidateItem(ValueCodeableConcept, validationContext)) yield return r;
        foreach (var r in ValidateItem(ValueQuantity, validationContext)) yield return r;
        foreach (var r in ValidateItem(ValueRange, validationContext)) yield return r;
        foreach (var r in ValidateItem(ValueReference, validationContext)) yield return r;
    }
}
