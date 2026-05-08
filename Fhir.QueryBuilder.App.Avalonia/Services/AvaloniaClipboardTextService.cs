using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Fhir.QueryBuilder.Platform;

namespace Fhir.QueryBuilder.App.Avalonia.Services;

public sealed class AvaloniaClipboardTextService : IClipboardTextService
{
    public async ValueTask SetTextAsync(string? text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(text))
            return;

        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        if (desktop.MainWindow is not { Clipboard: { } clipboard })
            return;

        await clipboard.SetTextAsync(text);
    }
}
