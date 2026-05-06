using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype MarketingStatus。</summary>
public class MarketingStatus : ComplexTypeBase
{
    [JsonPropertyName("country")] public CodeableConcept? Country { get; set; }
    [JsonPropertyName("jurisdiction")] public List<CodeableConcept>? Jurisdiction { get; set; }
    [JsonPropertyName("status")] public CodeableConcept? Status { get; set; }
    [JsonPropertyName("dateRange")] public Period? DateRange { get; set; }
    [JsonPropertyName("restoreDate")] public FhirDateTime? RestoreDate { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (MarketingStatus)copy;
        c.Country = Country?.DeepCopy() as CodeableConcept;
        c.Jurisdiction = DeepCopyList(Jurisdiction);
        c.Status = Status?.DeepCopy() as CodeableConcept;
        c.DateRange = DateRange?.DeepCopy() as Period;
        c.RestoreDate = RestoreDate?.DeepCopy() as FhirDateTime;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (MarketingStatus)other;
        return ValueEquals(Country, o.Country)
               && AreListsEqual(Jurisdiction, o.Jurisdiction)
               && ValueEquals(Status, o.Status)
               && ValueEquals(DateRange, o.DateRange)
               && ValueEquals(RestoreDate, o.RestoreDate);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Country, validationContext)) yield return r;
        foreach (var r in ValidateList(Jurisdiction, validationContext)) yield return r;
        foreach (var r in ValidateItem(Status, validationContext)) yield return r;
        foreach (var r in ValidateItem(DateRange, validationContext)) yield return r;
        foreach (var r in ValidateItem(RestoreDate, validationContext)) yield return r;
    }
}
