using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.DataTypes;

public class FhirUri : StringPrimitiveTypeBase
{
    public FhirUri() { }
    public FhirUri(string? v) : base(v) { }
    public static implicit operator FhirUri?(string? s) => s is null ? null : new FhirUri(s);
    public static implicit operator string?(FhirUri? s) => s?.StringValue;

    public override bool IsValidValue(object? value)
        => value is null || (value is string s && ValidationFramework.ValidateFhirUri(s) && base.IsValidValue(value));

    protected override bool ValidateStringValue(string value) => ValidationFramework.ValidateFhirUri(value);
}

