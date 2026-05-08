using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Fhir.QueryBuilder.SearchUi;

/// <summary>
/// Avalonia／VM 共用的「結果修飾參數」（對齊 WinForms <c>ModifyResult</c>）。
/// </summary>
public partial class ModifyingSearchDraft : ObservableObject
{
    /// <summary>空白開頭表示「未選」（不送出該參數）。</summary>
    public IReadOnlyList<string> ContainedPresetChoices { get; } =
        ["", "both", "true", "false"];

    public IReadOnlyList<string> ContainedTypePresetChoices { get; } =
        ["", "container", "contained"];

    public IReadOnlyList<string> SummaryPresetChoices { get; } =
        ["", "count", "data", "text", "false", "true"];

    /// <summary>空白表示未選（不送出 <c>_contained</c>）。值：<c>both</c>、<c>true</c>、<c>false</c>。</summary>
    [ObservableProperty]
    private string _containedChoice = "";

    /// <summary>空白表示未選。<c>container</c>、<c>contained</c>。</summary>
    [ObservableProperty]
    private string _containedTypeChoice = "";

    [ObservableProperty]
    private string _countValue = "";

    [ObservableProperty]
    private string _maxResultsValue = "";

    /// <summary>空白表示未選。<c>count</c>、<c>data</c>、<c>text</c>、<c>false</c>、<c>true</c>。</summary>
    [ObservableProperty]
    private string _summaryChoice = "";

    public ObservableCollection<string> ElementChoices { get; } = new();

    public ObservableCollection<string> Elements { get; } = new();

    public ObservableCollection<string> SortLines { get; } = new();

    [ObservableProperty]
    private string? _selectedElementHint;

    [ObservableProperty]
    private bool _sortDescendingPrefix;

    /// <summary>依目前資源類型更新下拉提示（與 WinForms <c>GetResourceElement</c> 對齊）。</summary>
    public void RefreshElementChoices(string? resourceName)
    {
        ElementChoices.Clear();
        foreach (var hint in GetResourceElementHints(resourceName))
            ElementChoices.Add(hint);

        SelectedElementHint = ElementChoices.Count > 0 ? ElementChoices[0] : null;
    }

    private static IEnumerable<string> GetResourceElementHints(string? resourceName)
    {
        if (string.IsNullOrEmpty(resourceName))
            yield break;

        if (resourceName.Equals("Patient", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var s in new[] { "id", "identifier", "name", "birthDate", "gender", "address", "telecom" })
                yield return s;
        }
        else
        {
            yield return "id";
            yield return "meta";
        }
    }

    public void AddSelectedToElements()
    {
        if (string.IsNullOrEmpty(SelectedElementHint))
            return;
        if (!Elements.Contains(SelectedElementHint))
            Elements.Add(SelectedElementHint);
    }

    public void AddSelectedToSort()
    {
        if (string.IsNullOrEmpty(SelectedElementHint))
            return;

        var piece = SortDescendingPrefix ? $"-{SelectedElementHint}" : SelectedElementHint;
        SortLines.Add(piece);
    }

    /// <summary>對齊 WinForms <c>ModifyResult.Btn_Cancel_Click</c>（Reset）。</summary>
    public void ResetForm()
    {
        ContainedChoice = "";
        ContainedTypeChoice = "";
        CountValue = "";
        MaxResultsValue = "";
        SummaryChoice = "";
        Elements.Clear();
        SortLines.Clear();
    }

    /// <summary>對齊 WinForms <c>ModifyResult.Btn_OK_Click</c> 產生的片段。</summary>
    public List<string> ToParameterStrings()
    {
        var list = new List<string>();

        if (!string.IsNullOrEmpty(ContainedChoice))
            list.Add($"_contained={ContainedChoice}");

        if (!string.IsNullOrEmpty(ContainedTypeChoice))
            list.Add($"_containedType={ContainedTypeChoice}");

        if (!string.IsNullOrWhiteSpace(CountValue))
            list.Add($"_count={CountValue.Trim()}");

        if (!string.IsNullOrWhiteSpace(MaxResultsValue))
            list.Add($"_maxresults={MaxResultsValue.Trim()}");

        if (!string.IsNullOrEmpty(SummaryChoice))
            list.Add($"_summary={SummaryChoice}");

        if (Elements.Count > 0)
            list.Add($"_elements={string.Join(",", Elements)}");

        if (SortLines.Count > 0)
            list.Add($"_sort={string.Join(",", SortLines)}");

        return list;
    }
}
