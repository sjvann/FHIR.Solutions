using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Address。</summary>
public class Address : ComplexTypeBase
{
    [JsonPropertyName("use")] public FhirCode? Use { get; set; }
    [JsonPropertyName("type")] public FhirCode? Type { get; set; }
    [JsonPropertyName("text")] public FhirString? Text { get; set; }
    [JsonPropertyName("line")] public List<FhirString>? Line { get; set; }
    [JsonPropertyName("city")] public FhirString? City { get; set; }
    [JsonPropertyName("district")] public FhirString? District { get; set; }
    [JsonPropertyName("state")] public FhirString? State { get; set; }
    [JsonPropertyName("postalCode")] public FhirString? PostalCode { get; set; }
    [JsonPropertyName("country")] public FhirString? Country { get; set; }
    [JsonPropertyName("period")] public Period? Period { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Address)copy;
        c.Use = Use?.DeepCopy() as FhirCode;
        c.Type = Type?.DeepCopy() as FhirCode;
        c.Text = Text?.DeepCopy() as FhirString;
        c.Line = DeepCopyList(Line);
        c.City = City?.DeepCopy() as FhirString;
        c.District = District?.DeepCopy() as FhirString;
        c.State = State?.DeepCopy() as FhirString;
        c.PostalCode = PostalCode?.DeepCopy() as FhirString;
        c.Country = Country?.DeepCopy() as FhirString;
        c.Period = Period?.DeepCopy() as Period;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Address)other;
        return ValueEquals(Use, o.Use)
               && ValueEquals(Type, o.Type)
               && ValueEquals(Text, o.Text)
               && AreListsEqual(Line, o.Line)
               && ValueEquals(City, o.City)
               && ValueEquals(District, o.District)
               && ValueEquals(State, o.State)
               && ValueEquals(PostalCode, o.PostalCode)
               && ValueEquals(Country, o.Country)
               && ValueEquals(Period, o.Period);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Use, validationContext)) yield return r;
        foreach (var r in ValidateItem(Type, validationContext)) yield return r;
        foreach (var r in ValidateItem(Text, validationContext)) yield return r;
        foreach (var r in ValidateList(Line, validationContext)) yield return r;
        foreach (var r in ValidateItem(City, validationContext)) yield return r;
        foreach (var r in ValidateItem(District, validationContext)) yield return r;
        foreach (var r in ValidateItem(State, validationContext)) yield return r;
        foreach (var r in ValidateItem(PostalCode, validationContext)) yield return r;
        foreach (var r in ValidateItem(Country, validationContext)) yield return r;
        foreach (var r in ValidateItem(Period, validationContext)) yield return r;
    }
}
