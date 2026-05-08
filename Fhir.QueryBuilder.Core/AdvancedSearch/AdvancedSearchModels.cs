namespace Fhir.QueryBuilder.AdvancedSearch;

public sealed class ChainParameter
{
    public string Path { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class CompositeParameter
{
    public string ParameterName { get; set; } = string.Empty;
    public string[] Components { get; set; } = Array.Empty<string>();

    public string ComponentsSummary =>
        Components.Length == 0 ? "" : string.Join(" | ", Components);
}

public sealed class ResultControlParameters
{
    public int? Count { get; set; }
    public int? Offset { get; set; }
    public string? Total { get; set; }
    public string? Summary { get; set; }
    public string[]? Elements { get; set; }
}

public sealed class AdvancedParametersChangedEventArgs : EventArgs
{
    public List<ChainParameter> ChainParameters { get; set; } = new();
    public List<CompositeParameter> CompositeParameters { get; set; } = new();
    public List<string> FilterExpressions { get; set; } = new();
    public ResultControlParameters ResultControl { get; set; } = new();
}
