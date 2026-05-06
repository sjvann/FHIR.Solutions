using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Dosage。</summary>
public class Dosage : ComplexTypeBase
{
    [JsonPropertyName("sequence")] public FhirInteger? Sequence { get; set; }
    [JsonPropertyName("text")] public FhirString? Text { get; set; }
    [JsonPropertyName("additionalInstruction")] public List<CodeableConcept>? AdditionalInstruction { get; set; }
    [JsonPropertyName("patientInstruction")] public FhirString? PatientInstruction { get; set; }
    [JsonPropertyName("timing")] public Timing? Timing { get; set; }
    [JsonPropertyName("asNeededBoolean")] public FhirBoolean? AsNeededBoolean { get; set; }
    [JsonPropertyName("asNeededCodeableConcept")] public CodeableConcept? AsNeededCodeableConcept { get; set; }
    [JsonPropertyName("site")] public CodeableConcept? Site { get; set; }
    [JsonPropertyName("route")] public CodeableConcept? Route { get; set; }
    [JsonPropertyName("method")] public CodeableConcept? Method { get; set; }
    [JsonPropertyName("doseAndRate")] public List<BackboneElement>? DoseAndRate { get; set; }
    [JsonPropertyName("maxDosePerPeriod")] public List<BackboneElement>? MaxDosePerPeriod { get; set; }
    [JsonPropertyName("maxDosePerAdministration")] public Quantity? MaxDosePerAdministration { get; set; }
    [JsonPropertyName("maxDosePerLifetime")] public Quantity? MaxDosePerLifetime { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Dosage)copy;
        c.Sequence = Sequence?.DeepCopy() as FhirInteger;
        c.Text = Text?.DeepCopy() as FhirString;
        c.AdditionalInstruction = DeepCopyList(AdditionalInstruction);
        c.PatientInstruction = PatientInstruction?.DeepCopy() as FhirString;
        c.Timing = Timing?.DeepCopy() as Timing;
        c.AsNeededBoolean = AsNeededBoolean?.DeepCopy() as FhirBoolean;
        c.AsNeededCodeableConcept = AsNeededCodeableConcept?.DeepCopy() as CodeableConcept;
        c.Site = Site?.DeepCopy() as CodeableConcept;
        c.Route = Route?.DeepCopy() as CodeableConcept;
        c.Method = Method?.DeepCopy() as CodeableConcept;
        c.DoseAndRate = DeepCopyList(DoseAndRate);
        c.MaxDosePerPeriod = DeepCopyList(MaxDosePerPeriod);
        c.MaxDosePerAdministration = MaxDosePerAdministration?.DeepCopy() as Quantity;
        c.MaxDosePerLifetime = MaxDosePerLifetime?.DeepCopy() as Quantity;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Dosage)other;
        return ValueEquals(Sequence, o.Sequence)
               && ValueEquals(Text, o.Text)
               && AreListsEqual(AdditionalInstruction, o.AdditionalInstruction)
               && ValueEquals(PatientInstruction, o.PatientInstruction)
               && ValueEquals(Timing, o.Timing)
               && ValueEquals(AsNeededBoolean, o.AsNeededBoolean)
               && ValueEquals(AsNeededCodeableConcept, o.AsNeededCodeableConcept)
               && ValueEquals(Site, o.Site)
               && ValueEquals(Route, o.Route)
               && ValueEquals(Method, o.Method)
               && AreListsEqual(DoseAndRate, o.DoseAndRate)
               && AreListsEqual(MaxDosePerPeriod, o.MaxDosePerPeriod)
               && ValueEquals(MaxDosePerAdministration, o.MaxDosePerAdministration)
               && ValueEquals(MaxDosePerLifetime, o.MaxDosePerLifetime);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Sequence, validationContext)) yield return r;
        foreach (var r in ValidateItem(Text, validationContext)) yield return r;
        foreach (var r in ValidateList(AdditionalInstruction, validationContext)) yield return r;
        foreach (var r in ValidateItem(PatientInstruction, validationContext)) yield return r;
        foreach (var r in ValidateItem(Timing, validationContext)) yield return r;
        foreach (var r in ValidateItem(AsNeededBoolean, validationContext)) yield return r;
        foreach (var r in ValidateItem(AsNeededCodeableConcept, validationContext)) yield return r;
        foreach (var r in ValidateItem(Site, validationContext)) yield return r;
        foreach (var r in ValidateItem(Route, validationContext)) yield return r;
        foreach (var r in ValidateItem(Method, validationContext)) yield return r;
        foreach (var r in ValidateList(DoseAndRate, validationContext)) yield return r;
        foreach (var r in ValidateList(MaxDosePerPeriod, validationContext)) yield return r;
        foreach (var r in ValidateItem(MaxDosePerAdministration, validationContext)) yield return r;
        foreach (var r in ValidateItem(MaxDosePerLifetime, validationContext)) yield return r;
    }
}
