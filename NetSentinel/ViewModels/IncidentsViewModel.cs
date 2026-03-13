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

    [ObservableProperty]
    private ObservableCollection<Incident> _incidents = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    public IncidentsViewModel(INavigationService navigator, IIncidentService incidentService, DialogManager dialogManager, ToastManager toastManager) : base(navigator)
    {
        Title = "Incidents";
        _incidentService = incidentService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
    }

    public Task RefreshAsync() => LoadIncidentsAsync();

    [RelayCommand]
    private async Task LoadIncidentsAsync() {
        IsLoading = true;
        IsEmpty = false;
        Incidents = new ObservableCollection<Incident>();

        try {
            var list = await _incidentService.ReadAllIncidentsAsync();
            Incidents = new ObservableCollection<Incident>(list);
            IsEmpty = Incidents.Count == 0;
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to load incidents")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }

        finally {
            IsLoading = false;
        }

    }

    [RelayCommand]
    private async Task DeleteIncidentAsync(Incident incident)
    {
        try
        {
            await _incidentService.DeleteIncidentAsync(incident);
            Incidents.Remove(incident);
            IsEmpty = Incidents.Count == 0;
            _toastManager
                .CreateToast("Incident deleted")
                .DismissOnClick()
                .ShowSuccess();
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to delete incident")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
    }

}
