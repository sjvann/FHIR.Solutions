using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype ProdCharacteristic。</summary>
public class ProdCharacteristic : ComplexTypeBase
{
    [JsonPropertyName("type")] public CodeableConcept? Type { get; set; }
    [JsonPropertyName("value")] public List<Quantity>? Value { get; set; }
    [JsonPropertyName("valueCodeableConcept")] public List<CodeableConcept>? ValueCodeableConcept { get; set; }
    [JsonPropertyName("valueAttachment")] public List<Attachment>? ValueAttachment { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (ProdCharacteristic)copy;
        c.Type = Type?.DeepCopy() as CodeableConcept;
        c.Value = DeepCopyList(Value);
        c.ValueCodeableConcept = DeepCopyList(ValueCodeableConcept);
        c.ValueAttachment = DeepCopyList(ValueAttachment);
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (ProdCharacteristic)other;
        return ValueEquals(Type, o.Type)
               && AreListsEqual(Value, o.Value)
               && AreListsEqual(ValueCodeableConcept, o.ValueCodeableConcept)
               && AreListsEqual(ValueAttachment, o.ValueAttachment);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Type, validationContext)) yield return r;
        foreach (var r in ValidateList(Value, validationContext)) yield return r;
        foreach (var r in ValidateList(ValueCodeableConcept, validationContext)) yield return r;
        foreach (var r in ValidateList(ValueAttachment, validationContext)) yield return r;
    }
}
