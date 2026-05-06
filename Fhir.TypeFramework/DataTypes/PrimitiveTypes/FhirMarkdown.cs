using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

public class FhirMarkdown : StringPrimitiveTypeBase
{
    public FhirMarkdown() { }
    public FhirMarkdown(string? v) : base(v) { }
    public static implicit operator FhirMarkdown?(string? s) => s is null ? null : new FhirMarkdown(s);
    public static implicit operator string?(FhirMarkdown? s) => s?.StringValue;

    protected override bool ValidateStringValue(string value) => true;
}

