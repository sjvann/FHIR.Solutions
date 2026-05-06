using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.DataTypes;

public class FhirId : StringPrimitiveTypeBase
{
    public FhirId() { }
    public FhirId(string? v) : base(v) { }
    public static implicit operator FhirId?(string? s) => s is null ? null : new FhirId(s);
    public static implicit operator string?(FhirId? s) => s?.StringValue;

    public override bool IsValidValue(object? value)
        => value is null || (value is string s && ValidationFramework.ValidateFhirId(s) && base.IsValidValue(value));

    protected override bool ValidateStringValue(string value) => ValidationFramework.ValidateFhirId(value);
}

