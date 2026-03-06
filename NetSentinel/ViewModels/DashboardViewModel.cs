using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Models;
using NetSentinel.Services;

namespace NetSentinel.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    public DashboardViewModel(NavigationService navigator) : base(navigator)
    {
        Title = "Dashboard";
    }
}