using System.Text;
using FhirResourceCreator.Models;

namespace FhirResourceCreator.Generation;

/// <summary>
/// Emits object-initializer lines for required root-level primitive elements (generator smoke construction test).
/// </summary>
public static class ResourceTestConstructionEmitter
{
    public static string EmitObjectInitializerBody(string resourceTypeName, IReadOnlyList<ElementRecord> records)
    {
        var sb = new StringBuilder();
        var safeId = SanitizeForId(resourceTypeName);
        sb.AppendLine($"            Id = new FhirId(\"tc-{safeId}-id\"),");

        foreach (var el in records)
        {
            if (el.IsSkip || !el.IsMust || el.IsChoice || el.IsBackboneElement)
                continue;
            if (!el.IsPrimitive || el.IsMulti)
                continue;
            if (!string.Equals(el.ParentPath, resourceTypeName, StringComparison.Ordinal))
                continue;

            var prop = el.FinalElementName;
            if (string.IsNullOrEmpty(prop))
                continue;
            if (string.Equals(prop, "ResourceTypeJson", StringComparison.Ordinal))
                continue;
            if (string.Equals(prop, "Id", StringComparison.Ordinal))
                continue;

            var clr = el.MappedPrimitiveClr;
            if (string.IsNullOrEmpty(clr))
                continue;

            sb.AppendLine($"            {prop} = {NewPrimitiveExpression(clr, el.OriginalElementName)},");
        }

        return sb.ToString();
    }

    static string SanitizeForId(string typeName)
    {
        var sb = new StringBuilder(typeName.Length);
        foreach (var c in typeName)
        {
            if (char.IsLetterOrDigit(c))
                sb.Append(c);
            else
                sb.Append('-');
        }

        return sb.Length > 0 ? sb.ToString() : "res";
    }

    static string NewPrimitiveExpression(string clr, string? jsonName)
    {
        var n = jsonName ?? "";
        var statusHint = n.Contains("status", StringComparison.OrdinalIgnoreCase);

        return clr switch
        {
            "FhirCode" => statusHint ? "new FhirCode(\"final\")" : "new FhirCode(\"test\")",
            "FhirBoolean" => "new FhirBoolean(true)",
            "FhirInteger" => "new FhirInteger(1)",
            "FhirInteger64" => "new FhirInteger64(1L)",
            "FhirPositiveInt" => "new FhirPositiveInt(1)",
            "FhirUnsignedInt" => "new FhirUnsignedInt(1)",
            "FhirDecimal" => "new FhirDecimal(1m)",
            "FhirDate" => "new FhirDate(\"2020-01-01\")",
            "FhirDateTime" => "new FhirDateTime(\"2020-01-01T12:00:00Z\")",
            "FhirInstant" => "new FhirInstant(\"2020-01-01T12:00:00Z\")",
            "FhirTime" => "new FhirTime(\"12:00:00\")",
            "FhirString" => "new FhirString(\"test\")",
            "FhirMarkdown" => "new FhirMarkdown(\"test\")",
            "FhirId" => "new FhirId(\"tc-id\")",
            "FhirOid" => "new FhirOid(\"urn:oid:1.2.3.4.5\")",
            "FhirUuid" => "new FhirUuid(\"urn:uuid:00000000-0000-0000-0000-000000000001\")",
            "FhirUri" => "new FhirUri(\"http://example.org\")",
            "FhirUrl" => "new FhirUrl(\"http://example.org\")",
            "FhirCanonical" => "new FhirCanonical(\"http://hl7.org/fhir/StructureDefinition/Patient\")",
            "FhirBase64Binary" => "new FhirBase64Binary(\"dGVzdA==\")",
            "FhirXhtml" => "new FhirXhtml(\"<div xmlns=\\\"http://www.w3.org/1999/xhtml\\\">x</div>\")",
            _ => "new FhirString(\"test\")"
        };
    }
}
