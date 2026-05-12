using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>
/// FHIR R4 datatype SubstanceAmount（用於 SubstancePolymer 等；R5 已改為其他元素形狀）。
/// </summary>
public sealed class SubstanceAmount : BackboneElement
{
    [JsonPropertyName("amountQuantity")] public Quantity? AmountQuantity { get; set; }

    [JsonPropertyName("amountRange")]
    public global::Fhir.TypeFramework.DataTypes.Range? AmountRange { get; set; }

    [JsonPropertyName("amountString")] public FhirString? AmountString { get; set; }

    [JsonPropertyName("amountType")] public CodeableConcept? AmountType { get; set; }

    [JsonPropertyName("amountText")] public FhirString? AmountText { get; set; }

    [JsonPropertyName("referenceRange")] public ReferenceRangeComponent? ReferenceRange { get; set; }

    public sealed class ReferenceRangeComponent : BackboneElement
    {
        [JsonPropertyName("lowLimit")] public Quantity? LowLimit { get; set; }

        [JsonPropertyName("highLimit")] public Quantity? HighLimit { get; set; }

        public override Base DeepCopy()
        {
            var copy = (ReferenceRangeComponent)base.DeepCopy();
            copy.LowLimit = LowLimit?.DeepCopy() as Quantity;
            copy.HighLimit = HighLimit?.DeepCopy() as Quantity;
            return copy;
        }

        public override bool IsExactly(Base other)
        {
            if (other is not ReferenceRangeComponent o) return false;
            if (!base.IsExactly(other)) return false;
            return ValueEq(LowLimit, o.LowLimit) && ValueEq(HighLimit, o.HighLimit);
        }

        private static bool ValueEq<T>(T? a, T? b) where T : Base =>
            (a == null && b == null) || (a != null && b != null && a.IsExactly(b));
    }

    public override Base DeepCopy()
    {
        var copy = (SubstanceAmount)base.DeepCopy();
        copy.AmountQuantity = AmountQuantity?.DeepCopy() as Quantity;
        copy.AmountRange = AmountRange?.DeepCopy() as global::Fhir.TypeFramework.DataTypes.Range;
        copy.AmountString = AmountString?.DeepCopy() as FhirString;
        copy.AmountType = AmountType?.DeepCopy() as CodeableConcept;
        copy.AmountText = AmountText?.DeepCopy() as FhirString;
        copy.ReferenceRange = ReferenceRange?.DeepCopy() as ReferenceRangeComponent;
        return copy;
    }

    public override bool IsExactly(Base other)
    {
        if (other is not SubstanceAmount o) return false;
        if (!base.IsExactly(other)) return false;
        if (!ValueEq(AmountQuantity, o.AmountQuantity)) return false;
        if (!ValueEq(AmountRange, o.AmountRange)) return false;
        if (!ValueEq(AmountString, o.AmountString)) return false;
        if (!ValueEq(AmountType, o.AmountType)) return false;
        if (!ValueEq(AmountText, o.AmountText)) return false;
        if (!ValueEq(ReferenceRange, o.ReferenceRange)) return false;
        return true;
    }

    private static bool ValueEq<T>(T? a, T? b) where T : Base =>
        (a == null && b == null) || (a != null && b != null && a.IsExactly(b));
}
