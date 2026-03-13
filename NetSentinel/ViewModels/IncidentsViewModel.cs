using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;
using NetSentinel.Models;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class IncidentsViewModel : ViewModelBase
{
    private readonly IIncidentService _incidentService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;

    private const int PageSize = 10;

    [ObservableProperty]
    private ObservableCollection<Incident> _incidents = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    private int _currentPage = 1;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    private int _totalPages = 1;

    [ObservableProperty]
    private int _totalCount;

    public string PageInfo => $"Page {CurrentPage} of {TotalPages}";

    public IncidentsViewModel(INavigationService navigator, IIncidentService incidentService, DialogManager dialogManager, ToastManager toastManager) : base(navigator)
    {
        Title = "Incidents";
        _incidentService = incidentService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
    }

    partial void OnCurrentPageChanged(int value) => OnPropertyChanged(nameof(PageInfo));
    partial void OnTotalPagesChanged(int value) => OnPropertyChanged(nameof(PageInfo));

    public Task RefreshAsync() => LoadIncidentsAsync();

    [RelayCommand]
    private async Task LoadIncidentsAsync()
    {
        IsLoading = true;
        IsEmpty = false;
        Incidents = new ObservableCollection<Incident>();

        try
        {
            var (items, totalCount) = await _incidentService.ReadIncidentsPagedAsync(CurrentPage, PageSize);
            Incidents = new ObservableCollection<Incident>(items);
            TotalCount = totalCount;
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            IsEmpty = totalCount == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private async Task PreviousPageAsync()
    {
        CurrentPage--;
        await LoadIncidentsAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private async Task NextPageAsync()
    {
        CurrentPage++;
        await LoadIncidentsAsync();
    }

    private bool CanGoPrevious() => CurrentPage > 1;
    private bool CanGoNext() => CurrentPage < TotalPages;

    [RelayCommand]
    private async Task DeleteIncidentAsync(Incident incident)
    {
        await _incidentService.DeleteIncidentAsync(incident);
        await LoadIncidentsAsync();
    }
}
