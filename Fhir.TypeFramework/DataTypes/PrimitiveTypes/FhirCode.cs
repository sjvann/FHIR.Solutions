using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.DataTypes;

public class FhirCode : StringPrimitiveTypeBase
{
    public FhirCode() { }
    public FhirCode(string? v) : base(v) { }
    public static implicit operator FhirCode?(string? s) => s is null ? null : new FhirCode(s);
    public static implicit operator string?(FhirCode? s) => s?.StringValue;

    public override bool IsValidValue(object? value)
        => value is null || (value is string s && ValidationFramework.ValidateFhirCode(s) && base.IsValidValue(value));

    protected override bool ValidateStringValue(string value) => ValidationFramework.ValidateFhirCode(value);
}

