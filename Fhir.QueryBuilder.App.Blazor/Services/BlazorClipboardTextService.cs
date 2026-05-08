using Fhir.QueryBuilder.Platform;
using Microsoft.JSInterop;

namespace Fhir.QueryBuilder.App.Blazor.Services;

public sealed class BlazorClipboardTextService(IJSRuntime js) : IClipboardTextService
{
    public async ValueTask SetTextAsync(string? text, CancellationToken cancellationToken = default)
    {
        if (text == null)
            return;
        await js.InvokeVoidAsync("fhirQueryBuilderInterop.setClipboardText", cancellationToken, text);
    }
}
