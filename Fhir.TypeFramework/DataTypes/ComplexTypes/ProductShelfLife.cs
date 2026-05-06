using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype ProductShelfLife。</summary>
public class ProductShelfLife : ComplexTypeBase
{
    [JsonPropertyName("type")] public CodeableConcept? Type { get; set; }
    [JsonPropertyName("period")] public Duration? Period { get; set; }
    [JsonPropertyName("periodString")] public FhirString? PeriodString { get; set; }
    [JsonPropertyName("specialPrecautionsForStorage")] public List<Coding>? SpecialPrecautionsForStorage { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (ProductShelfLife)copy;
        c.Type = Type?.DeepCopy() as CodeableConcept;
        c.Period = Period?.DeepCopy() as Duration;
        c.PeriodString = PeriodString?.DeepCopy() as FhirString;
        c.SpecialPrecautionsForStorage = DeepCopyList(SpecialPrecautionsForStorage);
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (ProductShelfLife)other;
        return ValueEquals(Type, o.Type)
               && ValueEquals(Period, o.Period)
               && ValueEquals(PeriodString, o.PeriodString)
               && AreListsEqual(SpecialPrecautionsForStorage, o.SpecialPrecautionsForStorage);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Type, validationContext)) yield return r;
        foreach (var r in ValidateItem(Period, validationContext)) yield return r;
        foreach (var r in ValidateItem(PeriodString, validationContext)) yield return r;
        foreach (var r in ValidateList(SpecialPrecautionsForStorage, validationContext)) yield return r;
    }
}
