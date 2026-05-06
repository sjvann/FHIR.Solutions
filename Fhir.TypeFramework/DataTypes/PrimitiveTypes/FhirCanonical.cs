using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.DataTypes;

public class FhirCanonical : StringPrimitiveTypeBase
{
    public FhirCanonical() { }
    public FhirCanonical(string? v) : base(v) { }
    public static implicit operator FhirCanonical?(string? s) => s is null ? null : new FhirCanonical(s);
    public static implicit operator string?(FhirCanonical? s) => s?.StringValue;

    public override bool IsValidValue(object? value)
        => value is null || (value is string s && ValidationFramework.ValidateFhirCanonical(s) && base.IsValidValue(value));

    protected override bool ValidateStringValue(string value) => ValidationFramework.ValidateFhirCanonical(value);
}

