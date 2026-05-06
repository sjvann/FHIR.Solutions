using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype TriggerDefinition。</summary>
public class TriggerDefinition : ComplexTypeBase
{
    [JsonPropertyName("type")] public FhirCode? Type { get; set; }
    [JsonPropertyName("name")] public FhirString? Name { get; set; }
    [JsonPropertyName("data")] public List<DataRequirement>? Data { get; set; }
    [JsonPropertyName("condition")] public Expression? Condition { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (TriggerDefinition)copy;
        c.Type = Type?.DeepCopy() as FhirCode;
        c.Name = Name?.DeepCopy() as FhirString;
        c.Data = DeepCopyList(Data);
        c.Condition = Condition?.DeepCopy() as Expression;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (TriggerDefinition)other;
        return ValueEquals(Type, o.Type)
               && ValueEquals(Name, o.Name)
               && AreListsEqual(Data, o.Data)
               && ValueEquals(Condition, o.Condition);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Type, validationContext)) yield return r;
        foreach (var r in ValidateItem(Name, validationContext)) yield return r;
        foreach (var r in ValidateList(Data, validationContext)) yield return r;
        foreach (var r in ValidateItem(Condition, validationContext)) yield return r;
    }
}
