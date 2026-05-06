using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>
/// FHIR R5 Datatype ElementDefinition（含常用約束欄位；完整規格龐大，其餘欄位可後續擴充）。
/// </summary>
public class ElementDefinition : ComplexTypeBase
{
    [JsonPropertyName("path")] public FhirString? Path { get; set; }
    [JsonPropertyName("short")] public FhirString? Short { get; set; }
    [JsonPropertyName("definition")] public FhirString? Definition { get; set; }
    [JsonPropertyName("comment")] public FhirString? Comment { get; set; }
    [JsonPropertyName("min")] public FhirUnsignedInt? Min { get; set; }
    [JsonPropertyName("max")] public FhirString? Max { get; set; }
    [JsonPropertyName("type")] public List<ElementDefinitionTypeComponent>? Type { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (ElementDefinition)copy;
        c.Path = Path?.DeepCopy() as FhirString;
        c.Short = Short?.DeepCopy() as FhirString;
        c.Definition = Definition?.DeepCopy() as FhirString;
        c.Comment = Comment?.DeepCopy() as FhirString;
        c.Min = Min?.DeepCopy() as FhirUnsignedInt;
        c.Max = Max?.DeepCopy() as FhirString;
        c.Type = Type?.Select(t => (ElementDefinitionTypeComponent)t.DeepCopy()).ToList();
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (ElementDefinition)other;
        if (!ValueEquals(Path, o.Path)) return false;
        if (!ValueEquals(Short, o.Short)) return false;
        if (!ValueEquals(Definition, o.Definition)) return false;
        if (!ValueEquals(Comment, o.Comment)) return false;
        if (!ValueEquals(Min, o.Min)) return false;
        if (!ValueEquals(Max, o.Max)) return false;
        if (Type == null && o.Type == null) return true;
        if (Type == null || o.Type == null || Type.Count != o.Type.Count) return false;
        return Type.Zip(o.Type, (a, b) => a.IsExactly(b)).All(z => z);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(Path, validationContext)) yield return r;
        foreach (var r in ValidateItem(Short, validationContext)) yield return r;
        foreach (var r in ValidateItem(Definition, validationContext)) yield return r;
        foreach (var r in ValidateItem(Comment, validationContext)) yield return r;
        foreach (var r in ValidateItem(Min, validationContext)) yield return r;
        foreach (var r in ValidateItem(Max, validationContext)) yield return r;
        if (Type != null)
        {
            foreach (var t in Type)
            {
                foreach (var r in t.Validate(new ValidationContext(t))) yield return r;
            }
        }
    }
}
