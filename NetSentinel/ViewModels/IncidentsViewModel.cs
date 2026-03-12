using System;
using NetSentinel.Interfaces;
using ShadUI;

namespace NetSentinel.ViewModels;

public class IncidentsViewModel : ViewModelBase
{
    private readonly IIncidentService _incidentService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;

    public IncidentsViewModel(INavigationService navigator, IIncidentService incidentService, DialogManager dialogManager, ToastManager toastManager) : base(navigator)
    {
        Title = "Incidents";
        _incidentService = incidentService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
    }

    


}
