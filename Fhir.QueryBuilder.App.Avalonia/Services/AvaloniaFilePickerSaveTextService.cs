using Avalonia.Platform.Storage;
using Fhir.QueryBuilder.Platform;

namespace Fhir.QueryBuilder.App.Avalonia.Services;

public sealed class AvaloniaFilePickerSaveTextService(AvaloniaTopLevelAccessor topLevelAccessor) : IFilePickerSaveTextService
{
    public async Task<string?> PickSaveFilePathAsync(string suggestedFileName, CancellationToken cancellationToken = default)
    {
        var window = topLevelAccessor.Window;
        if (window == null)
            return null;

        var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save JSON",
            SuggestedFileName = suggestedFileName,
            DefaultExtension = "json",
            ShowOverwritePrompt = true,
            FileTypeChoices =
            [
                new FilePickerFileType("JSON") { Patterns = ["*.json"] },
                new FilePickerFileType("All files") { Patterns = ["*.*"] }
            ]
        }).ConfigureAwait(false);

        return file?.TryGetLocalPath();
    }
}
