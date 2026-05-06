using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype SampledData。</summary>
/// <remarks>
/// R5 使用 <c>interval</c> + <c>intervalUnit</c>（取代早期僅 <c>period</c> decimal 之表述）；<c>origin</c> 型別為 SimpleQuantity。
/// </remarks>
public class SampledData : ComplexTypeBase
{
    [JsonPropertyName("origin")] public SimpleQuantity? Origin { get; set; }
    [JsonPropertyName("interval")] public FhirDecimal? Interval { get; set; }
    [JsonPropertyName("intervalUnit")] public FhirCode? IntervalUnit { get; set; }
    [JsonPropertyName("factor")] public FhirDecimal? Factor { get; set; }
    [JsonPropertyName("lowerLimit")] public FhirDecimal? LowerLimit { get; set; }
    [JsonPropertyName("upperLimit")] public FhirDecimal? UpperLimit { get; set; }
    [JsonPropertyName("dimensions")] public FhirPositiveInt? Dimensions { get; set; }
    [JsonPropertyName("codeMap")] public FhirCanonical? CodeMap { get; set; }
    [JsonPropertyName("offsets")] public FhirString? Offsets { get; set; }
    [JsonPropertyName("data")] public FhirString? Data { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (SampledData)copy;
        c.Origin = Origin?.DeepCopy() as SimpleQuantity;
        c.Interval = Interval?.DeepCopy() as FhirDecimal;
        c.IntervalUnit = IntervalUnit?.DeepCopy() as FhirCode;
        c.Factor = Factor?.DeepCopy() as FhirDecimal;
        c.LowerLimit = LowerLimit?.DeepCopy() as FhirDecimal;
        c.UpperLimit = UpperLimit?.DeepCopy() as FhirDecimal;
        c.Dimensions = Dimensions?.DeepCopy() as FhirPositiveInt;
        c.CodeMap = CodeMap?.DeepCopy() as FhirCanonical;
        c.Offsets = Offsets?.DeepCopy() as FhirString;
        c.Data = Data?.DeepCopy() as FhirString;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (SampledData)other;
        return ValueEquals(Origin, o.Origin)
               && ValueEquals(Interval, o.Interval)
               && ValueEquals(IntervalUnit, o.IntervalUnit)
               && ValueEquals(Factor, o.Factor)
               && ValueEquals(LowerLimit, o.LowerLimit)
               && ValueEquals(UpperLimit, o.UpperLimit)
               && ValueEquals(Dimensions, o.Dimensions)
               && ValueEquals(CodeMap, o.CodeMap)
               && ValueEquals(Offsets, o.Offsets)
               && ValueEquals(Data, o.Data);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Origin, validationContext)) yield return r;
        foreach (var r in ValidateItem(Interval, validationContext)) yield return r;
        foreach (var r in ValidateItem(IntervalUnit, validationContext)) yield return r;
        foreach (var r in ValidateItem(Factor, validationContext)) yield return r;
        foreach (var r in ValidateItem(LowerLimit, validationContext)) yield return r;
        foreach (var r in ValidateItem(UpperLimit, validationContext)) yield return r;
        foreach (var r in ValidateItem(Dimensions, validationContext)) yield return r;
        foreach (var r in ValidateItem(CodeMap, validationContext)) yield return r;
        foreach (var r in ValidateItem(Offsets, validationContext)) yield return r;
        foreach (var r in ValidateItem(Data, validationContext)) yield return r;
    }
}
