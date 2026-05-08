namespace Fhir.QueryBuilder.Platform;

/// <summary>跨 UI 框架的文字剪貼簿寫入（另於各桌面殼實作）。</summary>
public interface IClipboardTextService
{
    ValueTask SetTextAsync(string? text, CancellationToken cancellationToken = default);
}
