using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Fhir.QueryBuilder.AdvancedSearch;

namespace Fhir.QueryBuilder.SearchUi;

/// <summary>進階搜尋（chain／composite／filter／結果控制）編輯狀態。</summary>
public partial class AdvancedSearchDraft : ObservableObject
{
    public ObservableCollection<ChainParameter> Chains { get; } = new();
    public ObservableCollection<CompositeParameter> Composites { get; } = new();
    public ObservableCollection<string> Filters { get; } = new();

    [ObservableProperty] private string _pendingChainPath = "";
    [ObservableProperty] private string _pendingChainValue = "";

    [ObservableProperty] private string _pendingCompositeName = "";
    [ObservableProperty] private string _pendingCompositeComponents = "";

    [ObservableProperty] private string _pendingFilterExpression = "";

    [ObservableProperty] private string _resultCountText = "";
    [ObservableProperty] private string _resultOffsetText = "";

    /// <summary>_total 模式（如 accurate、none）。</summary>
    [ObservableProperty] private string _resultTotalMode = "accurate";

    /// <summary>_summary 模式（如 true、false、text）。</summary>
    [ObservableProperty] private string _resultSummaryMode = "false";

    [ObservableProperty] private string _resultElementsCsv = "";

    public ResultControlParameters BuildResultControlParameters()
    {
        var r = new ResultControlParameters();
        if (int.TryParse(ResultCountText, out var c) && c > 0)
            r.Count = c;
        if (int.TryParse(ResultOffsetText, out var o) && o >= 0)
            r.Offset = o;
        r.Total = string.IsNullOrWhiteSpace(ResultTotalMode) ? null : ResultTotalMode.Trim();
        r.Summary = string.IsNullOrWhiteSpace(ResultSummaryMode) ? null : ResultSummaryMode.Trim();
        if (!string.IsNullOrWhiteSpace(ResultElementsCsv))
        {
            r.Elements = ResultElementsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(e => e.Trim()).ToArray();
        }

        return r;
    }

    public bool TryAddPendingChain()
    {
        if (string.IsNullOrWhiteSpace(PendingChainPath) || string.IsNullOrWhiteSpace(PendingChainValue))
            return false;
        Chains.Add(new ChainParameter { Path = PendingChainPath.Trim(), Value = PendingChainValue.Trim() });
        PendingChainPath = string.Empty;
        PendingChainValue = string.Empty;
        return true;
    }

    public bool TryAddPendingComposite()
    {
        if (string.IsNullOrWhiteSpace(PendingCompositeName) || string.IsNullOrWhiteSpace(PendingCompositeComponents))
            return false;
        var components = PendingCompositeComponents.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(c => c.Trim()).ToArray();
        if (components.Length < 2)
            return false;
        Composites.Add(new CompositeParameter { ParameterName = PendingCompositeName.Trim(), Components = components });
        PendingCompositeName = string.Empty;
        PendingCompositeComponents = string.Empty;
        return true;
    }

    public bool TryAddPendingFilter()
    {
        if (string.IsNullOrWhiteSpace(PendingFilterExpression))
            return false;
        Filters.Add(PendingFilterExpression.Trim());
        PendingFilterExpression = string.Empty;
        return true;
    }

    public void RemoveChainAt(int index)
    {
        if (index >= 0 && index < Chains.Count)
            Chains.RemoveAt(index);
    }

    public void RemoveCompositeAt(int index)
    {
        if (index >= 0 && index < Composites.Count)
            Composites.RemoveAt(index);
    }

    public void RemoveFilterAt(int index)
    {
        if (index >= 0 && index < Filters.Count)
            Filters.RemoveAt(index);
    }

    public void ClearAll()
    {
        Chains.Clear();
        Composites.Clear();
        Filters.Clear();
        PendingChainPath = PendingChainValue = string.Empty;
        PendingCompositeName = PendingCompositeComponents = string.Empty;
        PendingFilterExpression = string.Empty;
        ResultCountText = ResultOffsetText = string.Empty;
        ResultTotalMode = "accurate";
        ResultSummaryMode = "false";
        ResultElementsCsv = string.Empty;
    }
}
