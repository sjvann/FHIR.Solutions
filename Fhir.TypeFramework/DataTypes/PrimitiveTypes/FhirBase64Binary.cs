using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.DataTypes;

public class FhirBase64Binary : StringPrimitiveTypeBase
{
    public FhirBase64Binary() { }
    public FhirBase64Binary(string? v) : base(v) { }
    public static implicit operator FhirBase64Binary?(string? s) => s is null ? null : new FhirBase64Binary(s);
    public static implicit operator string?(FhirBase64Binary? s) => s?.StringValue;

    protected override bool ValidateStringValue(string value) => ValidationFramework.ValidateFhirBase64Binary(value);
}

