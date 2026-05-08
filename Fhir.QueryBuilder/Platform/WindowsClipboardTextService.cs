using Fhir.QueryBuilder.Platform;

namespace Fhir.QueryBuilder.PlatformServices;

public sealed class WindowsClipboardTextService : IClipboardTextService
{
    public ValueTask SetTextAsync(string? text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(text))
            return ValueTask.CompletedTask;
        try
        {
            System.Windows.Forms.Clipboard.Clear();
            System.Windows.Forms.Clipboard.SetText(text);
        }
        catch (System.Runtime.InteropServices.ExternalException)
        {
            // Clipboard busy — ignore for parity with typical desktop handling
        }

        return ValueTask.CompletedTask;
    }
}
