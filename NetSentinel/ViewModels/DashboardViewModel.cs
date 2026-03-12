using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;
using NetSentinel.Models;

namespace NetSentinel.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    public DashboardViewModel(INavigationService navigator) : base(navigator)
    {
        Title = "Dashboard";
    }
}