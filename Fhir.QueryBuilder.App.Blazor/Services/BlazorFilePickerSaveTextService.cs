using Fhir.QueryBuilder.Platform;
using Microsoft.JSInterop;

namespace Fhir.QueryBuilder.App.Blazor.Services;

/// <summary>WASM：以建議檔名觸發瀏覽器下載（無傳統檔案路徑對話框）。</summary>
public sealed class BlazorFilePickerSaveTextService(IJSRuntime js) : IFilePickerSaveTextService
{
    public Task<string?> PickSaveFilePathAsync(string suggestedFileName, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(suggestedFileName);

    public Task SaveTextAsync(string path, string content, CancellationToken cancellationToken = default)
        => js.InvokeVoidAsync("fhirQueryBuilderInterop.downloadTextFile", cancellationToken, path, content).AsTask();
}
