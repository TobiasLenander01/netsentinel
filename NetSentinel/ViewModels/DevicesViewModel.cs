
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Identity.Client;
using NetSentinel.Interfaces;
using NetSentinel.Models;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class DevicesViewModel : ViewModelBase, IRefreshable
{
    private readonly IDeviceService _deviceService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;
    private readonly DeviceDialogViewModel _deviceDialogViewModel;

    [ObservableProperty]
    private ObservableCollection<Device> _devices = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    public DevicesViewModel(INavigationService navigator, IDeviceService deviceService, DialogManager dialogManager, ToastManager toastManager, DeviceDialogViewModel deviceDialogViewModel) : base(navigator)
    {
        Title = "Devices";
        _deviceService = deviceService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _deviceDialogViewModel = deviceDialogViewModel;
    }

    public Task RefreshAsync() => LoadDevicesAsync();

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
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to load devices")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task ShowNewDeviceDialog()
    {
        try
        {
            await _deviceDialogViewModel.LoadNewDeviceCommand.ExecuteAsync(null);
            _dialogManager.CreateDialog(_deviceDialogViewModel)
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
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to open new device")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
    }

    [RelayCommand]
    public async Task ShowEditDeviceDialog(Device device)
    {
        try
        {
            await _deviceDialogViewModel.LoadExistingDeviceCommand.ExecuteAsync(device);
            _dialogManager.CreateDialog(_deviceDialogViewModel)
            .Dismissible()
            .WithSuccessCallback(() =>
                {
                    _toastManager
                        .CreateToast("Device edited")
                        .DismissOnClick()
                        .ShowSuccess();
                    _ = LoadDevicesAsync();
                })
            .Show();
        }
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to open device")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
    }

    [RelayCommand]
    public void ShowDeleteDeviceDialog(Device device)
    {
        _dialogManager.CreateDialog(
            "Are you absolutely sure?", 
            "This will permanently delete your device.")
        .WithPrimaryButton("Confirm", async () =>
        {
            try
            {
                await _deviceService.DeleteDevice(device);
                _toastManager.CreateToast("Device deleted")
                    .WithContent("Device deleted successfully!")
                    .DismissOnClick()
                    .ShowSuccess();
                await LoadDevicesAsync();
            }
            catch (Exception ex)
            {
                _toastManager
                    .CreateToast("Failed to delete device")
                    .WithContent(ex.Message)
                    .DismissOnClick()
                    .ShowError();
            }
        }, DialogButtonStyle.Destructive)
        .WithCancelButton("Cancel")
        .WithMaxWidth(512)
        .Dismissible()
        .Show();
    }
}