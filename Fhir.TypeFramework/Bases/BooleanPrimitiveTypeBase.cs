using System.Text.Json.Serialization;

namespace Fhir.TypeFramework.Bases;

public abstract class BooleanPrimitiveTypeBase : PrimitiveTypeBase<bool>
{
    protected BooleanPrimitiveTypeBase() { }
    protected BooleanPrimitiveTypeBase(bool value) : base(value) { }
    protected BooleanPrimitiveTypeBase(string? value) : base(value) { }

    public override object? ParseValue(string value)
    {
        if (bool.TryParse(value, out var b)) return b;
        if (value == "1") return true;
        if (value == "0") return false;
        return null;
    }

    public override string? ValueToString(object? value)
        => value?.ToString()?.ToLowerInvariant();

    public override bool IsValidValue(object? value)
        => value is null || value is bool;

    protected override bool ParseTypedValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        if (bool.TryParse(value, out var b)) return b;
        if (value == "1") return true;
        if (value == "0") return false;
        return default;
    }

    protected override string? ConvertToStringValue(bool value)
        => value ? "true" : "false";

    protected static T? CreateFromBoolean<T>(bool? value) where T : BooleanPrimitiveTypeBase, new()
    {
        if (value is null) return null;
        var t = new T { Value = value.Value };
        return t;
    }

    protected static bool? GetBooleanValue(BooleanPrimitiveTypeBase? instance) => instance is null ? null : instance.Value;
}

