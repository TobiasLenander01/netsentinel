using System.ComponentModel;
using NetSentinel.ViewModels;

namespace NetSentinel.Interfaces;

public interface INavigationService : INotifyPropertyChanged
{
    ViewModelBase? CurrentView { get; }
    void GoToDashboard();
    void GoToDevices();
    void GoToDepartments();
    void GoToEmployees();
    void GoToIncidents();
    void GoToSecurityMeasures();
    void GoToSettings();
    void GoToAIReport();
    void GoBack();
}
