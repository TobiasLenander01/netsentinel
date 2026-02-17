using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using NetSentinel.Interfaces;
using NetSentinel.ViewModels;

namespace NetSentinel.Services;

public partial class NavigationService : ObservableObject, IAppNavigator
{
    // Stack to keep track of navigation history
    private readonly Stack<ViewModelBase> _navigationStack = new();

    // Current view being displayed
    [ObservableProperty]
    private ViewModelBase _currentView;

    public NavigationService()
    {
        // Set initial view
        _currentView = new DashboardViewModel(this);
    }

    private void NavigateTo(ViewModelBase viewModel)
    {
        if (CurrentView != null)
            _navigationStack.Push(CurrentView);

        CurrentView = viewModel;
    }

    public void GoToDashboard()
    {
        if (CurrentView is not DashboardViewModel)
            NavigateTo(new DashboardViewModel(this));
    }

    public void GoToEmployees()
    {
        if (CurrentView is not EmployeesViewModel)
            NavigateTo(new EmployeesViewModel(this));
    }

    public void GoToSettings()
    {
        if (CurrentView is not SettingsViewModel)
            NavigateTo(new SettingsViewModel(this));
    }

    public void GoBack()
    {
        if (_navigationStack.Count > 0)
            CurrentView = _navigationStack.Pop();
    }
}