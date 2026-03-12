using System;
using NetSentinel.Services;
using ShadUI;

namespace NetSentinel.ViewModels;

public class IncidentsViewModel : ViewModelBase
{
    private readonly IncidentService _incidentService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;

    public IncidentsViewModel(NavigationService navigator, IncidentService incidentService, DialogManager dialogManager, ToastManager toastManager) : base(navigator)
    {
        Title = "Incidents";
        _incidentService = incidentService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        
    }
}
