using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;

namespace Fhir.Terminology.Core;

public static class FhirPrimitive
{
    public static string? String(FhirString? s) => s?.StringValue;

    public static string? Uri(FhirUri? u) => u?.StringValue;

    public static string? Code(FhirCode? c) => c?.StringValue;
}
