using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.DataTypes;

public class FhirOid : StringPrimitiveTypeBase
{
    public FhirOid() { }
    public FhirOid(string? v) : base(v) { }
    public static implicit operator FhirOid?(string? s) => s is null ? null : new FhirOid(s);
    public static implicit operator string?(FhirOid? s) => s?.StringValue;

    public override bool IsValidValue(object? value)
        => value is null || (value is string s && ValidationFramework.ValidateFhirOid(s) && base.IsValidValue(value));

    protected override bool ValidateStringValue(string value) => ValidationFramework.ValidateFhirOid(value);
}

