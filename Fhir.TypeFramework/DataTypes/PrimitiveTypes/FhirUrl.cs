using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.DataTypes;

public class FhirUrl : StringPrimitiveTypeBase
{
    public FhirUrl() { }
    public FhirUrl(string? v) : base(v) { }
    public static implicit operator FhirUrl?(string? s) => s is null ? null : new FhirUrl(s);
    public static implicit operator string?(FhirUrl? s) => s?.StringValue;

    public override bool IsValidValue(object? value)
        => value is null || (value is string s && ValidationFramework.ValidateFhirUri(s) && base.IsValidValue(value));

    protected override bool ValidateStringValue(string value) => ValidationFramework.ValidateFhirUri(value);
}

