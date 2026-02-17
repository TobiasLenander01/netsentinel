using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class EmployeesViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "Employee Management";

    // Inject the navigator so we can go back
    public EmployeesViewModel(IAppNavigator navigator) : base(navigator)
    {
        
    }

    [RelayCommand]
    private void GoBack()
    {
        Navigator.GoBack();
    }
}