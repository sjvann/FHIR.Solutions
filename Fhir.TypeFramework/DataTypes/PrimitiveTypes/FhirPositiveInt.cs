using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Interface;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.DataTypes;

public class FhirPositiveInt : NumericPrimitiveTypeBase<int>, IInteger32Value
{
    public FhirPositiveInt() { }
    public FhirPositiveInt(int v) : base(v) { }
    public FhirPositiveInt(string? v) : base(v) { }

    public static implicit operator FhirPositiveInt?(int? value) => CreateFromNumber<FhirPositiveInt>(value);
    public static implicit operator int?(FhirPositiveInt? instance) => GetNumericValue(instance);
    public static implicit operator FhirPositiveInt?(string? value) => value is null ? null : new FhirPositiveInt(value);
    public static implicit operator string?(FhirPositiveInt? instance) => instance?.StringValue;

    protected override bool ValidateNumericValue(int value) => ValidationFramework.ValidatePositiveInteger(value);

    int? IValue<int?>.Value => HasValue ? Value : null;
    public bool HasValue => !IsNull;
}

