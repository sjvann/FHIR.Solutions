using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Fhir.QueryBuilder.App.Avalonia.Services;
using Fhir.QueryBuilder.ViewModels;

namespace Fhir.QueryBuilder.App.Avalonia;

public partial class MainWindow : Window
{
    private readonly IApplicationLifetimeService? _lifetime;

    /// <summary>設計器／執行時載入 XAML 所需。</summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(
        MainViewModel viewModel,
        IApplicationLifetimeService lifetime,
        AvaloniaTopLevelAccessor topLevelAccessor)
        : this()
    {
        DataContext = viewModel;
        _lifetime = lifetime;
        Loaded += (_, _) =>
        {
            if (DataContext is MainViewModel vm)
                vm.SetUiThreadPost(a => Dispatcher.UIThread.Post(a, DispatcherPriority.Normal));
        };
        Opened += (_, _) => topLevelAccessor.Window = this;
        Closing += (_, _) =>
        {
            if (ReferenceEquals(topLevelAccessor.Window, this))
                topLevelAccessor.Window = null;
        };
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        _lifetime?.Shutdown();
    }

    private async void ResourceAutoComplete_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not AutoCompleteBox ac || DataContext is not MainViewModel vm)
            return;

        var picked = ac.SelectedItem as string;
        if (string.IsNullOrEmpty(picked) || !vm.IsConnected)
            return;

        if (!string.Equals(vm.SelectedResource, picked, StringComparison.Ordinal))
            vm.SelectedResource = picked;

        await vm.SelectResourceCommand.ExecuteAsync(null);
    }

    private async void ResourceAutoComplete_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is not AutoCompleteBox ac || DataContext is not MainViewModel vm)
            return;

        var text = ac.Text?.Trim() ?? "";
        var match = vm.SupportedResources.FirstOrDefault(r =>
            string.Equals(r, text, StringComparison.OrdinalIgnoreCase));
        if (match == null)
            return;

        if (ac.SelectedItem is string picked &&
            string.Equals(picked, match, StringComparison.OrdinalIgnoreCase))
            return;

        vm.SelectedResource = match;
        await vm.SelectResourceCommand.ExecuteAsync(null);
    }
}
