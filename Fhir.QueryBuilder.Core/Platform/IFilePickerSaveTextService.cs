namespace Fhir.QueryBuilder.Platform;

/// <summary>提示使用者選擇儲存路徑；取消則回傳 null。</summary>
public interface IFilePickerSaveTextService
{
    Task<string?> PickSaveFilePathAsync(string suggestedFileName, CancellationToken cancellationToken = default);
}
