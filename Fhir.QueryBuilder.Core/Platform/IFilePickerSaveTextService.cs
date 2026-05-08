namespace Fhir.QueryBuilder.Platform;

/// <summary>提示使用者選擇儲存路徑；取消則回傳 null。</summary>
public interface IFilePickerSaveTextService
{
    Task<string?> PickSaveFilePathAsync(string suggestedFileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 將內容寫入 <paramref name="path"/>（桌面為真實路徑）。WebAssembly 等環境可改為觸發瀏覽器下載，仍以 <paramref name="path"/> 作為建議檔名。
    /// </summary>
    Task SaveTextAsync(string path, string content, CancellationToken cancellationToken = default);
}
