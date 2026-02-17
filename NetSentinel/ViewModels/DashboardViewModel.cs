using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces; // Ensure this is using your Interface namespace

namespace NetSentinel.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IAppNavigator _navigator;

    [ObservableProperty]
    private string _testMessage = "Welcome to NetSentinel";

    // Constructor Injection: We ask for the Navigator here
    public DashboardViewModel(IAppNavigator navigator)
    {
        _navigator = navigator;
    }

    // This generates a "GoToEmployeesCommand" that the button binds to
    [RelayCommand]
    private void GoToEmployees()
    {
        _navigator.GoToEmployees();
    }
}