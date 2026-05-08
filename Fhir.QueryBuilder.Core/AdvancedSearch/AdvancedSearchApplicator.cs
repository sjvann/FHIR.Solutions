using Fhir.QueryBuilder.QueryBuilders.FluentApi;

namespace Fhir.QueryBuilder.AdvancedSearch;

/// <summary>將進階搜尋狀態套用至 <see cref="IFhirQueryBuilder"/>（與 WinForms AdvancedSearchControl.ApplyToQueryBuilder 對齊）。</summary>
public static class AdvancedSearchApplicator
{
    public static void Apply(IFhirQueryBuilder builder, ResultControlParameters result,
        IEnumerable<ChainParameter> chains,
        IEnumerable<CompositeParameter> composites,
        IEnumerable<string> filters)
    {
        foreach (var chain in chains)
            builder.Chain(chain.Path, chain.Value);

        foreach (var composite in composites)
            builder.WhereComposite(composite.ParameterName, composite.Components);

        foreach (var filter in filters)
            builder.Filter(filter);

        if (result.Count.HasValue)
            builder.Count(result.Count.Value);

        if (result.Offset.HasValue)
            builder.Offset(result.Offset.Value);

        if (!string.IsNullOrEmpty(result.Total))
            builder.Total(result.Total);

        if (!string.IsNullOrEmpty(result.Summary))
            builder.Summary(result.Summary);

        if (result.Elements?.Length > 0)
            builder.Elements(result.Elements);
    }
}
