using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;
using NetSentinel.Models;
using NetSentinel.Services;

namespace NetSentinel.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly DeviceService _deviceService;
    public DashboardViewModel(INavigation navigator, DeviceService deviceService) : base(navigator)
    {
        Title = "Dashboard";
        _deviceService = deviceService;
    }

    [RelayCommand]
    public async Task TestAddDevice()
    {
        Device device = new Device("Filippa's iPhone", "192.162.1.4", "7656788");
        await _deviceService.AddDeviceAsync(device);
    }
}