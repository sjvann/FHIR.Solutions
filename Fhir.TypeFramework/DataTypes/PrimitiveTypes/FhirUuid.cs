using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.DataTypes;

public class FhirUuid : StringPrimitiveTypeBase
{
    public FhirUuid() { }
    public FhirUuid(string? v) : base(v) { }
    public static implicit operator FhirUuid?(string? s) => s is null ? null : new FhirUuid(s);
    public static implicit operator string?(FhirUuid? s) => s?.StringValue;

    public override bool IsValidValue(object? value)
        => value is null || (value is string s && ValidationFramework.ValidateFhirUuid(s) && base.IsValidValue(value));

    protected override bool ValidateStringValue(string value) => ValidationFramework.ValidateFhirUuid(value);
}

