using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace Fhir.QueryBuilder.App.Avalonia.Services;

public sealed class AvaloniaApplicationLifetimeService : IApplicationLifetimeService
{
    public void Shutdown()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d)
            d.Shutdown();
    }
}
