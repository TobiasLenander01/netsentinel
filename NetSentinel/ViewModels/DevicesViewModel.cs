
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Models;
using NetSentinel.Services;

namespace NetSentinel.ViewModels;

public partial class DevicesViewModel : ViewModelBase
{
    private readonly DeviceService _deviceService;

    [ObservableProperty]
    private ObservableCollection<Device> _devices = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    public DevicesViewModel(NavigationService navigator, DeviceService deviceService) : base(navigator)
    {
        Title = "Devices";
        _deviceService = deviceService;
    }

    [RelayCommand]
    private async Task DeleteDeviceAsync(Device device)
    {
        await _deviceService.DeleteDevice(device);
        Devices.Remove(device);
        IsEmpty = Devices.Count == 0;
    }

    [RelayCommand]
    private async Task LoadDevicesAsync()
    {
        IsLoading = true;
        IsEmpty = false;
        Devices = new ObservableCollection<Device>();

        try
        {
            var deviceList = await _deviceService.ReadAllDevicesAsync();
            Devices = new ObservableCollection<Device>(deviceList);
            IsEmpty = Devices.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }
}