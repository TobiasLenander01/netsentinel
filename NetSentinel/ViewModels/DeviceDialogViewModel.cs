using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
    private bool _isEditingExistingDevice = false;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required")]
    private string _name = String.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "IP Address is required")]
    private string _ipAddress = String.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Serial Number is required")]
    private string _serialNumber = String.Empty;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Owner is required")]
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
        _isEditingExistingDevice = false;
        await InitializeProperties(new Device());
    }

    [RelayCommand]
    public async Task LoadExistingDeviceAsync(Device device)
    {
        Title = "Edit Device";
        _isEditingExistingDevice = true;
        await InitializeProperties(device);
    }

    [RelayCommand]
    private async Task SaveDeviceAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
            return;

        // Get device properties from form
        _device.Name = Name;
        _device.IPAddress = IpAddress;
        _device.SerialNumber = SerialNumber;
        _device.Owner = SelectedEmployee;

        try
        {
            if (_isEditingExistingDevice)
                await _deviceService.UpdateDeviceAsync(_device);
            else
                await _deviceService.AddDeviceAsync(_device);

            _dialogManager.Close(this, new CloseDialogOptions { Success = true });
        }
        catch
        {
            _dialogManager.Close(this, new CloseDialogOptions { Success = false });
        }
        finally
        {
            _dialogManager.Dispose();
        }
    }

    private async Task InitializeProperties(Device device)
    {
        ClearErrors();

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
