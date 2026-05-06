using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Availability。</summary>
public class Availability : ComplexTypeBase
{
    [JsonPropertyName("availableTime")] public List<AvailabilityAvailableTime>? AvailableTime { get; set; }
    [JsonPropertyName("notAvailableTime")] public List<AvailabilityNotAvailableTime>? NotAvailableTime { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Availability)copy;
        c.AvailableTime = AvailableTime?.Select(x => (AvailabilityAvailableTime)x.DeepCopy()).ToList();
        c.NotAvailableTime = NotAvailableTime?.Select(x => (AvailabilityNotAvailableTime)x.DeepCopy()).ToList();
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Availability)other;
        if (AvailableTime == null && o.AvailableTime == null) { /* ok */ }
        else if (AvailableTime == null || o.AvailableTime == null || AvailableTime.Count != o.AvailableTime.Count)
            return false;
        else if (!AvailableTime.Zip(o.AvailableTime, (a, b) => a.IsExactly(b)).All(z => z))
            return false;

        if (NotAvailableTime == null && o.NotAvailableTime == null) return true;
        if (NotAvailableTime == null || o.NotAvailableTime == null || NotAvailableTime.Count != o.NotAvailableTime.Count)
            return false;
        return NotAvailableTime.Zip(o.NotAvailableTime, (a, b) => a.IsExactly(b)).All(z => z);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        if (AvailableTime != null)
        {
            foreach (var item in AvailableTime)
            {
                foreach (var r in item.Validate(new ValidationContext(item))) yield return r;
            }
        }

        if (NotAvailableTime != null)
        {
            foreach (var item in NotAvailableTime)
            {
                foreach (var r in item.Validate(new ValidationContext(item))) yield return r;
            }
        }
    }
}

/// <summary>FHIR R5 Availability.availableTime 切片。</summary>
public sealed class AvailabilityAvailableTime : BackboneElement
{
    [JsonPropertyName("daysOfWeek")] public List<FhirCode>? DaysOfWeek { get; set; }
    [JsonPropertyName("allDay")] public FhirBoolean? AllDay { get; set; }
    [JsonPropertyName("availableStartTime")] public FhirTime? AvailableStartTime { get; set; }
    [JsonPropertyName("availableEndTime")] public FhirTime? AvailableEndTime { get; set; }

    public override Base DeepCopy()
    {
        var copy = (AvailabilityAvailableTime)base.DeepCopy();
        copy.DaysOfWeek = DaysOfWeek?.Select(d => (d.DeepCopy() as FhirCode)!).ToList();
        copy.AllDay = AllDay?.DeepCopy() as FhirBoolean;
        copy.AvailableStartTime = AvailableStartTime?.DeepCopy() as FhirTime;
        copy.AvailableEndTime = AvailableEndTime?.DeepCopy() as FhirTime;
        return copy;
    }

    public override bool IsExactly(Base other)
    {
        if (other is not AvailabilityAvailableTime o) return false;
        if (!base.IsExactly(other)) return false;
        if (DaysOfWeek?.Count != o.DaysOfWeek?.Count) return false;
        if (DaysOfWeek != null && o.DaysOfWeek != null &&
            !DaysOfWeek.Zip(o.DaysOfWeek, (a, b) => a.IsExactly(b)).All(z => z))
            return false;
        return ValueEq(AllDay, o.AllDay)
               && ValueEq(AvailableStartTime, o.AvailableStartTime)
               && ValueEq(AvailableEndTime, o.AvailableEndTime);
    }

    private static bool ValueEq<T>(T? a, T? b) where T : Base =>
        (a == null && b == null) || (a != null && b != null && a.IsExactly(b));
}

/// <summary>FHIR R5 Availability.notAvailableTime 切片。</summary>
public sealed class AvailabilityNotAvailableTime : BackboneElement
{
    [JsonPropertyName("description")] public FhirString? Description { get; set; }
    [JsonPropertyName("during")] public Period? During { get; set; }

    public override Base DeepCopy()
    {
        var copy = (AvailabilityNotAvailableTime)base.DeepCopy();
        copy.Description = Description?.DeepCopy() as FhirString;
        copy.During = During?.DeepCopy() as Period;
        return copy;
    }

    public override bool IsExactly(Base other)
    {
        if (other is not AvailabilityNotAvailableTime o) return false;
        if (!base.IsExactly(other)) return false;
        return ValueEq(Description, o.Description) && ValueEq(During, o.During);
    }

    private static bool ValueEq<T>(T? a, T? b) where T : Base =>
        (a == null && b == null) || (a != null && b != null && a.IsExactly(b));
}
