using NetSentinel.ViewModels;

namespace NetSentinel.Interfaces;

public interface IAppNavigator
{
    ViewModelBase CurrentView { get; set; } 
    
    void GoToDashboard();
    void GoToEmployees();
    void GoBack();
}