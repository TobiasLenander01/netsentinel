using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSentinel.Models;
using NetSentinel.Services;
using ShadUI;

namespace NetSentinel.ViewModels;

public partial class EmployeesViewModel : ViewModelBase
{
    private readonly EmployeeService _employeeService;
    private readonly ToastManager _toastManager;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    public EmployeesViewModel(NavigationService navigator, EmployeeService employeeService, ToastManager toastManager) : base(navigator)
    {
        Title = "Employees";
        _employeeService = employeeService;
        _toastManager = toastManager;
    }

    [RelayCommand]
    private async Task LoadEmployeesAsync()
    {
        IsLoading = true;
        IsEmpty = false;
        Employees = new ObservableCollection<Employee>();

        try
        {
            var employeeList = await _employeeService.ReadAllEmployeesAsync();
            Employees = new ObservableCollection<Employee>(employeeList);
            IsEmpty = Employees.Count == 0;
        }
        catch (Exception ex)
        {
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
    }
}
