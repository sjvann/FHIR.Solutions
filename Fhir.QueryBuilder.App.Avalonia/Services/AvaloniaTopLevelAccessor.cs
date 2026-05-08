using Avalonia.Controls;

namespace Fhir.QueryBuilder.App.Avalonia.Services;

/// <summary>供存檔對話框取得目前主視窗 <see cref="TopLevel.StorageProvider"/>。</summary>
public sealed class AvaloniaTopLevelAccessor
{
    public Window? Window { get; set; }
}
