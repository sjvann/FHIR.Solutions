namespace Fhir.QueryBuilder.Models;

/// <summary>
/// SearchParameter.component 之一筆（TD-1：依伺服器 SearchParameter 資源解析）。
/// </summary>
public sealed class SearchParamComponentModel
{
    public required int Index { get; init; }

    /// <summary>元件指向之 SearchParameter 的 canonical URL。</summary>
    public string? DefinitionCanonical { get; init; }

    /// <summary>由 definition 解析出的 FHIR search parameter type（number、token…）。</summary>
    public string? ResolvedParameterType { get; init; }

    public string ShortDefinition =>
        string.IsNullOrEmpty(DefinitionCanonical)
            ? $"Part {Index + 1}"
            : TrimCanonicalTail(DefinitionCanonical);

    /// <summary>列標題（含型別提示）。</summary>
    public string RowCaption =>
        string.IsNullOrEmpty(ResolvedParameterType)
            ? ShortDefinition
            : $"{ShortDefinition} ({ResolvedParameterType})";

    private static string TrimCanonicalTail(string canonical)
    {
        var s = canonical.TrimEnd('/');
        var i = s.LastIndexOf('/');
        return i >= 0 ? s[(i + 1)..] : canonical;
    }
}
