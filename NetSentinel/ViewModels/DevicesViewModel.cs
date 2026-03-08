
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Models;
using NetSentinel.Services;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class DevicesViewModel : ViewModelBase
{
    private readonly DeviceService _deviceService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;
    private readonly AddDeviceViewModel _addDeviceViewModel;

    [ObservableProperty]
    private ObservableCollection<Device> _devices = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    public DevicesViewModel(NavigationService navigator, DeviceService deviceService, DialogManager dialogManager, ToastManager toastManager, AddDeviceViewModel addDeviceViewModel) : base(navigator)
    {
        Title = "Devices";
        _deviceService = deviceService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _addDeviceViewModel = addDeviceViewModel;
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

    [RelayCommand]
    public void ShowAddDeviceDialog()
    {
        _dialogManager.CreateDialog(_addDeviceViewModel)
        .Dismissible()
        .WithSuccessCallback(() =>
            {
                _toastManager
                    .CreateToast("Device added")
                    .DismissOnClick()
                    .ShowSuccess();
                _ = LoadDevicesAsync();
            })
        .Show();
    }
}