using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class EmployeesViewModel : ViewModelBase
{
    public EmployeesViewModel(IAppNavigator navigator) : base(navigator)
    {
        Title = "Employees";
    }
}