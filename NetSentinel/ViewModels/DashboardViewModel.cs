using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _testMessage = "Welcome to NetSentinel";

    public DashboardViewModel(IAppNavigator navigator) : base(navigator)
    {
        
    }

    [RelayCommand]
    private void GoToEmployees()
    {
        Navigator.GoToEmployees();
    }
}