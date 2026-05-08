namespace Fhir.QueryBuilder.Models;

/// <summary>搜尋參數列表項目（UI／CapabilityStatement 對應）。</summary>
public sealed class SearchParamModel
{
    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string? Documentation { get; set; }

    public string DisplayText => $"{Name} : {Type}";
}
