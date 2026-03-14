using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
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

    private const int PageSize = 10;

    [ObservableProperty]
    private ObservableCollection<Device> _devices = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    private int _currentPage = 1;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    private int _totalPages = 1;

    [ObservableProperty]
    private int _totalCount;

    public string PageInfo => $"Page {CurrentPage} of {TotalPages}";

    public DevicesViewModel(IDeviceService deviceService, DialogManager dialogManager, ToastManager toastManager, DeviceDialogViewModel deviceDialogViewModel) : base()
    {
        Title = "Devices";
        _deviceService = deviceService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
        _deviceDialogViewModel = deviceDialogViewModel;
    }

    partial void OnCurrentPageChanged(int value) => OnPropertyChanged(nameof(PageInfo));
    partial void OnTotalPagesChanged(int value) => OnPropertyChanged(nameof(PageInfo));

    public Task RefreshAsync() => LoadDevicesAsync();

    [RelayCommand]
    private async Task LoadDevicesAsync()
    {
        IsLoading = true;
        IsEmpty = false;
        Devices = new ObservableCollection<Device>();

        try
        {
            var (items, totalCount) = await _deviceService.ReadDevicesPagedAsync(CurrentPage, PageSize);
            Devices = new ObservableCollection<Device>(items);
            TotalCount = totalCount;
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            IsEmpty = totalCount == 0;
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

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private async Task PreviousPageAsync()
    {
        CurrentPage--;
        await LoadDevicesAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private async Task NextPageAsync()
    {
        CurrentPage++;
        await LoadDevicesAsync();
    }

    private bool CanGoPrevious() => CurrentPage > 1;
    private bool CanGoNext() => CurrentPage < TotalPages;

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
            catch (DbUpdateException)
            {
                _toastManager
                    .CreateToast("Cannot delete device")
                    .WithContent("This device has incidents. Delete those incidents first.")
                    .DismissOnClick()
                    .ShowError();
            }
            catch (InvalidOperationException ex)
            {
                _toastManager
                    .CreateToast("Cannot delete device")
                    .WithContent("This device has incidents. Delete those incidents first.")
                    .DismissOnClick()
                    .ShowError();
                Console.WriteLine(ex);
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
