using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(IAppNavigator navigator) : base(navigator)
    {
        
    }

    [RelayCommand]
    public void GoToDashboard()
    {
        Navigator.GoToDashboard();
    }

    [RelayCommand]
    public void GoToEmployees()
    {
        Navigator.GoToEmployees();
    }
    
    // You'll need to add this method to your NavigationService first
    [RelayCommand]
    public void GoToSettings()
    {
        // Navigator.GoToSettings(); // Uncomment once added to service
    }
}
