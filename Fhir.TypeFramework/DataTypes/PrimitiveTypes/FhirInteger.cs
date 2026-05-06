using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Interface;

namespace Fhir.TypeFramework.DataTypes;

public class FhirInteger : NumericPrimitiveTypeBase<int>, IInteger32Value
{
    public FhirInteger() { }
    public FhirInteger(int v) : base(v) { }
    public FhirInteger(string? v) : base(v) { }

    public static implicit operator FhirInteger?(int? value) => CreateFromNumber<FhirInteger>(value);
    public static implicit operator int?(FhirInteger? instance) => GetNumericValue(instance);
    public static implicit operator FhirInteger?(string? value) => value is null ? null : new FhirInteger(value);
    public static implicit operator string?(FhirInteger? instance) => instance?.StringValue;

    protected override bool ValidateNumericValue(int value) => true;

    int? IValue<int?>.Value => HasValue ? Value : null;
    public bool HasValue => !IsNull;
}

