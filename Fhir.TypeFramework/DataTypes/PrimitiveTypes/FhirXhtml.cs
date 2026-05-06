using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

public class FhirXhtml : StringPrimitiveTypeBase
{
    public FhirXhtml() { }
    public FhirXhtml(string? v) : base(v) { }
    public static implicit operator FhirXhtml?(string? s) => s is null ? null : new FhirXhtml(s);
    public static implicit operator string?(FhirXhtml? s) => s?.StringValue;

    protected override bool ValidateStringValue(string value) => true;
}

