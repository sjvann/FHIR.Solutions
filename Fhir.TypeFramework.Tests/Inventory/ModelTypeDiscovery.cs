using System.Reflection;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.Tests.Inventory;

/// <summary>
/// 列出組件中「應被 JSON 往返涵蓋」的具體模型型別（供測試總管逐型別顯示案例）。
/// </summary>
public static class ModelTypeDiscovery
{
    static readonly string[] NamespacePrefixes =
    [
        "Fhir.TypeFramework.DataTypes",
    ];

    /// <summary>開發／範例等非正式模型目錄。</summary>
    static readonly HashSet<string> ExcludedPathsSegments =
    [
        "Development",
        "Examples",
        "Performance",
        "Templates",
    ];

    public static IReadOnlyList<Type> GetConcreteModelTypes()
    {
        var asm = typeof(Base).Assembly;
        var types = asm.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(Base).IsAssignableFrom(t))
            .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
            .Where(t =>
            {
                var ns = t.Namespace ?? "";
                if (!NamespacePrefixes.Any(ns.StartsWith))
                    return false;
                return !ExcludedPathsSegments.Any(seg => ns.Contains(seg, StringComparison.Ordinal));
            })
            .OrderBy(t => t.FullName, StringComparer.Ordinal)
            .ToArray();

        return types;
    }
}
