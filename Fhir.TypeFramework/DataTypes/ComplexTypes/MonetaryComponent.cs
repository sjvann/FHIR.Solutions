using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype MonetaryComponent。</summary>
public class MonetaryComponent : ComplexTypeBase
{
    [JsonPropertyName("type")] public CodeableConcept? Type { get; set; }
    [JsonPropertyName("amount")] public Money? Amount { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (MonetaryComponent)copy;
        c.Type = Type?.DeepCopy() as CodeableConcept;
        c.Amount = Amount?.DeepCopy() as Money;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (MonetaryComponent)other;
        return ValueEquals(Type, o.Type) && ValueEquals(Amount, o.Amount);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Type, validationContext)) yield return r;
        foreach (var r in ValidateItem(Amount, validationContext)) yield return r;
    }
}
