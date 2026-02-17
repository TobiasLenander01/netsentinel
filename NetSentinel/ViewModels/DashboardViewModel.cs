using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "Dashboard";

    public DashboardViewModel(IAppNavigator navigator) : base(navigator)
    {
        Title = _title;
    }

    [RelayCommand]
    private void GoToEmployees()
    {
        Navigator.GoToEmployees();
    }
}