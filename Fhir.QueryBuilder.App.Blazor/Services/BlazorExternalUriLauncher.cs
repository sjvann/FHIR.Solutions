using Fhir.QueryBuilder.Platform;
using Microsoft.JSInterop;

namespace Fhir.QueryBuilder.App.Blazor.Services;

public sealed class BlazorExternalUriLauncher(IJSRuntime js) : IExternalUriLauncher
{
    public void OpenUri(string url)
        => _ = js.InvokeVoidAsync("fhirQueryBuilderInterop.openUrl", url);
}
