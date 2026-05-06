using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Interface;

namespace Fhir.TypeFramework.DataTypes;

public class FhirDecimal : NumericPrimitiveTypeBase<decimal>, IDecimalValue
{
    public FhirDecimal() { }
    public FhirDecimal(decimal v) : base(v) { }
    public FhirDecimal(string? v) : base(v) { }

    public static implicit operator FhirDecimal?(decimal? value) => CreateFromNumber<FhirDecimal>(value);
    public static implicit operator decimal?(FhirDecimal? instance) => GetNumericValue(instance);
    public static implicit operator FhirDecimal?(string? value) => value is null ? null : new FhirDecimal(value);
    public static implicit operator string?(FhirDecimal? instance) => instance?.StringValue;

    protected override bool ValidateNumericValue(decimal value) => true;

    decimal? IValue<decimal?>.Value => HasValue ? Value : null;
    public bool HasValue => !IsNull;
}

