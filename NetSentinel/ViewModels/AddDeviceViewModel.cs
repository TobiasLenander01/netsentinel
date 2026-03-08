using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Models;
using NetSentinel.Services;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class AddDeviceViewModel : ViewModelBase
{
    private readonly DeviceService _deviceService;
    private readonly EmployeeService _employeeService;
    private readonly DialogManager _dialogManager;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _ipAddress = string.Empty;

    [ObservableProperty]
    private string _serialNumber = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    [ObservableProperty]
    private Employee? _selectedEmployee;

    [ObservableProperty]
    private bool _isLoading;

    public AddDeviceViewModel(NavigationService navigator, DialogManager dialogManager, DeviceService deviceService, EmployeeService employeeService) : base(navigator)
    {
        Title = "Add Device";
        _deviceService = deviceService;
        _employeeService = employeeService;
        _dialogManager = dialogManager;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        var list = await _employeeService.ReadAllEmployeesAsync();
        Employees = new ObservableCollection<Employee>(list);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task SaveAsync()
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
}
