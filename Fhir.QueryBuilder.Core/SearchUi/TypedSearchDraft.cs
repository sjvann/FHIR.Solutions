using CommunityToolkit.Mvvm.ComponentModel;

namespace Fhir.QueryBuilder.SearchUi;

/// <summary>
/// Avalonia／VM 共用的「依型別組句」輸入狀態（對齊 WinForms 各 Panel 欄位）。
/// </summary>
public partial class TypedSearchDraft : ObservableObject
{
    // string
    [ObservableProperty] private string? _stringGroupModifier;
    [ObservableProperty] private string _stringValue = "";

    // token
    [ObservableProperty] private string? _tokenGroupModifier;
    [ObservableProperty] private string _tokenCodeOnly = "";
    [ObservableProperty] private string _tokenSystem = "";
    [ObservableProperty] private string _tokenCodeWithSystem = "";

    // uri
    [ObservableProperty] private string? _uriGroupModifier;
    [ObservableProperty] private string _uriText = "";

    // reference
    [ObservableProperty] private string? _referenceGroupModifier;
    [ObservableProperty] private string _referenceId = "";
    [ObservableProperty] private string _referenceVersion = "";
    [ObservableProperty] private string _referenceType = "";
    [ObservableProperty] private string _referenceTypeId = "";
    [ObservableProperty] private string _referenceUrl = "";

    // number
    [ObservableProperty] private string? _numberGroupModifier;
    [ObservableProperty] private string? _numberPrefix;
    [ObservableProperty] private string _numberValue = "";

    // date
    [ObservableProperty] private string? _dateGroupModifier;
    [ObservableProperty] private string? _datePrefix;
    [ObservableProperty] private string _dateValue = "";

    // quantity
    [ObservableProperty] private string? _quantityGroupModifier;
    [ObservableProperty] private string? _quantityPrefix;
    [ObservableProperty] private string _quantityNumber = "";
    [ObservableProperty] private string _quantityNscNumber = "";
    [ObservableProperty] private string _quantityNscSystem = "";
    [ObservableProperty] private string _quantityNscCode = "";
}
