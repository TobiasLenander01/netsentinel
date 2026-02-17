using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using NetSentinel.Interfaces;
using NetSentinel.ViewModels;

namespace NetSentinel;

public partial class AppNavigator : ObservableObject, IAppNavigator
{
    private readonly Stack<ViewModelBase> _navigationStack = new();

    [ObservableProperty]
    private ViewModelBase _currentView;

    public AppNavigator()
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

    public void GoBack()
    {
        if (_navigationStack.Count > 0)
            CurrentView = _navigationStack.Pop();
    }
}