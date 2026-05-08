using System.Collections.Specialized;
using System.ComponentModel;
using Fhir.QueryBuilder.ViewModels;

namespace Fhir.QueryBuilder.App.Blazor.Services;

/// <summary>將 <see cref="MainViewModel"/> 的通知轉成 Blazor 重繪（Observable / 集合變更）。</summary>
internal static class MainViewModelRefreshBinder
{
    /// <summary>
    /// 綁定 VM 變更通知；<paramref name="renderAsync"/> 應為 <c>() => InvokeAsync(StateHasChanged)</c>。
    /// 合併同一時間連續觸發的通知，降低 Blazor WASM 在大量屬性／集合通知時的渲染壓力與例外風險。
    /// </summary>
    public static IDisposable Attach(MainViewModel vm, Func<Task> renderAsync)
    {
        var undo = new List<Action>();

        void Add(Action dispose) => undo.Add(dispose);

        var pending = false;
        var flushing = false;
        void RequestRender()
        {
            pending = true;
            if (flushing)
                return;
            flushing = true;
            _ = FlushAsync();

            async Task FlushAsync()
            {
                try
                {
                    while (pending)
                    {
                        pending = false;
                        await Task.Yield();
                        try
                        {
                            await renderAsync().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            // 避免單次轉譯例外讓整頁進入 #blazor-error-ui；例外仍應在開發時被看見
                            System.Diagnostics.Debug.WriteLine($"MainViewModelRefreshBinder render: {ex}");
                        }
                    }
                }
                finally
                {
                    flushing = false;
                    if (pending)
                        RequestRender();
                }
            }
        }

        void OnProp(object? s, PropertyChangedEventArgs e) => RequestRender();
        void OnCol(object? s, NotifyCollectionChangedEventArgs e) => RequestRender();

        vm.PropertyChanged += OnProp;
        Add(() => vm.PropertyChanged -= OnProp);

        HookObs(vm.TypedSearch);
        HookObs(vm.Advanced);
        HookObs(vm.Modifying);

        void HookObs(INotifyPropertyChanged o)
        {
            o.PropertyChanged += OnProp;
            Add(() => o.PropertyChanged -= OnProp);
        }

        HookCol(vm.SupportedResources);
        HookCol(vm.SearchParameters);
        HookCol(vm.SearchIncludes);
        HookCol(vm.SearchRevIncludes);
        HookCol(vm.DraftSearchParameters);
        HookCol(vm.IncludeChoices);
        HookCol(vm.RevIncludeChoices);
        HookCol(vm.QueryParameters);
        HookCol(vm.ModifyingParameters);
        HookCol(vm.ValidationErrors);
        HookCol(vm.QueryResultTreeRoots);
        HookCol(vm.Advanced.Chains);
        HookCol(vm.Advanced.Composites);
        HookCol(vm.Advanced.Filters);
        HookCol(vm.Modifying.ElementChoices);
        HookCol(vm.Modifying.Elements);
        HookCol(vm.Modifying.SortLines);
        HookCol(vm.TypedSearch.CompositePartRows);

        void HookCol(INotifyCollectionChanged col)
        {
            col.CollectionChanged += OnCol;
            Add(() => col.CollectionChanged -= OnCol);
        }

        return new CallbackDisposable(() =>
        {
            for (var i = undo.Count - 1; i >= 0; i--)
                undo[i]();
        });
    }

    private sealed class CallbackDisposable(Action dispose) : IDisposable
    {
        private Action? _dispose = dispose;

        public void Dispose()
        {
            Interlocked.Exchange(ref _dispose, null)?.Invoke();
        }
    }
}
