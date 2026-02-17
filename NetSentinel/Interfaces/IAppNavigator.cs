using NetSentinel.ViewModels;

namespace NetSentinel.Interfaces;

/// <summary>
/// Exposes navigation methods for the app. ViewModels can call these to change the current view.
/// </summary>

public interface IAppNavigator
{
    ViewModelBase CurrentView { get; set; }

    void GoToDashboard();
    void GoToEmployees();
    void GoToSettings();
    void GoBack();
}