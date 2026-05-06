using System.Globalization;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.Bases;

public abstract class NumericPrimitiveTypeBase<TNumeric> : PrimitiveTypeBase<TNumeric>
    where TNumeric : struct, IParsable<TNumeric>, IComparable<TNumeric>, IEquatable<TNumeric>
{
    protected NumericPrimitiveTypeBase() { }
    protected NumericPrimitiveTypeBase(TNumeric value) : base(value) { }
    protected NumericPrimitiveTypeBase(string? value) : base(value) { }

    public override object? ParseValue(string value)
        => TNumeric.TryParse(value, CultureInfo.InvariantCulture, out var parsed) ? parsed : null;

    public override string? ValueToString(object? value)
        => value?.ToString();

    public override bool IsValidValue(object? value)
        => value is null || (value is TNumeric n && ValidateNumericValue(n));

    protected override TNumeric ParseTypedValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        return TNumeric.TryParse(value, CultureInfo.InvariantCulture, out var parsed) ? parsed : default;
    }

    protected override string? ConvertToStringValue(TNumeric value)
        => value.ToString();

    protected abstract bool ValidateNumericValue(TNumeric value);

    protected static T? CreateFromNumber<T>(TNumeric? value) where T : NumericPrimitiveTypeBase<TNumeric>, new()
    {
        if (value is null) return null;
        var t = new T { Value = value.Value };
        return t;
    }

    protected static TNumeric? GetNumericValue(NumericPrimitiveTypeBase<TNumeric>? instance) => instance is null ? null : instance.Value;
}

