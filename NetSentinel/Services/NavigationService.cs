using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using NetSentinel.ViewModels;

namespace NetSentinel.Services;

/// <summary>
/// Implementation of the navigation functionality.
/// Purpose: Used by the viewmodels to switch view.
/// </summary>

public partial class NavigationService : ObservableObject
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
            var dashboardView = _serviceProvider.GetRequiredService<DashboardViewModel>();
            NavigateTo(dashboardView);
        }
    }

    public void GoToDevices()
    {
        if (CurrentView is not DevicesViewModel)
        {
            var devicesView = _serviceProvider.GetRequiredService<DevicesViewModel>();
            NavigateTo(devicesView);
        }
    }

    public void GoToDepartments()
    {
        if (CurrentView is not DepartmentsViewModel)
        {
            var departmentsView = _serviceProvider.GetRequiredService<DepartmentsViewModel>();
            NavigateTo(departmentsView);
        }
    }

    public void GoToSettings()
    {
        if (CurrentView is not SettingsViewModel)
        {
            var settingsView = _serviceProvider.GetRequiredService<SettingsViewModel>();
            NavigateTo(settingsView);
        }
    }

    public void GoBack()
    {
        if (_navigationHistoryStack.Count > 0)
            CurrentView = _navigationHistoryStack.Pop();
    }

    public void GoToIncidents()
    {
        if (CurrentView is not IncidentsViewModel)
        {
            var incidentsView = _serviceProvider.GetRequiredService<IncidentsViewModel>();
            NavigateTo(incidentsView);
        }
    }

}