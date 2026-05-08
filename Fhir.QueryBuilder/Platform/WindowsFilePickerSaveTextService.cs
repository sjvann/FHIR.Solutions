using Fhir.QueryBuilder.Platform;

namespace Fhir.QueryBuilder.PlatformServices;

public sealed class WindowsFilePickerSaveTextService : IFilePickerSaveTextService
{
    public Task<string?> PickSaveFilePathAsync(string suggestedFileName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var dlg = new System.Windows.Forms.SaveFileDialog
        {
            Filter = "json files (*.json)|*.json|All files (*.*)|*.*",
            FilterIndex = 1,
            RestoreDirectory = true,
            FileName = suggestedFileName
        };

        return dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK
            ? Task.FromResult<string?>(dlg.FileName)
            : Task.FromResult<string?>(null);
    }
}
