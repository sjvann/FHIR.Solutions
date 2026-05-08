using System.Reflection;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.Serialization;

/// <summary>
/// 從資源組件（例如 Fhir.Resources.R5）以 <c>ResourceTypeValue</c> 靜態欄位建立
/// <c>resourceType</c> 字串 → CLR 型別 對照表，供多型別 JSON 反序列化使用。
/// </summary>
public static class FhirResourceTypeMap
{
    public static IReadOnlyDictionary<string, Type> FromResourceAssembly(Assembly assembly, Type resourceBaseType)
    {
        if (!typeof(Resource).IsAssignableFrom(resourceBaseType))
            throw new ArgumentException("resourceBaseType must inherit from Resource.", nameof(resourceBaseType));

        var dict = new Dictionary<string, Type>(StringComparer.Ordinal);
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || !resourceBaseType.IsAssignableFrom(type))
                continue;
            var field = type.GetField("ResourceTypeValue", BindingFlags.Public | BindingFlags.Static);
            if (field?.GetValue(null) is string name && !string.IsNullOrEmpty(name))
                dict[name] = type;
        }

        return dict;
    }
}
