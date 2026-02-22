using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;

namespace NetSentinel.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    public DashboardViewModel(INavigation navigator) : base(navigator)
    {
        Title = "Dashboard";
    }
}