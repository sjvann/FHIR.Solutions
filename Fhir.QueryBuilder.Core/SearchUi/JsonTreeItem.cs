using System.Collections.ObjectModel;

namespace Fhir.QueryBuilder.SearchUi;

/// <summary>JSON 結果樹狀檢視節點（對齊 WinForms <c>TreeViewExtension</c>）。</summary>
public sealed class JsonTreeItem
{
    public string Header { get; init; } = "";

    public ObservableCollection<JsonTreeItem> Children { get; } = new();
}
