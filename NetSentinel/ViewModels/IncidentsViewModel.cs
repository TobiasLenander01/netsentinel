using System;
using NetSentinel.Services;

namespace NetSentinel.ViewModels;

public class IncidentsViewModel : ViewModelBase
{

    
    public IncidentsViewModel(NavigationService navigator) : base(navigator)
    {
        Title = "Incidents";
    }
}
