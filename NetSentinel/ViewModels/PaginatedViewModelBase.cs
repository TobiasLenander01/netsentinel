using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public abstract partial class PaginatedViewModelBase : ViewModelBase, IRefreshable
{
    protected virtual int PageSize => 10;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(FirstPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(LastPageCommand))]
    private int _currentPage = 1;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(LastPageCommand))]
    private int _totalPages = 1;

    [ObservableProperty]
    private int _totalCount;

    public string PageInfo => $"Page {CurrentPage} of {TotalPages}";

    partial void OnCurrentPageChanged(int value) => OnPropertyChanged(nameof(PageInfo));
    partial void OnTotalPagesChanged(int value) => OnPropertyChanged(nameof(PageInfo));

    public Task RefreshAsync() => LoadPageAsync();

    protected abstract Task LoadPageAsync();

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private async Task PreviousPageAsync()
    {
        CurrentPage--;
        await LoadPageAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private async Task NextPageAsync()
    {
        CurrentPage++;
        await LoadPageAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoFirst))]
    private async Task FirstPageAsync()
    {
        CurrentPage = 1;
        await LoadPageAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoLast))]
    private async Task LastPageAsync()
    {
        CurrentPage = TotalPages;
        await LoadPageAsync();
    }

    private bool CanGoPrevious() => CurrentPage > 1;
    private bool CanGoNext() => CurrentPage < TotalPages;
    private bool CanGoFirst() => CurrentPage > 1;
    private bool CanGoLast() => CurrentPage < TotalPages;
}
