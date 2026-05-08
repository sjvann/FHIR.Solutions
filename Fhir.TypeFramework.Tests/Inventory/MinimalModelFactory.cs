using System.Reflection;
using Fhir.TypeFramework.Bases;
namespace Fhir.TypeFramework.Tests.Inventory;

/// <summary>
/// 建立可供 JSON 往返的最小合法實例（primitive 給予最小 wire 字串／數值）。
/// </summary>
public static class MinimalModelFactory
{
    public static Base CreateMinimal(Type type)
    {
        if (!typeof(Base).IsAssignableFrom(type))
            throw new ArgumentException($"Not a Base-derived type: {type.FullName}", nameof(type));

        if (typeof(PrimitiveType).IsAssignableFrom(type))
            return CreateMinimalPrimitive(type);

        var obj = Activator.CreateInstance(type)
                  ?? throw new InvalidOperationException($"Cannot create instance of {type.FullName}");
        return (Base)obj;
    }

    static Base CreateMinimalPrimitive(Type type)
    {
        var name = type.Name;
        var wire = SampleWireString(name);
        var inst = Activator.CreateInstance(type)
                   ?? throw new InvalidOperationException($"Cannot create primitive {type.FullName}");

        var p = (PrimitiveType)inst;

        // 多數 primitive 以 StringValue / value JSON 往返
        var stringValueProp = type.GetProperty("StringValue", BindingFlags.Public | BindingFlags.Instance);
        if (stringValueProp?.CanWrite == true)
        {
            stringValueProp.SetValue(p, wire);
            return p;
        }

        // 無 StringValue 時呼叫已知 ctor（數值類）
        foreach (var ctor in type.GetConstructors())
        {
            var ps = ctor.GetParameters();
            if (ps.Length == 1 && ps[0].ParameterType == typeof(int))
            {
                return (Base)ctor.Invoke([42]);
            }

            if (ps.Length == 1 && ps[0].ParameterType == typeof(long))
            {
                return (Base)ctor.Invoke([42L]);
            }

            if (ps.Length == 1 && ps[0].ParameterType == typeof(decimal))
            {
                return (Base)ctor.Invoke([1.0m]);
            }

            if (ps.Length == 1 && ps[0].ParameterType == typeof(bool))
            {
                return (Base)ctor.Invoke([true]);
            }
        }

        throw new InvalidOperationException($"No minimal wiring strategy for primitive {type.FullName}");
    }

    /// <summary>對應 FHIR JSON primitive「value」慣例的最小字串。</summary>
    static string SampleWireString(string clrTypeName) =>
        clrTypeName switch
        {
            "FhirBoolean" => "true",
            "FhirInteger" => "42",
            "FhirInteger64" => "42",
            "FhirDecimal" => "1.0",
            "FhirUnsignedInt" => "1",
            "FhirPositiveInt" => "1",
            "FhirDate" => "2020-01-01",
            "FhirDateTime" => "2020-01-01T12:00:00Z",
            "FhirInstant" => "2020-01-01T12:00:00Z",
            "FhirTime" => "12:00:00",
            "FhirBase64Binary" => "dGVzdA==",
            "FhirOid" => "urn:oid:1.2.3",
            "FhirUuid" => "urn:uuid:00000000-0000-0000-0000-000000000001",
            "FhirCanonical" => "http://hl7.org/fhir/StructureDefinition/Patient",
            "FhirUrl" => "http://example.org",
            "FhirUri" => "http://example.org",
            "FhirMarkdown" => "x",
            "FhirCode" => "active",
            "FhirString" => "test",
            "FhirId" => "id1",
            "FhirXhtml" => "<div xmlns=\"http://www.w3.org/1999/xhtml\">x</div>",
            _ => "test"
        };
}
