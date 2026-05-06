using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using Fhir.TypeFramework.Abstractions;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;

namespace Fhir.TypeFramework.DataTypes;

public sealed class Extension : Element, IExtension
{
    [System.Text.Json.Serialization.JsonPropertyName("url")]
    public FhirString? Url { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public object? Value { get; set; }

    string? IExtension.Url => Url?.StringValue;

    public Extension()
    {
    }

    public Extension(JsonObject obj)
    {
        if (obj.TryGetPropertyValue("url", out var u) && u is JsonValue jv)
            Url = jv.GetValue<string>();
    }

    public override Base DeepCopy()
    {
        var c = new Extension
        {
            Id = Id?.DeepCopy() as FhirString,
            Url = Url?.DeepCopy() as FhirString,
            Value = Value
        };
        if (Extension is { Count: > 0 } ext)
            c.Extension = ext.Select(x => (IExtension)x.DeepCopy()).ToList();
        return c;
    }

    public override bool IsExactly(Base? other)
    {
        if (other is not Extension e)
            return false;
        if (!base.IsExactly(other))
            return false;
        return (Url == null && e.Url == null) || (Url != null && e.Url != null && Url.IsExactly(e.Url));
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Url?.StringValue))
            yield return new ValidationResult("Extension.url 為必要", [nameof(Url)]);
        foreach (var r in base.Validate(validationContext))
            yield return r;
    }

    public override JsonNode? GetJsonNode()
    {
        var o = new JsonObject();
        if (Url != null) o["url"] = JsonValue.Create(Url.StringValue);
        return o;
    }
}

