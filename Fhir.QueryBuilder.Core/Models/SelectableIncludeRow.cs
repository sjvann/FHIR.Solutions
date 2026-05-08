using CommunityToolkit.Mvvm.ComponentModel;

namespace Fhir.QueryBuilder.Models;

/// <summary>_include / _revinclude 列：供 Avalonia／WinForms 勾選同步。</summary>
public partial class SelectableIncludeRow : ObservableObject
{
    public SelectableIncludeRow(string value)
    {
        Value = value;
    }

    public string Value { get; }

    [ObservableProperty]
    private bool _isSelected;
}
