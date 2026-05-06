using System.Globalization;
using System.Text.Json.Serialization;

namespace Fhir.TypeFramework.Bases;

public abstract class DateTimePrimitiveTypeBase<TDateTime> : PrimitiveTypeBase<TDateTime>
    where TDateTime : struct, IParsable<TDateTime>, IComparable<TDateTime>, IEquatable<TDateTime>
{
    protected DateTimePrimitiveTypeBase() { }
    protected DateTimePrimitiveTypeBase(TDateTime value) : base(value) { }
    protected DateTimePrimitiveTypeBase(string? value) : base(value) { }

    public override object? ParseValue(string value)
        => TDateTime.TryParse(value, CultureInfo.InvariantCulture, out var parsed) ? parsed : null;

    public override string? ValueToString(object? value)
        => value?.ToString();

    public override bool IsValidValue(object? value)
        => value is null || (value is TDateTime dt && ValidateDateTimeValue(dt));

    protected override TDateTime ParseTypedValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        return TDateTime.TryParse(value, CultureInfo.InvariantCulture, out var parsed) ? parsed : default;
    }

    protected override string? ConvertToStringValue(TDateTime value)
        => value.ToString();

    protected abstract bool ValidateDateTimeValue(TDateTime value);

    protected static T? CreateFromDateTime<T>(TDateTime? value) where T : DateTimePrimitiveTypeBase<TDateTime>, new()
    {
        if (value is null) return null;
        var t = new T { Value = value.Value };
        return t;
    }

    protected static TDateTime? GetDateTimeValue(DateTimePrimitiveTypeBase<TDateTime>? instance) => instance is null ? null : instance.Value;
}

