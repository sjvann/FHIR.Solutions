using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Timing。</summary>
public class Timing : ComplexTypeBase
{
    [JsonPropertyName("event")] public List<FhirDateTime>? Event { get; set; }
    [JsonPropertyName("repeat")] public BackboneElement? Repeat { get; set; }
    [JsonPropertyName("code")] public CodeableConcept? Code { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Timing)copy;
        c.Event = DeepCopyList(Event);
        c.Repeat = Repeat?.DeepCopy() as BackboneElement;
        c.Code = Code?.DeepCopy() as CodeableConcept;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Timing)other;
        return AreListsEqual(Event, o.Event)
               && (Repeat == null && o.Repeat == null
                   || Repeat != null && o.Repeat != null && Repeat.IsExactly(o.Repeat))
               && ValueEquals(Code, o.Code);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateList(Event, validationContext)) yield return r;
        if (Repeat != null)
        {
            foreach (var r in Repeat.Validate(new ValidationContext(Repeat))) yield return r;
        }

        foreach (var r in ValidateItem(Code, validationContext)) yield return r;
    }
}
