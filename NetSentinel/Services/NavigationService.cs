using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using NetSentinel.Interfaces;
using NetSentinel.ViewModels;

namespace NetSentinel.Services;

/// <summary>
/// Implementation of the navigation functionality.
/// Purpose: Used by the viewmodels to switch view.
/// </summary>

public partial class NavigationService : ObservableObject, INavigation
{
    private readonly Stack<ViewModelBase> _navigationHistoryStack = new();
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ViewModelBase? _currentView;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private void NavigateTo(ViewModelBase viewModel)
    {
        if (CurrentView != null)
            _navigationHistoryStack.Push(CurrentView);

        CurrentView = viewModel;
    }

    public void GoToDashboard()
    {
        if (CurrentView is not DashboardViewModel)
        {
            var dashboard = _serviceProvider.GetRequiredService<DashboardViewModel>();
            NavigateTo(dashboard);
        }
    }

    public void GoToEmployees()
    {
        if (CurrentView is not EmployeesViewModel)
        {
            var employees = _serviceProvider.GetRequiredService<EmployeesViewModel>();
            NavigateTo(employees);
        }
    }

    public void GoToSettings()
    {
        if (CurrentView is not SettingsViewModel)
        {
            var settings = _serviceProvider.GetRequiredService<SettingsViewModel>();
            NavigateTo(settings);
        }
    }

    public void GoBack()
    {
        if (_navigationHistoryStack.Count > 0)
            CurrentView = _navigationHistoryStack.Pop();
    }
}