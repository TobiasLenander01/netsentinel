using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class EmployeesViewModel : ViewModelBase
{
    public EmployeesViewModel(INavigation navigator) : base(navigator)
    {
        Title = "Employees";
    }
}