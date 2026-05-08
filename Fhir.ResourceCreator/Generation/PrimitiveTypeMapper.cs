namespace FhirResourceCreator.Generation;

/// <summary>
/// Maps FHIR primitive type codes (elementdefinition-types) to Fhir.TypeFramework primitive CLR type names (unqualified; use with PrimitiveTypes namespace).
/// </summary>
public static class PrimitiveTypeMapper
{
    static readonly HashSet<string> PrimitiveCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "base64Binary", "boolean", "canonical", "code", "date", "dateTime", "decimal", "id", "instant",
        "integer", "integer64", "markdown", "oid", "positiveInt", "string", "time", "unsignedInt",
        "uri", "url", "uuid", "xhtml"
    };

    public static bool IsPrimitiveCode(string code) => PrimitiveCodes.Contains(code);

    public static string ToClrTypeName(string fhirCodeLower)
    {
        var c = fhirCodeLower.Trim().ToLowerInvariant();
        return c switch
        {
            "base64binary" => "FhirBase64Binary",
            "boolean" => "FhirBoolean",
            "canonical" => "FhirCanonical",
            "code" => "FhirCode",
            "date" => "FhirDate",
            "datetime" => "FhirDateTime",
            "decimal" => "FhirDecimal",
            "id" => "FhirId",
            "instant" => "FhirInstant",
            "integer" => "FhirInteger",
            "integer64" => "FhirInteger64",
            "markdown" => "FhirMarkdown",
            "oid" => "FhirOid",
            "positiveint" => "FhirPositiveInt",
            "string" => "FhirString",
            "time" => "FhirTime",
            "unsignedint" => "FhirUnsignedInt",
            "uri" => "FhirUri",
            "url" => "FhirUrl",
            "uuid" => "FhirUuid",
            "xhtml" => "FhirXhtml",
            _ => IdentifierUtility.ToPascalCase(c)
        };
    }
}
