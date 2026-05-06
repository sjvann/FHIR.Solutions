using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Interface;

namespace Fhir.TypeFramework.DataTypes;

public class FhirUnsignedInt : NumericPrimitiveTypeBase<uint>, IUnsignedInteger32Value
{
    public FhirUnsignedInt() { }
    public FhirUnsignedInt(uint v) : base(v) { }
    public FhirUnsignedInt(string? v) : base(v) { }

    public static implicit operator FhirUnsignedInt?(uint? value) => CreateFromNumber<FhirUnsignedInt>(value);
    public static implicit operator uint?(FhirUnsignedInt? instance) => GetNumericValue(instance);
    public static implicit operator FhirUnsignedInt?(string? value) => value is null ? null : new FhirUnsignedInt(value);
    public static implicit operator string?(FhirUnsignedInt? instance) => instance?.StringValue;

    protected override bool ValidateNumericValue(uint value) => true;

    uint? IValue<uint?>.Value => HasValue ? Value : null;
    public bool HasValue => !IsNull;
}

