using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Interfaces;
using NetSentinel.Models;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class DeviceDialogViewModel : ViewModelBase
{
    private readonly IDeviceService _deviceService;
    private readonly IEmployeeService _employeeService;
    private readonly DialogManager _dialogManager;
    private readonly ToastManager _toastManager;

    private Device _device = new Device();
    private bool _isEditingExistingDevice = false;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(50, ErrorMessage = "Name must be 50 characters or less")]
    private string _name = String.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "IP Address is required")]
    [MaxLength(50, ErrorMessage = "IP Address must be 50 characters or less")]
    private string _ipAddress = String.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Serial Number is required")]
    [MaxLength(50, ErrorMessage = "Serial Number must be 50 characters or less")]
    private string _serialNumber = String.Empty;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Owner is required")]
    private Employee? _selectedEmployee;

    [ObservableProperty]
    private bool _isLoading;

    public DeviceDialogViewModel(DialogManager dialogManager, ToastManager toastManager, IDeviceService deviceService, IEmployeeService employeeService) : base()
    {
        Title = "Device Dialog";
        _deviceService = deviceService;
        _employeeService = employeeService;
        _dialogManager = dialogManager;
        _toastManager = toastManager;
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
        catch (Exception ex)
        {
            _toastManager
                .CreateToast("Failed to save device")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
            _dialogManager.Close(this, new CloseDialogOptions { Success = false });
        }
        finally
        {
            _dialogManager.Dispose();
        }
    }

    private async Task InitializeProperties(Device device)
    {
        // Load employees into drop down menu
        IsLoading = true;
        try
        {
            var list = await _employeeService.ReadAllEmployeesAsync();
            Employees = new ObservableCollection<Employee>(list);
        }
        catch (Exception ex)
        {
            Employees = new ObservableCollection<Employee>();
            _toastManager
                .CreateToast("Failed to load employees")
                .WithContent(ex.Message)
                .DismissOnClick()
                .ShowError();
        }
        finally
        {
            IsLoading = false;
        }

        // Observe device properties
        _device = device;
        Name = device.Name;
        IpAddress = device.IPAddress;
        SerialNumber = device.SerialNumber;
        SelectedEmployee = device.Owner is null
            ? null
            : Employees.FirstOrDefault(e => e.Id == device.Owner.Id) ?? device.Owner;

        ClearErrors();
    }
}
