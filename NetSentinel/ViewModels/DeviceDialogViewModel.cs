using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Models;
using NetSentinel.Services;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class DeviceDialogViewModel : ViewModelBase
{
    private readonly DeviceService _deviceService;
    private readonly EmployeeService _employeeService;
    private readonly DialogManager _dialogManager;

    private Device _device = new Device();

    [ObservableProperty]
    private string _name = String.Empty;

    [ObservableProperty]
    private string _ipAddress = String.Empty;

    [ObservableProperty]
    private string _serialNumber = String.Empty;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    [ObservableProperty]
    private Employee? _selectedEmployee;

    [ObservableProperty]
    private bool _isLoading;

    public DeviceDialogViewModel(NavigationService navigator, DialogManager dialogManager, DeviceService deviceService, EmployeeService employeeService) : base(navigator)
    {
        Title = "Device Dialog";
        _deviceService = deviceService;
        _employeeService = employeeService;
        _dialogManager = dialogManager;
    }

    [RelayCommand]
    private async Task LoadNewDeviceAsync()
    {
        Title = "Add Device";
        InitializeProperties(new Device());
    }

    [RelayCommand]
    public async Task LoadExistingDeviceAsync(Device device)
    {
        Title = "Edit Device";
        InitializeProperties(device);
    }

    [RelayCommand]
    private async Task SaveDeviceAsync()
    {
        if (SelectedEmployee is null) return;

        var device = new Device
        {
            Name = Name,
            IPAddress = IpAddress,
            SerialNumber = SerialNumber,
            Owner = SelectedEmployee
        };

        await _deviceService.AddDeviceAsync(device);
        _dialogManager.Close(this, new CloseDialogOptions { Success = true });
    }

    private async void InitializeProperties(Device device)
    {
        // Load employees into drop down menu
        IsLoading = true;
        var list = await _employeeService.ReadAllEmployeesAsync();
        Employees = new ObservableCollection<Employee>(list);
        IsLoading = false;

        // Observe device properties
        _device = device;
        Name = device.Name;
        IpAddress = device.IPAddress;
        SerialNumber = device.SerialNumber;
        SelectedEmployee = device.Owner;
    }
}
