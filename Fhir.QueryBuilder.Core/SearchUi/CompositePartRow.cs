using CommunityToolkit.Mvvm.ComponentModel;

namespace Fhir.QueryBuilder.SearchUi;

/// <summary>Composite rich UI 單列（對應 SearchParameter.component 之一）。</summary>
public partial class CompositePartRow : ObservableObject
{
    [ObservableProperty]
    private string _label = "";

    /// <summary>FHIR type 提示（token、date…），來自 Capability／伺服器。</summary>
    [ObservableProperty]
    private string _typeHint = "";

    /// <summary>
    /// 由 <see cref="TypeHint"/>／ResolvedParameterType 正規化之小寫 FHIR search parameter type（token、quantity…）。
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTokenComponent))]
    [NotifyPropertyChangedFor(nameof(IsQuantityComponent))]
    [NotifyPropertyChangedFor(nameof(IsFallbackComponent))]
    private string _normalizedComponentType = "";

    /// <summary>後備：原始元件字串（metadata 未載入型別或小寫表單不適用時）。</summary>
    [ObservableProperty]
    private string _value = "";

    // --- Token（composite 元件片段不含每元件 modifier，見 ComposeTokenValueBody）---

    [ObservableProperty]
    private string _tokenCodeOnly = "";

    [ObservableProperty]
    private string _tokenSystem = "";

    [ObservableProperty]
    private string _tokenCodeWithSystem = "";

    // --- Quantity ---

    [ObservableProperty]
    private string? _quantityPrefix;

    [ObservableProperty]
    private string _quantityNumber = "";

    [ObservableProperty]
    private string _quantityNscNumber = "";

    [ObservableProperty]
    private string _quantityNscSystem = "";

    [ObservableProperty]
    private string _quantityNscCode = "";

    public bool IsTokenComponent =>
        string.Equals(NormalizedComponentType.Trim(), "token", StringComparison.OrdinalIgnoreCase);

    public bool IsQuantityComponent =>
        string.Equals(NormalizedComponentType.Trim(), "quantity", StringComparison.OrdinalIgnoreCase);

    public bool IsFallbackComponent => !IsTokenComponent && !IsQuantityComponent;

    partial void OnNormalizedComponentTypeChanged(string value)
    {
        OnPropertyChanged(nameof(IsTokenComponent));
        OnPropertyChanged(nameof(IsQuantityComponent));
        OnPropertyChanged(nameof(IsFallbackComponent));
    }
}
