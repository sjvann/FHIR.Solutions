using System.Text.Json.Serialization;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.Bases;

public abstract class StringPrimitiveTypeBase : PrimitiveTypeBase<string>
{
    protected StringPrimitiveTypeBase() { }
    protected StringPrimitiveTypeBase(string? value) : base(value) { }

    [JsonIgnore]
    public new string? Value
    {
        get => base.Value;
        set => base.Value = value;
    }

    public override object? ParseValue(string value) => value;

    public override string? ValueToString(object? value) => value?.ToString();

    public override bool IsValidValue(object? value)
        => value is null || (value is string s && ValidateStringValue(s) && ValidationFramework.ValidateStringByteSize(s, 1024 * 1024));

    protected override string? ParseTypedValue(string? value) => value;
    protected override string? ConvertToStringValue(string? value) => value;

    protected abstract bool ValidateStringValue(string value);

    protected static T? CreateFromString<T>(string? value) where T : StringPrimitiveTypeBase, new()
    {
        if (value is null) return null;
        var t = new T { StringValue = value };
        return t;
    }

    protected static string? GetStringValue(StringPrimitiveTypeBase? instance) => instance?.StringValue;
}

