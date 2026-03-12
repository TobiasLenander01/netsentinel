using System;
using NetSentinel.ViewModels;

namespace NetSentinel.Interfaces;

public interface INavigationService
{
    public void GoToDashboard();
    public void GoToDevices();
    public void GoToDepartments();
    public void GoToEmployees();
    // public void GoToIncidents();
    public void GoToSecurityMeasures();
    public void GoToSettings();
    public void GoBack();
}
