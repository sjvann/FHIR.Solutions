using System.Collections.Specialized;
using System.ComponentModel;
using Fhir.QueryBuilder.ViewModels;

namespace Fhir.QueryBuilder.App.Blazor.Services;

/// <summary>將 <see cref="MainViewModel"/> 的通知轉成 Blazor 重繪（Observable / 集合變更）。</summary>
internal static class MainViewModelRefreshBinder
{
    public static IDisposable Attach(MainViewModel vm, Action invalidate)
    {
        var undo = new List<Action>();

        void Add(Action dispose) => undo.Add(dispose);

        void OnProp(object? s, PropertyChangedEventArgs e) => invalidate();
        void OnCol(object? s, NotifyCollectionChangedEventArgs e) => invalidate();

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
