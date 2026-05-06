using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Interface;

namespace Fhir.TypeFramework.DataTypes;

public class FhirInteger64 : NumericPrimitiveTypeBase<long>, IInteger64Value
{
    public FhirInteger64() { }
    public FhirInteger64(long v) : base(v) { }
    public FhirInteger64(string? v) : base(v) { }

    public static implicit operator FhirInteger64?(long? value) => CreateFromNumber<FhirInteger64>(value);
    public static implicit operator long?(FhirInteger64? instance) => GetNumericValue(instance);
    public static implicit operator FhirInteger64?(string? value) => value is null ? null : new FhirInteger64(value);
    public static implicit operator string?(FhirInteger64? instance) => instance?.StringValue;

    protected override bool ValidateNumericValue(long value) => true;

    long? IValue<long?>.Value => HasValue ? Value : null;
    public bool HasValue => !IsNull;
}

